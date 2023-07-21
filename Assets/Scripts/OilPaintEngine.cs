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

    public int LAYERS_MAX;
    public int STEPS_PER_FRAME;

    private bool UsePen;
    private bool PenConfigLoaded;

    public Configuration Config { get; private set; }
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
            1, // convert world space to local scale
            Config.CanvasConfig.Height / 10);
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
            Config.CanvasConfig.Width, Config.CanvasConfig.Height, LAYERS_MAX,
            Config.CanvasConfig.CellVolume, Config.CanvasConfig.DiffuseDepth, Config.CanvasConfig.DiffuseRatio,
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
            Config.RakelConfig.Length, Config.RakelConfig.Width, Config.TextureResolution, LAYERS_MAX,
            Config.RakelConfig.CellVolume, Config.RakelConfig.DiffuseDepth, Config.RakelConfig.DiffuseRatio,
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

            if (InputManager.DrawingEnabled)
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

    public void UpdateReadjustZToRakelVolume(bool value)
    {
        Config.TransferConfig.ReadjustZToRakelVolume = value;
    }

    public void UpdateReadjustZToCanvasVolume(bool value)
    {
        Config.TransferConfig.ReadjustZToCanvasVolume = value;
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

    public void UpdateLayerThickness_MAX(float value)
    {
        Config.TransferConfig.LayerThickness_MAX = value;
    }

    public void UpdateNormalScale(float value)
    {
        Config.CanvasConfig.NormalScale = value;
        Canvas.NormalScale = value;
        Canvas.Render(Canvas.GetFullShaderRegion());
    }

    public void DoMacroAction()
    {
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreenLight, Config.ColorSpace), new FlatVolumeFiller(1, 4)));
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

    public void DoMacro3Action()
    {
    }

    public void DoMacro4Action()
    {
    }

    private void DoBenchmark(Vector3 beginPosition, Vector3 endPosition, float rotation, float tilt)
    {
        // Parameters adjusted for windowed unity
        STEPS_PER_FRAME = 200;
        LAYERS_MAX = 50;

        int LAYERS = 4;
        bool AUTO_Z_ENABLED = false;
        bool PICKUP_BENCHMARK = false;

        Config.TextureResolution = 40;
        Config.RakelConfig.Length = 4;
        Config.RakelConfig.Width = 1;
        Config.RakelConfig.TiltNoiseEnabled = false;
        Config.TransferConfig.LayerThickness_MAX = LAYERS * Paint.VOLUME_THICKNESS;
        Start();

        if (PICKUP_BENCHMARK)
        {
            Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreenLight, Config.ColorSpace), new FlatVolumeFiller(1, 4)));
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

            UpdateFillColor(Color_.CadmiumGreenLight);
            UpdateFillVolume(100);
            UpdateFillWidthPart(1);
            FillApply();
        }

        int RUNS = 10;
        for (int i=0; i < RUNS; i++)
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

    public void DoMacro5Action()
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

    public void DoMacro6Action()
    {
        // rotated diagonal
        Vector3 startPosition = new Vector3(-4, 4, 0);
        Vector3 endPosition = new Vector3(4, -4, 0);
        float rotation = 45;
        float tilt = 0;
        DoBenchmark(startPosition, endPosition, rotation, tilt);
    }

    public void DoMacro7Action()
    {
    }

    public void DoMacro8Action()
    {
    }
}