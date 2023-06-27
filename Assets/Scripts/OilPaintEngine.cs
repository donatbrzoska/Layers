using UnityEngine;

public class OilPaintEngine : MonoBehaviour
{
    // These attributes are set through inspector
    public bool DEBUG_SHADER;
    public int BENCHMARK_STEPS;
    public int TH_GROUP_SIZE_X;
    public int TH_GROUP_SIZE_Y;

    public float ANCHOR_RATIO_X;
    public float ANCHOR_RATIO_Y;

    public bool USE_PEN;

    private int LAYERS_MAX = 20;

    public Configuration Configuration { get; private set; }
    public InputManager InputManager { get; private set; }

    private TransferEngine TransferEngine;
    private Canvas_ Canvas;
    public Rakel Rakel;
    private InputInterpolator InputInterpolator;

    void Awake()
    {
        Configuration = new Configuration();

        //Configuration.LoadDebug();
        //Configuration.RakelRotation = 22;
        //Configuration.RakelTilt = 20;
        //Configuration.FillConfiguration.VolumeMode = VolumeMode.Flat;
        //Configuration.FillConfiguration.Volume = 30;

        if (USE_PEN)
        {
            Configuration.InputConfiguration.RakelPressure.Source = InputSourceType.Pen;
        }

        CreateInputManager();

        if (BENCHMARK_STEPS > 0)
        {
            Configuration.LoadBenchmark();
        }
    }

    void CreateInputManager()
    {
        InputManager = new InputManager(Configuration.InputConfiguration);
    }

    void Start()
    {
        ComputeShaderTask.ThreadGroupSize = new Vector2Int(TH_GROUP_SIZE_X, TH_GROUP_SIZE_Y);

        CreateCanvas();
        CreateRakel();
        CreateOilPaintTransferEngine();
        CreateInputInterpolator();
    }

    void CreateCanvas()
    {
        DisposeCanvas();

        float width = GameObject.Find("Canvas").GetComponent<Transform>().localScale.x * 10; // convert scale attribute to world space
        float height = GameObject.Find("Canvas").GetComponent<Transform>().localScale.y * 10; // convert scale attribute to world space
        Vector3 position = GameObject.Find("Canvas").GetComponent<Transform>().position;
        Canvas = new Canvas_(width, height, LAYERS_MAX, position, Configuration.TextureResolution, Configuration.NormalScale, Configuration.ColorSpace);

        Renderer renderer = GameObject.Find("Canvas").GetComponent<Renderer>();
        renderer.material.SetTexture("_MainTex", Canvas.Texture);
        renderer.material.EnableKeyword("_NORMALMAP");
        renderer.material.SetTexture("_BumpMap", Canvas.NormalMap);

        // Trigger lighting update
        Canvas.Render(Canvas.GetFullShaderRegion());

        Debug.Log("Texture is "
                  + Canvas.Texture.width + "x" + Canvas.Texture.height
                  + " = " + Canvas.Texture.width * Canvas.Texture.height);
    }

    void CreateRakel()
    {
        DisposeRakel();

        Rakel = new Rakel(
            Configuration.RakelConfiguration.Length,
            Configuration.RakelConfiguration.Width,
            Configuration.TextureResolution,
            LAYERS_MAX,
            ANCHOR_RATIO_Y,
            ANCHOR_RATIO_X);

        Debug.Log("Rakel is "
                  + Configuration.RakelConfiguration.Length * Configuration.TextureResolution + "x" + Configuration.RakelConfiguration.Width * Configuration.TextureResolution
                  + " = " + Configuration.RakelConfiguration.Length * Configuration.TextureResolution * Configuration.RakelConfiguration.Width * Configuration.TextureResolution);
    }

    void CreateOilPaintTransferEngine()
    {
        TransferEngine = new TransferEngine(DEBUG_SHADER);
    }

    void CreateInputInterpolator()
    {
        InputInterpolator = new InputInterpolator(TransferEngine, Rakel, Canvas);
    }

    void Update()
    {
        if (BENCHMARK_STEPS > 0){
            for (int i = 0; i < BENCHMARK_STEPS; i++){
                float x = Random.Range(-5f, 5f);
                float y = Random.Range(-3f, 3f);
                TransferEngine.SimulateStep(
                    new Vector3(x, y, 0),
                    0,
                    0,
                    0,
                    Configuration.TransferConfiguration,
                    Rakel,
                    Canvas);
            }
        }
        else
        {
            InputManager.Update();

            if (InputManager.DrawingEnabled)
            {
                if (InputManager.StrokeBegin)
                {
                    InputInterpolator.NewStroke();
                }

                if (InputManager.InStroke)
                {
                    Vector3 position = new Vector3(InputManager.RakelPositionX, InputManager.RakelPositionY, InputManager.RakelPositionZ);
                    float pressure = InputManager.RakelPressure;
                    float rotation = InputManager.RakelRotation;
                    float tilt = InputManager.RakelTilt;

                    InputInterpolator.AddNode(
                        position,
                        pressure,
                        rotation,
                        tilt,
                        Configuration.TransferConfiguration,
                        Configuration.TextureResolution);
                }
            }
        }
    }
    
    private void OnDestroy()
    {
        DisposeRakel();
        DisposeCanvas();
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

    public bool RakelPositionXLocked { get { return Configuration.InputConfiguration.RakelPositionX.Source == InputSourceType.Text; } }

    public bool RakelPositionYLocked { get { return Configuration.InputConfiguration.RakelPositionY.Source == InputSourceType.Text; } }

    public bool RakelPositionZLocked { get { return Configuration.InputConfiguration.RakelPositionZ.Source == InputSourceType.Text; } }

    public bool RakelPressureLocked { get { return Configuration.InputConfiguration.RakelPressure.Source == InputSourceType.Text; } }

    public bool RakelRotationLocked { get { return Configuration.InputConfiguration.RakelRotation.Source == InputSourceType.Text; } }

    public bool RakelTiltLocked { get { return Configuration.InputConfiguration.RakelTilt.Source == InputSourceType.Text; } }

    public void UpdateRakelPositionX(float value)
    {
        Configuration.InputConfiguration.RakelPositionX.Value = value;
        CreateInputManager();
    }

    public void UpdateRakelPositionXLocked(bool locked)
    {
        Configuration.InputConfiguration.RakelPositionX.Source = locked ? InputSourceType.Text : InputSourceType.Mouse;
        CreateInputManager();
    }

    public void UpdateRakelPositionY(float value)
    {
        Configuration.InputConfiguration.RakelPositionY.Value = value;
        CreateInputManager();
    }

    public void UpdateRakelPositionYLocked(bool locked)
    {
        Configuration.InputConfiguration.RakelPositionY.Source = locked ? InputSourceType.Text : InputSourceType.Mouse;
        CreateInputManager();
    }

    public void UpdateRakelPositionZ(float value)
    {
        Configuration.InputConfiguration.RakelPositionZ.Value = value;
        CreateInputManager();
    }

    public void UpdateRakelPositionZLocked(bool locked)
    {
        InputSourceType penOrKeyboard = USE_PEN ? InputSourceType.Pen : InputSourceType.Keyboard;
        Configuration.InputConfiguration.RakelPositionZ.Source = locked ? InputSourceType.Text : penOrKeyboard;
        CreateInputManager();
    }

    public void UpdateRakelPressure(float value)
    {
        Configuration.InputConfiguration.RakelPressure.Value = value;
        CreateInputManager();
    }

    public void UpdateRakelPressureLocked(bool locked)
    {
        InputSourceType penOrKeyboard = USE_PEN ? InputSourceType.Pen : InputSourceType.Keyboard;
        Configuration.InputConfiguration.RakelPressure.Source = locked ? InputSourceType.Text : penOrKeyboard;
        CreateInputManager();
    }

    public void UpdateRakelRotation(float rotation)
    {
        Configuration.InputConfiguration.RakelRotation.Value = rotation;
        CreateInputManager();
    }

    public void UpdateRakelRotationLocked(bool locked)
    {
        Configuration.InputConfiguration.RakelRotation.Source = locked ? InputSourceType.Text : InputSourceType.Mouse;
        CreateInputManager();
    }

    public void UpdateRakelTilt(float tilt)
    {
        Configuration.InputConfiguration.RakelTilt.Value = tilt;
        CreateInputManager();
    }

    public void UpdateRakelTiltLocked(bool locked)
    {
        Configuration.InputConfiguration.RakelTilt.Source = locked ? InputSourceType.Text : InputSourceType.Keyboard;
        CreateInputManager();
    }

    public void UpdateRakelLength(float worldSpaceLength)
    {
        Configuration.RakelConfiguration.Length = worldSpaceLength;
        CreateRakel();
        CreateInputInterpolator();
    }

    public void UpdateRakelWidth(float worldSpaceWidth)
    {
        Configuration.RakelConfiguration.Width = worldSpaceWidth;
        CreateRakel();
        CreateInputInterpolator();
    }

    public void UpdateTextureResolution(int pixelsPerWorldSpaceUnit)
    {
        Configuration.TextureResolution = pixelsPerWorldSpaceUnit;
        CreateCanvas();
        CreateRakel();
        CreateInputInterpolator();
    }

    // ****************************************************************************************
    // ***                                     TOP RIGHT                                    ***
    // ****************************************************************************************

    public void UpdateFillColor(Color_ color)
    {
        Configuration.FillConfiguration.Color = color;
    }

    public void UpdateColorMode(ColorMode mode)
    {
        Configuration.FillConfiguration.ColorMode = mode;
    }

    public void UpdateFillVolume(int volume)
    {
        Configuration.FillConfiguration.Volume = volume;
    }

    public void UpdateVolumeMode(VolumeMode mode)
    {
        Configuration.FillConfiguration.VolumeMode = mode;
    }

    public void FillApply()
    {
        FillConfiguration fillConfig = Configuration.FillConfiguration;

        ColorFiller colorFiller;
        if (fillConfig.ColorMode == ColorMode.Flat)
        {
            colorFiller = new FlatColorFiller(fillConfig.Color, Configuration.ColorSpace);
        } else
        {
            colorFiller = new GradientColorFiller(fillConfig.ColorMode, Configuration.ColorSpace);
        }

        VolumeFiller volumeFiller;
        if (fillConfig.VolumeMode == VolumeMode.Flat)
        {
            volumeFiller = new FlatVolumeFiller(fillConfig.Volume);
        } else
        {
            volumeFiller = new PerlinVolumeFiller(fillConfig.Volume);
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
        CreateInputInterpolator();
    }

    public void ClearCanvas()
    {
        CreateCanvas();
        CreateInputInterpolator();
    }

    // ****************************************************************************************
    // ***                                   BOTTOM RIGHT                                   ***
    // ****************************************************************************************

    public void UpdateEmitVolumeApplicationReservoir(float value)
    {
        Configuration.TransferConfiguration.EmitVolumeApplicationReservoirRate = value;
    }

    public void UpdateEmitVolumePickupReservoir(float value)
    {
        Configuration.TransferConfiguration.EmitVolumePickupReservoirRate = value;
    }

    public void UpdatePickupVolume(float value)
    {
        Configuration.TransferConfiguration.PickupVolume_MAX = value;
    }

    public void UpdateLayerThickness_MAX(float value)
    {
        Configuration.TransferConfiguration.LayerThickness_MAX = value;
    }

    public void UpdateNormalScale(float value)
    {
        Configuration.NormalScale = value;
        Canvas.NormalScale = value;
        Canvas.Render(Canvas.GetFullShaderRegion());
    }

    public void DoMacroAction()
    {
        Rakel.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen, Configuration.ColorSpace), new FlatVolumeFiller(1)));

        InputInterpolator.NewStroke();
        InputInterpolator.AddNode(
            new Vector3(-3, 0, -0.10f), 0,
            0,
            0,
            Configuration.TransferConfiguration,
            Configuration.TextureResolution);
    }

    public void DoMacro2Action()
    {

    }
}