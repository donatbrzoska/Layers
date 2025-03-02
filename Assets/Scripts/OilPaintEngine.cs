using UnityEngine;
using System.Collections.Generic;

public class OilPaintEngine : MonoBehaviour
{
    // These attributes are set through inspector
    public int TH_GROUP_SIZE_X;
    public int TH_GROUP_SIZE_Y;

    public int CANVAS_LAYERS_MAX;
    public int RAKEL_LAYERS_MAX;
    public int STEPS_PER_FRAME;

    public bool EVALUATE;

    public GameObject _InputManager;
    public GameObject _ScriptControl;

    private InputManager InputManager;
    private ScriptControl Control;

    private bool InputLocked = false;

    public Configuration Config { get; private set; }
    public PaintMode PaintMode
    {
        get
        {
            if (Config.TransferConfig.CanvasSnapshotBufferEnabled)
            {
                if (Config.TransferConfig.DeletePickedUpFromCSB)
                {
                    return PaintMode.LightSmearing;
                }
                else
                {
                    return PaintMode.IntenseSmearing;
                }
            }
            else
            {
                return PaintMode.Blurry;
            }
        }

        private set
        {
            switch (value)
            {
                case PaintMode.Blurry:
                    UpdateCanvasSnapshotBufferEnabled(false);
                    break;
                case PaintMode.IntenseSmearing:
                    UpdateCanvasSnapshotBufferEnabled(true);
                    UpdateDeletePickedUpFromCSB(false);
                    break;
                case PaintMode.LightSmearing:
                    UpdateCanvasSnapshotBufferEnabled(true);
                    UpdateDeletePickedUpFromCSB(true);
                    break;
                default:
                    break;
            }
        }
    }


    public TransferEngine TransferEngine;
    private Canvas_ Canvas;
    public Rakel Rakel;
    private InputInterpolator InputInterpolator;

    void Awake()
    {
        Config = new Configuration();
        InputManager = _InputManager.GetComponent<InputManager>();
        Control = _ScriptControl.GetComponent<ScriptControl>();

        if (EVALUATE)
        {
            Config.TextureResolution = 60;

            GameObject.Find("CSB Toggle").SetActive(false);
            GameObject.Find("CSB Delete Toggle").SetActive(false);
        }
        else
        {
            GameObject.Find("Paint Mode Text").SetActive(false);
        }
    }

    void Start()
    {
        ComputeShaderTask.ThreadGroupSize = new Vector2Int(TH_GROUP_SIZE_X, TH_GROUP_SIZE_Y);

        InputInterpolator = new InputInterpolator();

        CreateTransferEngine();
        CreateCanvas();
        CreateRakel();
    }

    void CreateTransferEngine()
    {
        DisposeTransferEngine();
        TransferEngine = new TransferEngine(STEPS_PER_FRAME > 0, Config.TransferConfig, InputInterpolator);
    }

    void CreateCanvas()
    {
        DisposeCanvas();

        Vector3 localScale = new Vector3(
            Config.CanvasConfig.Width / 10, // convert world space to local scale
            1,
            Config.CanvasConfig.Height / 10); // convert world space to local scale
        GameObject canvas_ = GameObject.Find("Canvas");
        Transform canvasTransform = canvas_.GetComponent<Transform>();
        canvasTransform.localScale = localScale;

        // HACK:
        // For some reason, writing to transform.localScale does not have an
        // effect, so we need to recreate the canvas game object every time
        GameObject canvas = GameObject.CreatePrimitive(PrimitiveType.Plane);
        canvas.transform.position = canvasTransform.position;
        canvas.transform.localScale = canvasTransform.localScale;
        canvas.transform.rotation = canvasTransform.rotation;
        Destroy(canvas_);
        canvas.name = "Canvas";
        canvas.tag = "Canvas";

        Vector3 position = canvas.transform.position;
        Canvas = new Canvas_(
            Config.CanvasConfig.Width, Config.CanvasConfig.Height,
            CANVAS_LAYERS_MAX, Config.CanvasConfig.CellVolume,
            position,
            Config.TextureResolution,
            Config.CanvasConfig.NormalScale,
            Config.ColorSpace);

        Renderer renderer = canvas.GetComponent<Renderer>();
        renderer.material.SetTexture("_MainTex", Canvas.Texture);
        renderer.material.EnableKeyword("_NORMALMAP");
        renderer.material.SetTexture("_BumpMap", Canvas.NormalMap);
        renderer.material.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");

        // Trigger lighting update
        Canvas.Render(Canvas.GetFullShaderRegion());

        Debug.Log("Texture is "
                  + Canvas.Texture.width + "x" + Canvas.Texture.height
                  + " = " + Canvas.Texture.width * Canvas.Texture.height);

        TransferEngine.SetCanvas(Canvas);
        InputInterpolator.SetCanvas(Canvas);
    }

    void CreateRakel()
    {
        DisposeRakel();

        Rakel = new Rakel(
            Config.RakelConfig.Length, Config.RakelConfig.Width, Config.TextureResolution,
            RAKEL_LAYERS_MAX, Config.RakelConfig.CellVolume);

        Debug.Log("Rakel is "
                  + Config.RakelConfig.Length * Config.TextureResolution + "x" + Config.RakelConfig.Width * Config.TextureResolution
                  + " = " + Config.RakelConfig.Length * Config.TextureResolution * Config.RakelConfig.Width * Config.TextureResolution);

        TransferEngine.SetRakel(Rakel);
        InputInterpolator.SetRakel(Rakel);
    }

    void Update()
    {
        if (TransferEngine.IsDone())
        {
            InputLocked = false;
        }

        if (InputManager.InStroke && !InputLocked)
        {
            if (InputManager.StrokeBegin)
            {
                TransferEngine.NewStroke(
                    Config.RakelConfig.TiltNoiseEnabled,
                    Config.RakelConfig.TiltNoiseFrequency,
                    Config.RakelConfig.TiltNoiseAmplitude,
                    Config.TransferConfig.FloatingZLength,
                    Config.TransferConfig.CanvasSnapshotBufferEnabled);
            }
            InputInterpolator.AddNode(InputManager.InputState, InputManager.StrokeBegin, Config.TextureResolution);
        }

        // Prevent accidental tap while waiting for stroke computations to finish
        bool inputForStrokeHasEnded = !InputManager.InStroke;
        if (inputForStrokeHasEnded && !TransferEngine.IsDone())
        {
            InputLocked = true;
        }

        TransferEngine.ProcessSteps(STEPS_PER_FRAME);
    }

    private void OnDestroy()
    {
        DisposeTransferEngine();
        DisposeRakel();
        DisposeCanvas();
    }

    private void DisposeTransferEngine()
    {
        TransferEngine?.Dispose();
    }

    private void DisposeRakel()
    {
        Rakel?.Dispose();
    }

    private void DisposeCanvas()
    {
        Canvas?.Dispose();
    }

    // ****************************************************************************************
    // ***                                      TOP LEFT                                    ***
    // ****************************************************************************************

    public void UpdateRakelVolumeReduceFunction(ReduceFunction value)
    {
        Config.TransferConfig.RakelVolumeReduceFunction = value;
    }

    public void UpdateCanvasVolumeReduceFunction(ReduceFunction value)
    {
        Config.TransferConfig.CanvasVolumeReduceFunction = value;
    }

    public void UpdateReadjustZToRakelVolume(bool value)
    {
        Config.TransferConfig.ReadjustZToRakelVolume = value;
    }

    public void UpdateReadjustZToCanvasVolume(bool value)
    {
        Config.TransferConfig.ReadjustZToCanvasVolume = value;
    }

    public void UpdateSmoothZLength(float value)
    {
        Config.TransferConfig.FloatingZLength = value;
    }

    public void UpdateRakelTiltNoiseEnabled(bool enabled)
    {
        Config.RakelConfig.TiltNoiseEnabled = enabled;
    }

    public void UpdateRakelTiltNoiseFrequency(float frequency)
    {
        Config.RakelConfig.TiltNoiseFrequency = frequency;
    }

    public void UpdateRakelTiltNoiseAmplitude(float amplitude)
    {
        Config.RakelConfig.TiltNoiseAmplitude = amplitude;
    }

    public void UpdateRakelLength(float worldSpaceLength)
    {
        Config.RakelConfig.Length = worldSpaceLength;
        CreateRakel();
    }

    public void UpdateRakelWidth(float worldSpaceWidth)
    {
        Config.RakelConfig.Width = worldSpaceWidth;
        CreateRakel();
    }

    public void UpdateCanvasFormatA(int formatA)
    {
        Config.CanvasConfig.FormatA = formatA;
        CreateCanvas();
    }

    public void UpdateCanvasFormatB(int formatB)
    {
        Config.CanvasConfig.FormatB = formatB;
        CreateCanvas();
    }

    public void UpdateTextureResolution(int pixelsPerWorldSpaceUnit)
    {
        Config.TextureResolution = pixelsPerWorldSpaceUnit;
        CreateCanvas();
        CreateRakel();
    }

    // ****************************************************************************************
    // ***                                     TOP RIGHT                                    ***
    // ****************************************************************************************

    public void UpdateFillColor(Color_ color)
    {
        Config.FillConfig.Color = color;
    }

    public void UpdateColorMode(ColorMode mode)
    {
        Config.FillConfig.ColorMode = mode;
    }

    public void UpdateFillWidthPart(float value)
    {
        Config.FillConfig.WidthPart = value;
    }

    public void UpdateFillVolume(int volume)
    {
        Config.FillConfig.Volume = volume;
    }

    public void UpdateVolumeMode(VolumeMode mode)
    {
        Config.FillConfig.VolumeMode = mode;
    }

    public void FillApply()
    {
        ClearRakel();

        FillConfiguration fillConfig = Config.FillConfig;

        ColorFiller colorFiller;
        if (fillConfig.ColorMode == ColorMode.Flat)
        {
            colorFiller = new FlatColorFiller(fillConfig.Color, Config.ColorSpace);
        } else
        {
            colorFiller = new GradientColorFiller(fillConfig.ColorMode, Config.ColorSpace);
        }

        VolumeFiller volumeFiller;
        if (fillConfig.VolumeMode == VolumeMode.Flat)
        {
            volumeFiller = new FlatVolumeFiller(fillConfig.WidthPart, fillConfig.Volume);
        } else
        {
            volumeFiller = new PerlinVolumeFiller(fillConfig.WidthPart, fillConfig.Volume);
        }

        ReservoirFiller filler = new ReservoirFiller(colorFiller, volumeFiller);

        Rakel.Fill(filler);
    }

    // ****************************************************************************************
    // ***                                    BOTTOM LEFT                                   ***
    // ****************************************************************************************

    public void ClearRakel()
    {
        CreateRakel();
    }

    public void ClearCanvas()
    {
        CreateCanvas();
    }

    // ****************************************************************************************
    // ***                                   BOTTOM RIGHT                                   ***
    // ****************************************************************************************

    public void UpdateCanvasSnapshotBufferEnabled(bool value)
    {
        Config.TransferConfig.CanvasSnapshotBufferEnabled = value;
    }

    public void UpdateDeletePickedUpFromCSB(bool value)
    {
        Config.TransferConfig.DeletePickedUpFromCSB = value;
    }

    public void UpdatePaintMode(PaintMode value)
    {
        PaintMode = value;
    }

    public void UpdatePaintDoesPickup(bool value)
    {
        Config.TransferConfig.PaintDoesPickup = value;
    }

    public void UpdateEmitDistance_MAX(float value)
    {
        Config.TransferConfig.EmitDistance_MAX = value;
    }

    public void UpdateEmitVolume_MIN(float value)
    {
        Config.TransferConfig.EmitVolume_MIN = value;
    }

    public void UpdatePickupDistance_MAX(float value)
    {
        Config.TransferConfig.PickupDistance_MAX = value;
    }

    public void UpdatePickupVolume_MIN(float value)
    {
        Config.TransferConfig.PickupVolume_MIN = value;
    }

    public void UpdateLayerThickness_MAX_Volume(float value)
    {
        Config.TransferConfig.LayerThickness_MAX_Volume = value;
    }

    public void UpdateTiltAdjustLayerThickness(bool value)
    {
        Config.TransferConfig.TiltAdjustLayerThickness = value;
    }

    public void UpdateBaseSink_MAX_Volume(float value)
    {
        Config.TransferConfig.BaseSink_MAX_Volume = value;
    }

    public void UpdateLayerSink_MAX_Ratio(float value)
    {
        Config.TransferConfig.LayerSink_MAX_Ratio = value;
    }

    public void UpdateTiltSink_MAX_Volume(float value)
    {
        Config.TransferConfig.TiltSink_MAX_Volume = value;
    }

    public void UpdateRakelCellVolume(float value)
    {
        Config.RakelConfig.CellVolume = value;
        CreateRakel();
    }

    public void UpdateRakelDiffuseDepth(int value)
    {
        Config.TransferConfig.RakelDiffuseDepth = value;
    }

    public void UpdateRakelDiffuseRatio(float value)
    {
        Config.TransferConfig.RakelDiffuseRatio = value;
    }

    public void UpdateCanvasCellVolume(float value)
    {
        Config.CanvasConfig.CellVolume = value;
        CreateCanvas();
    }

    public void UpdateCanvasDiffuseDepth(int value)
    {
        Config.TransferConfig.CanvasDiffuseDepth = value;
    }

    public void UpdateCanvasDiffuseRatio(float value)
    {
        Config.TransferConfig.CanvasDiffuseRatio = value;
    }

    public void UpdateNormalScale(float value)
    {
        Config.CanvasConfig.NormalScale = value;
        Canvas.NormalScale = value;
        Canvas.Render(Canvas.GetFullShaderRegion());
    }

    public void UpdateColorSpace(ColorSpace value)
    {
        Config.ColorSpace = value;
        CreateRakel();
        CreateCanvas();
    }

    public void DoMacroAction()
    {
        string filepath = System.Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar + "colors_" + System.DateTime.Now.Ticks + ".png";

        FileUtil.SaveTextureToFile(
            Canvas.Texture,
            filepath,
            Canvas.Texture.width,
            Canvas.Texture.height);
    }

    public void DoMacro2Action()
    {
        string filepath = System.Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar + "normals_" + System.DateTime.Now.Ticks + ".png";

        FileUtil.SaveTextureToFile(
                Canvas.NormalMap,
                filepath,
                Canvas.Texture.width,
                Canvas.Texture.height);
    }

    private void DoLineDown(float posX, Color_ color, bool autoBaseZEnabled, float height)
    {
        Rakel.Fill(new ReservoirFiller(new FlatColorFiller(color, Config.ColorSpace), new FlatVolumeFiller(1, 60)));
        TransferEngine.NewStroke(
            false,
            Config.RakelConfig.TiltNoiseFrequency,
            Config.RakelConfig.TiltNoiseAmplitude,
            Config.TransferConfig.FloatingZLength,
            Config.TransferConfig.CanvasSnapshotBufferEnabled);
        InputInterpolator.AddNode(
            new InputState(
                new Vector3(posX, 5, -height), autoBaseZEnabled, 0,
                90,
                0),
            true,
            Config.TextureResolution);
        InputInterpolator.AddNode(
            new InputState(
                new Vector3(posX, -5, -height), autoBaseZEnabled, 0,
                90,
                0),
            false,
            Config.TextureResolution);
    }

    private void DoThreeLinesDown(float posX, float lineWidth, Color_ color1, Color_ color2, Color_ color3)
    {
        bool AUTO_BASE_Z_ENABLED = false;
        float HEIGHT = 4 * Paint.VOLUME_THICKNESS;

        ClearRakel();
        DoLineDown(posX - lineWidth, color1, AUTO_BASE_Z_ENABLED, HEIGHT);
        ClearRakel();
        DoLineDown(posX, color2, AUTO_BASE_Z_ENABLED, HEIGHT);
        ClearRakel();
        DoLineDown(posX + lineWidth, color3, AUTO_BASE_Z_ENABLED, HEIGHT);
    }

    float MATRIX_INIT_RAKEL_LENGTH = 0.5f;
    float MATRIX_SWIPE_RAKEL_WIDTH = 0.5f;

    public void DoMacro3Action()
    {
        STEPS_PER_FRAME = -1;
        Config.TextureResolution = 70;
        Start();

        Config.TransferConfig.CanvasSnapshotBufferEnabled = false;
        UpdateRakelLength(MATRIX_INIT_RAKEL_LENGTH);

        //Color_ COLOR_1 = Color_.UltramarineBlue;
        //Color_ COLOR_2 = Color_.TitanWhite;
        //Color_ COLOR_3 = Color_.LavenderLight;
        //Color_ COLOR_1 = Color_.CadmiumYellow;
        //Color_ COLOR_2 = Color_.CadmiumGreenLight;
        //Color_ COLOR_3 = Color_.LavenderLight;
        Color_ COLOR_1 = Color_.CadmiumYellow;
        Color_ COLOR_2 = Color_.CadmiumRed;
        Color_ COLOR_3 = Color_.CadmiumLightGreen;

        float x1 = -7.5f + MATRIX_SWIPE_RAKEL_WIDTH + 1.5f * MATRIX_INIT_RAKEL_LENGTH;
        float x2 = -2.5f + MATRIX_SWIPE_RAKEL_WIDTH + 1.5f * MATRIX_INIT_RAKEL_LENGTH;
        float x3 =  2.5f + MATRIX_SWIPE_RAKEL_WIDTH + 1.5f * MATRIX_INIT_RAKEL_LENGTH;
        DoThreeLinesDown(x1, MATRIX_INIT_RAKEL_LENGTH, COLOR_1, COLOR_2, COLOR_3);
        DoThreeLinesDown(x2, MATRIX_INIT_RAKEL_LENGTH, COLOR_1, COLOR_2, COLOR_3);
        DoThreeLinesDown(x3, MATRIX_INIT_RAKEL_LENGTH, COLOR_1, COLOR_2, COLOR_3);


        // setup default config for macro 4
        // by doing it here, you can also change it before executing macro 4
        InputManager.UpdateUsingScriptPositionBaseZ(false); // auto z enabled

        InputManager.UpdateUsingScriptPressure(true);
        Control.Pressure = 0.5f;

        Control.Tilt = 0;
    }

    private void DoSwipeRight(float posX0, float posX1, float posY)
    {
        TransferEngine.NewStroke(
            Config.RakelConfig.TiltNoiseEnabled,
            Config.RakelConfig.TiltNoiseFrequency,
            Config.RakelConfig.TiltNoiseAmplitude,
            Config.TransferConfig.FloatingZLength,
            Config.TransferConfig.CanvasSnapshotBufferEnabled);
        InputInterpolator.AddNode(
            new InputState(
                new Vector3(posX0, posY, InputManager.PositionBaseZ), InputManager.PositionAutoBaseZEnabled, InputManager.Pressure,
                InputManager.Rotation,
                InputManager.Tilt),
            true,
            Config.TextureResolution);
        InputInterpolator.AddNode(
            new InputState(
                new Vector3(posX1, posY, InputManager.PositionBaseZ), InputManager.PositionAutoBaseZEnabled, InputManager.Pressure,
                InputManager.Rotation,
                InputManager.Tilt),
            false,
            Config.TextureResolution);
    }

    private void DoSwipesRight(int swipes, float posX0, float posX1, Color_ color, float volume)
    {
        float GAP = 0.25f;
        float MATRIX_SWIPE_RAKEL_LENGTH = (10 - GAP) / swipes - GAP;

        UpdateRakelLength(MATRIX_SWIPE_RAKEL_LENGTH);
        UpdateRakelTiltNoiseEnabled(false);
        Control.PositionBaseZ = -3 * Paint.VOLUME_THICKNESS;

        float paramBegin = 0;
        float paramEnd = 1;
        float paramStep = (paramEnd - paramBegin) / (swipes - 1);

        float yBegin = 5 - GAP - MATRIX_SWIPE_RAKEL_LENGTH / 2;
        float yEnd = -5 + GAP + MATRIX_SWIPE_RAKEL_LENGTH / 2;
        float yStep = (yEnd - yBegin) / (swipes - 1);

        float paramCurrent = paramBegin;
        float yCurrent = yBegin;
        for (int i = 0; i < swipes; i++)
        {
            ClearRakel();
            Rakel.Fill(new ReservoirFiller(new FlatColorFiller(color, ColorSpace.RGB), new FlatVolumeFiller(1, volume)));
            Control.Pressure = paramCurrent;
            DoSwipeRight(posX0, posX1, yCurrent);

            paramCurrent += paramStep;
            yCurrent += yStep;
        }
    }

    private void DoSwipesRight(int swipes, float posX0, float posX1)
    {
        DoSwipesRight(swipes, posX0, posX1, Color_.TitanWhite, 0);
    }

    public void DoMacro4Action()
    {
        float x1 = -7.5f + MATRIX_SWIPE_RAKEL_WIDTH + 1.5f * MATRIX_INIT_RAKEL_LENGTH;
        float x2 = -2.5f + MATRIX_SWIPE_RAKEL_WIDTH + 1.5f * MATRIX_INIT_RAKEL_LENGTH;
        float x3 = 2.5f + MATRIX_SWIPE_RAKEL_WIDTH + 1.5f * MATRIX_INIT_RAKEL_LENGTH;

        UpdateRakelWidth(MATRIX_SWIPE_RAKEL_WIDTH);
        float SWIPE_GAP = 0.25f;
        float x1_0 = x1 - 1.5f * MATRIX_INIT_RAKEL_LENGTH - MATRIX_SWIPE_RAKEL_WIDTH;
        float x1_1 = -2.5f - MATRIX_SWIPE_RAKEL_WIDTH - SWIPE_GAP;
        float x2_0 = x2 - 1.5f * MATRIX_INIT_RAKEL_LENGTH - MATRIX_SWIPE_RAKEL_WIDTH;
        float x2_1 = 2.5f - MATRIX_SWIPE_RAKEL_WIDTH - SWIPE_GAP;
        float x3_0 = x3 - 1.5f * MATRIX_INIT_RAKEL_LENGTH - MATRIX_SWIPE_RAKEL_WIDTH;
        float x3_1 = 7.5f - MATRIX_SWIPE_RAKEL_WIDTH - SWIPE_GAP;

        int SWIPES = 6;

        Config.TransferConfig.CanvasSnapshotBufferEnabled = false;
        Config.TransferConfig.DeletePickedUpFromCSB = false;
        DoSwipesRight(SWIPES, x1_0, x1_1);
        Config.TransferConfig.CanvasSnapshotBufferEnabled = true;
        Config.TransferConfig.DeletePickedUpFromCSB = true;
        DoSwipesRight(SWIPES, x2_0, x2_1);
        Config.TransferConfig.CanvasSnapshotBufferEnabled = true;
        Config.TransferConfig.DeletePickedUpFromCSB = false;
        DoSwipesRight(SWIPES, x3_0, x3_1);
    }

    public void DoMacro5Action()
    {
        STEPS_PER_FRAME = -1;
        Config.TextureResolution = 70;
        Start();

        Config.TransferConfig.CanvasSnapshotBufferEnabled = false;
        UpdateRakelLength(MATRIX_INIT_RAKEL_LENGTH);

        Color_ COLOR = Color_.CadmiumYellow;
        bool AUTO_BASE_Z_ENABLED = false;
        float HEIGHT = 4 * Paint.VOLUME_THICKNESS;

        float x1 = -7.5f + MATRIX_SWIPE_RAKEL_WIDTH + 1.5f * MATRIX_INIT_RAKEL_LENGTH;
        float x2 = -2.5f + MATRIX_SWIPE_RAKEL_WIDTH + 1.5f * MATRIX_INIT_RAKEL_LENGTH;
        float x3 = 2.5f + MATRIX_SWIPE_RAKEL_WIDTH + 1.5f * MATRIX_INIT_RAKEL_LENGTH;
        ClearRakel();
        DoLineDown(x1, COLOR, AUTO_BASE_Z_ENABLED, HEIGHT);
        ClearRakel();
        DoLineDown(x2, COLOR, AUTO_BASE_Z_ENABLED, HEIGHT);
        ClearRakel();
        DoLineDown(x3, COLOR, AUTO_BASE_Z_ENABLED, HEIGHT);


        // setup default config for macro 6
        // by doing it here, you can also change it before executing macro 6
        InputManager.UpdateUsingScriptPositionBaseZ(false); // auto z enabled

        InputManager.UpdateUsingScriptPressure(true);
        Control.Pressure = 0.5f;

        Control.Tilt = 0;
    }

    public void DoMacro6Action()
    {
        float x1 = -7.5f + MATRIX_SWIPE_RAKEL_WIDTH + 1.5f * MATRIX_INIT_RAKEL_LENGTH;
        float x2 = -2.5f + MATRIX_SWIPE_RAKEL_WIDTH + 1.5f * MATRIX_INIT_RAKEL_LENGTH;
        float x3 = 2.5f + MATRIX_SWIPE_RAKEL_WIDTH + 1.5f * MATRIX_INIT_RAKEL_LENGTH;

        UpdateRakelWidth(MATRIX_SWIPE_RAKEL_WIDTH);
        float SWIPE_GAP = 0.25f;
        float x1_0 = x1 - 1.5f * MATRIX_INIT_RAKEL_LENGTH - MATRIX_SWIPE_RAKEL_WIDTH;
        float x1_1 = -2.5f - MATRIX_SWIPE_RAKEL_WIDTH - SWIPE_GAP;
        float x2_0 = x2 - 1.5f * MATRIX_INIT_RAKEL_LENGTH - MATRIX_SWIPE_RAKEL_WIDTH;
        float x2_1 = 2.5f - MATRIX_SWIPE_RAKEL_WIDTH - SWIPE_GAP;
        float x3_0 = x3 - 1.5f * MATRIX_INIT_RAKEL_LENGTH - MATRIX_SWIPE_RAKEL_WIDTH;
        float x3_1 = 7.5f - MATRIX_SWIPE_RAKEL_WIDTH - SWIPE_GAP;

        int SWIPES = 6;
        Color_ COLOR = Color_.DarkRed;
        float VOLUME = 60;

        Config.TransferConfig.CanvasSnapshotBufferEnabled = false;
        Config.TransferConfig.DeletePickedUpFromCSB = false;
        DoSwipesRight(SWIPES, x1_0, x1_1, COLOR, VOLUME);
        Config.TransferConfig.CanvasSnapshotBufferEnabled = true;
        Config.TransferConfig.DeletePickedUpFromCSB = true;
        DoSwipesRight(SWIPES, x2_0, x2_1, COLOR, VOLUME);
        Config.TransferConfig.CanvasSnapshotBufferEnabled = true;
        Config.TransferConfig.DeletePickedUpFromCSB = false;
        DoSwipesRight(SWIPES, x3_0, x3_1, COLOR, VOLUME);
    }

    private void DoBenchmark(Vector3 beginPosition, Vector3 endPosition, float rotation, float tilt)
    {
        // Parameters adjusted for windowed unity
        STEPS_PER_FRAME = 400;

        int LAYERS = 4;
        bool AUTO_BASE_Z_ENABLED = false;
        bool PICKUP_BENCHMARK = false;

        Config.TextureResolution = 70;
        Config.RakelConfig.Length = 10;
        Config.RakelConfig.Width = 1;
        Config.RakelConfig.TiltNoiseEnabled = false;
        Config.TransferConfig.LayerThickness_MAX_Volume = LAYERS;
        Start();

        if (PICKUP_BENCHMARK)
        {
            Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumLightGreen, Config.ColorSpace), new FlatVolumeFiller(1, 4)));
            Canvas.Render(Canvas.Reservoir.GetFullShaderRegion());

            AUTO_BASE_Z_ENABLED = false;
            beginPosition.z = 0;
            endPosition.z = 0;
        }
        else // EMIT BENCHMARK with option of no auto z
        {
            // NOTE that this only hits when AUTO_BASE_Z_ENABLED is false
            beginPosition.z = -LAYERS * Paint.VOLUME_THICKNESS;
            endPosition.z = -LAYERS * Paint.VOLUME_THICKNESS;

            UpdateFillColor(Color_.CadmiumLightGreen);
            UpdateFillVolume(120);
            UpdateFillWidthPart(1);
            FillApply();
        }

        int RUNS = 10;
        for (int i = 0; i < RUNS; i++)
        {
            TransferEngine.NewStroke(
                Config.RakelConfig.TiltNoiseEnabled,
                Config.RakelConfig.TiltNoiseFrequency,
                Config.RakelConfig.TiltNoiseAmplitude,
                Config.TransferConfig.FloatingZLength,
                Config.TransferConfig.CanvasSnapshotBufferEnabled);

            InputInterpolator.AddNode(
                new InputState(
                    beginPosition,
                    AUTO_BASE_Z_ENABLED,
                    0,
                    rotation,
                    tilt),
                true,
                Config.TextureResolution);

            InputInterpolator.AddNode(
                new InputState(
                    endPosition,
                    AUTO_BASE_Z_ENABLED,
                    0,
                    rotation,
                    tilt),
                false,
                Config.TextureResolution);
        }
    }

    public void DoMacro7Action()
    {
        // straight
        Vector3 startPosition = new Vector3(-7, 0, 0);
        Vector3 endPosition = new Vector3(7, 0, 0);
        float rotation = 0;
        float tilt = 0;
        //float tilt = 60;
        //float tilt = 79;
        DoBenchmark(startPosition, endPosition, rotation, tilt);
    }

    public void DoMacro8Action()
    {
        // rotated diagonal
        Vector3 startPosition = new Vector3(-4, 4, 0);
        Vector3 endPosition = new Vector3(4, -4, 0);
        float rotation = 45;
        float tilt = 0;
        DoBenchmark(startPosition, endPosition, rotation, tilt);
    }
}