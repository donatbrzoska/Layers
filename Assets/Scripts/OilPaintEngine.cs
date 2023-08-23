using UnityEngine;
using UnityEngine.InputSystem;

public class OilPaintEngine : MonoBehaviour
{
    // These attributes are set through inspector
    public int BENCHMARK_STEPS;
    public int TH_GROUP_SIZE_X;
    public int TH_GROUP_SIZE_Y;

    public float ANCHOR_RATIO_X;
    public float ANCHOR_RATIO_Y;

    public int CANVAS_LAYERS_MAX;
    public int RAKEL_LAYERS_MAX;
    public int STEPS_PER_FRAME;

    public bool EVALUATE;

    private bool UsePen;
    private bool PenConfigLoaded;

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

    public InputManager InputManager { get; private set; }

    public TransferEngine TransferEngine;
    private Canvas_ Canvas;
    public Rakel Rakel;
    private InputInterpolator InputInterpolator;

    void Awake()
    {
        Config = new Configuration();

        CreateInputManager();

        if (BENCHMARK_STEPS > 0)
        {
            Config.LoadBenchmark();
        }

        if (EVALUATE)
        {
            Config.TextureResolution = 70;

            GameObject.Find("CSB Toggle").SetActive(false);
            GameObject.Find("CSB Delete Toggle").SetActive(false);
        }
        else
        {
            GameObject.Find("Paint Mode Text").SetActive(false);
        }
    }

    void CreateInputManager()
    {
        InputManager = new InputManager(Config.InputConfig);
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
        TransferEngine = new TransferEngine(STEPS_PER_FRAME > 0 && BENCHMARK_STEPS == 0);
        InputInterpolator.SetTransferEngine(TransferEngine);
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
            RAKEL_LAYERS_MAX, Config.RakelConfig.CellVolume,
            ANCHOR_RATIO_Y, ANCHOR_RATIO_X);

        Debug.Log("Rakel is "
                  + Config.RakelConfig.Length * Config.TextureResolution + "x" + Config.RakelConfig.Width * Config.TextureResolution
                  + " = " + Config.RakelConfig.Length * Config.TextureResolution * Config.RakelConfig.Width * Config.TextureResolution);

        TransferEngine.SetRakel(Rakel);
        InputInterpolator.SetRakel(Rakel);
    }

    void Update()
    {
        if (Pen.current.IsActuated() && !PenConfigLoaded)
        {
            Config.InputConfig.RakelPressure.Source = InputSourceType.Pen;
            Config.InputConfig.RakelPositionX.Source = InputSourceType.Pen;
            Config.InputConfig.RakelPositionY.Source = InputSourceType.Pen;
            if (Config.InputConfig.RakelRotation.Source == InputSourceType.Mouse)
            {
                Config.InputConfig.RakelRotation.Source = InputSourceType.Pen;
            }
            Config.InputConfig.StrokeStateSource = InputSourceType.Pen;

            CreateInputManager();

            UsePen = true;
            PenConfigLoaded = true;
        }

        if (BENCHMARK_STEPS > 0){
            for (int i = 0; i < BENCHMARK_STEPS; i++){
                float x = Random.Range(-5f, 5f);
                float y = Random.Range(-3f, 3f);
                TransferEngine.SimulateStep(
                    new Vector3(x, y, 0),
                    false,
                    0,
                    0,
                    0,
                    Config.TransferConfig);
            }
        }
        else
        {
            InputManager.Update();

            if (InputManager.DrawingPossible)
            {
                if (InputManager.StrokeBegin)
                {
                    InputInterpolator.NewStroke(
                        Config.RakelConfig.TiltNoiseEnabled,
                        Config.RakelConfig.TiltNoiseFrequency,
                        Config.RakelConfig.TiltNoiseAmplitude,
                        Config.TransferConfig.FloatingZLength,
                        Config.TransferConfig.CanvasSnapshotBufferEnabled);
                }

                if (InputManager.InStroke)
                {
                    Vector3 position = new Vector3(InputManager.RakelPositionX, InputManager.RakelPositionY, InputManager.RakelPositionZ);
                    bool autoZEnabled = Config.InputConfig.RakelPositionZ.Source != InputSourceType.Text ? true : false;
                    float pressure = InputManager.RakelPressure;
                    float rotation = InputManager.RakelRotation;
                    float tilt = InputManager.RakelTilt;

                    InputInterpolator.AddNode(
                        position,
                        autoZEnabled,
                        pressure,
                        rotation,
                        tilt,
                        Config.TransferConfig,
                        Config.TextureResolution);
                }
            }
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

    public bool RakelPositionXLocked { get { return Config.InputConfig.RakelPositionX.Source == InputSourceType.Text; } }

    public bool RakelPositionYLocked { get { return Config.InputConfig.RakelPositionY.Source == InputSourceType.Text; } }

    public bool RakelPositionZLocked { get { return Config.InputConfig.RakelPositionZ.Source == InputSourceType.Text; } }

    public bool RakelPressureLocked { get { return Config.InputConfig.RakelPressure.Source == InputSourceType.Text; } }

    public bool RakelRotationLocked { get { return Config.InputConfig.RakelRotation.Source == InputSourceType.Text; } }

    public bool RakelTiltLocked { get { return Config.InputConfig.RakelTilt.Source == InputSourceType.Text; } }

    public void UpdateRakelPositionX(float value)
    {
        Config.InputConfig.RakelPositionX.Value = value;
        CreateInputManager();
    }

    public void UpdateRakelPositionXLocked(bool locked)
    {
        InputSourceType penOrMouse = UsePen ? InputSourceType.Pen : InputSourceType.Mouse;
        Config.InputConfig.RakelPositionX.Source = locked ? InputSourceType.Text : penOrMouse;
        CreateInputManager();
    }

    public void UpdateRakelPositionY(float value)
    {
        Config.InputConfig.RakelPositionY.Value = value;
        CreateInputManager();
    }

    public void UpdateRakelPositionYLocked(bool locked)
    {
        InputSourceType penOrMouse = UsePen ? InputSourceType.Pen : InputSourceType.Mouse;
        Config.InputConfig.RakelPositionY.Source = locked ? InputSourceType.Text : penOrMouse;
        CreateInputManager();
    }

    public void UpdateRakelPositionZ(float value)
    {
        Config.InputConfig.RakelPositionZ.Value = value;
        CreateInputManager();
    }

    public void UpdateRakelPositionZLocked(bool locked)
    {
        Config.InputConfig.RakelPositionZ.Source = locked ? InputSourceType.Text : InputSourceType.Auto;
        CreateInputManager();
    }

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

    public void UpdateRakelPressure(float value)
    {
        Config.InputConfig.RakelPressure.Value = value;
        CreateInputManager();
    }

    public void UpdateRakelPressureLocked(bool locked)
    {
        InputSourceType penOrKeyboard = UsePen ? InputSourceType.Pen : InputSourceType.Keyboard;
        Config.InputConfig.RakelPressure.Source = locked ? InputSourceType.Text : penOrKeyboard;
        CreateInputManager();
    }

    public void UpdateRakelRotation(float rotation)
    {
        Config.InputConfig.RakelRotation.Value = rotation;
        CreateInputManager();
    }

    public void UpdateRakelRotationLocked(bool locked)
    {
        InputSourceType penOrMouse = UsePen ? InputSourceType.Pen : InputSourceType.Mouse;
        Config.InputConfig.RakelRotation.Source = locked ? InputSourceType.Text : penOrMouse;
        CreateInputManager();
    }

    public void UpdateRakelTilt(float tilt)
    {
        Config.InputConfig.RakelTilt.Value = tilt;
        CreateInputManager();
    }

    public void UpdateRakelTiltLocked(bool locked)
    {
        Config.InputConfig.RakelTilt.Source = locked ? InputSourceType.Text : InputSourceType.Keyboard;
        CreateInputManager();
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
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumLightGreen, Config.ColorSpace), new FlatVolumeFiller(1, 4)));
        Canvas.Render(Canvas.Reservoir.GetFullShaderRegion());

        //int PRINTED_DEPTH = 1;
        //Canvas.Reservoir.PaintGrid.ReadbackContent();
        //LogUtil.Log(Canvas.Reservoir.PaintGrid.GetColors(), Canvas.Reservoir.Size, PRINTED_DEPTH, DebugListType.Float4, "Colors");
    }

    public void DoMacro2Action()
    {
        UpdateRakelLength(8);
        Rakel.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumYellow, Config.ColorSpace), new PerlinVolumeFiller(1, 10)));

        InputInterpolator.NewStroke(Config.RakelConfig.TiltNoiseEnabled, Config.RakelConfig.TiltNoiseFrequency, Config.RakelConfig.TiltNoiseAmplitude, Config.TransferConfig.FloatingZLength, Config.TransferConfig.CanvasSnapshotBufferEnabled);
        InputInterpolator.AddNode(
            new Vector3(-3, 0, -0.10f), true, 0.5f,
            0,
            6,
            Config.TransferConfig,
            Config.TextureResolution);
        InputInterpolator.AddNode(
            new Vector3(3, 0, -0.10f), true, 0.5f,
            0,
            6,
            Config.TransferConfig,
            Config.TextureResolution);

        //Canvas.Reservoir.PaintGrid.ReadbackInfo();
        //LogUtil.Log(Canvas.Reservoir.PaintGrid.GetVolumes(), Canvas.Reservoir.Size.y);
    }

    private void DoLineDown(float posX, Color_ color, bool autoZEnabled, float height)
    {
        Rakel.Fill(new ReservoirFiller(new FlatColorFiller(color, Config.ColorSpace), new FlatVolumeFiller(1, 60)));
        InputInterpolator.NewStroke(
            false,
            Config.RakelConfig.TiltNoiseFrequency,
            Config.RakelConfig.TiltNoiseAmplitude,
            Config.TransferConfig.FloatingZLength,
            Config.TransferConfig.CanvasSnapshotBufferEnabled);
        InputInterpolator.AddNode(
            new Vector3(posX, 5, -height), autoZEnabled, 0,
            90,
            0,
            Config.TransferConfig,
            Config.TextureResolution);
        InputInterpolator.AddNode(
            new Vector3(posX, -5, -height), autoZEnabled, 0,
            90,
            0,
            Config.TransferConfig,
            Config.TextureResolution);
    }

    private void DoThreeLinesDown(float posX, float lineWidth, Color_ color1, Color_ color2, Color_ color3)
    {
        bool AUTO_Z_ENABLED = false;
        float HEIGHT = 4 * Paint.VOLUME_THICKNESS;

        ClearRakel();
        DoLineDown(posX - lineWidth, color1, AUTO_Z_ENABLED, HEIGHT);
        ClearRakel();
        DoLineDown(posX, color2, AUTO_Z_ENABLED, HEIGHT);
        ClearRakel();
        DoLineDown(posX + lineWidth, color3, AUTO_Z_ENABLED, HEIGHT);
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
        UpdateRakelPositionZLocked(false); // auto z enabled

        UpdateRakelPressureLocked(true);
        UpdateRakelPressure(0.5f);

        UpdateRakelTilt(0);
    }

    private void DoSwipeRight(float posX0, float posX1, float posY)
    {
        bool autoZEnabled = Config.InputConfig.RakelPositionZ.Source == InputSourceType.Auto;
        
        InputInterpolator.NewStroke(
            Config.RakelConfig.TiltNoiseEnabled,
            Config.RakelConfig.TiltNoiseFrequency,
            Config.RakelConfig.TiltNoiseAmplitude,
            Config.TransferConfig.FloatingZLength,
            Config.TransferConfig.CanvasSnapshotBufferEnabled);
        InputInterpolator.AddNode(
            new Vector3(posX0, posY, InputManager.RakelPositionZ), autoZEnabled, InputManager.RakelPressure,
            InputManager.RakelRotation,
            InputManager.RakelTilt,
            Config.TransferConfig,
            Config.TextureResolution);
        InputInterpolator.AddNode(
            new Vector3(posX1, posY, InputManager.RakelPositionZ), autoZEnabled, InputManager.RakelPressure,
            InputManager.RakelRotation,
            InputManager.RakelTilt,
            Config.TransferConfig,
            Config.TextureResolution);
    }

    private void DoSwipesRight(int swipes, float posX0, float posX1, Color_ color, float volume)
    {
        float GAP = 0.25f;
        float MATRIX_SWIPE_RAKEL_LENGTH = (10 - GAP) / swipes - GAP;

        UpdateRakelLength(MATRIX_SWIPE_RAKEL_LENGTH);
        UpdateRakelTiltNoiseEnabled(false);
        UpdateRakelPositionZ(-3 * Paint.VOLUME_THICKNESS);

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
            UpdateRakelPressure(paramCurrent);
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
        bool AUTO_Z_ENABLED = false;
        float HEIGHT = 4 * Paint.VOLUME_THICKNESS;

        float x1 = -7.5f + MATRIX_SWIPE_RAKEL_WIDTH + 1.5f * MATRIX_INIT_RAKEL_LENGTH;
        float x2 = -2.5f + MATRIX_SWIPE_RAKEL_WIDTH + 1.5f * MATRIX_INIT_RAKEL_LENGTH;
        float x3 = 2.5f + MATRIX_SWIPE_RAKEL_WIDTH + 1.5f * MATRIX_INIT_RAKEL_LENGTH;
        ClearRakel();
        DoLineDown(x1, COLOR, AUTO_Z_ENABLED, HEIGHT);
        ClearRakel();
        DoLineDown(x2, COLOR, AUTO_Z_ENABLED, HEIGHT);
        ClearRakel();
        DoLineDown(x3, COLOR, AUTO_Z_ENABLED, HEIGHT);


        // setup default config for macro 6
        // by doing it here, you can also change it before executing macro 6
        UpdateRakelPositionZLocked(false); // auto z enabled

        UpdateRakelPressureLocked(true);
        UpdateRakelPressure(0.5f);

        UpdateRakelTilt(0);
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
        bool AUTO_Z_ENABLED = false;
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

            AUTO_Z_ENABLED = false;
            beginPosition.z = 0;
            endPosition.z = 0;
        }
        else // EMIT BENCHMARK with option of no auto z
        {
            // NOTE that this only hits when AUTO_Z_ENABLED is false
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
            InputInterpolator.NewStroke(
                Config.RakelConfig.TiltNoiseEnabled,
                Config.RakelConfig.TiltNoiseFrequency,
                Config.RakelConfig.TiltNoiseAmplitude,
                Config.TransferConfig.FloatingZLength,
                Config.TransferConfig.CanvasSnapshotBufferEnabled);

            InputInterpolator.AddNode(
                beginPosition,
                AUTO_Z_ENABLED,
                0,
                rotation,
                tilt,
                Config.TransferConfig,
                Config.TextureResolution);

            InputInterpolator.AddNode(
                endPosition,
                AUTO_Z_ENABLED,
                0,
                rotation,
                tilt,
                Config.TransferConfig,
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