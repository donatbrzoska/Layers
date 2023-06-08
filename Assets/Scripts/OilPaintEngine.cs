using UnityEngine;

public class OilPaintEngine : MonoBehaviour
{
    public bool DEBUG_SHADER = false;
    public int BENCHMARK_STEPS = 0;
    public int TH_GROUP_SIZE_X = 32;
    public int TH_GROUP_SIZE_Y = 1;

    public float ANCHOR_RATIO_X = 1;
    public float ANCHOR_RATIO_Y = 0.5f;

    public Configuration Configuration { get; private set; }
    public MouseAndKeyboardInput InputManager { get; private set; }
    public float RakelPositionX // used only for UI fetching
    {
        get
        {
            if (Configuration.RakelConfiguration.PositionLocked.x)
            {
                return Configuration.RakelConfiguration.Position.x;
            }
            else
            {
                return InputManager.Position.x;
            }
        }
    }
    public float RakelPositionY // used only for UI fetching
    {
        get
        {
            if (Configuration.RakelConfiguration.PositionLocked.y)
            {
                return Configuration.RakelConfiguration.Position.y;
            }
            else
            {
                return InputManager.Position.y;
            }
        }
    }
    public float RakelPositionZ // used only for UI fetching
    {
        get
        {
            if (Configuration.RakelConfiguration.PositionLocked.z)
            {
                return Configuration.RakelConfiguration.Position.z;
            }
            else
            {
                return InputManager.Position.z;
            }
        }
    }
    public float RakelRotation // used only for UI fetching
    {
        get
        {
            if (Configuration.RakelConfiguration.RotationLocked)
            {
                return Configuration.RakelConfiguration.Rotation;
            }
            else
            {
                return InputManager.Rotation;
            }
        }
    }
    public float RakelTilt // used only for UI fetching
    {
        get
        {
            if (Configuration.RakelConfiguration.TiltLocked)
            {
                return Configuration.RakelConfiguration.Tilt;
            }
            else
            {
                return InputManager.Tilt;
            }
        }
    }

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

        int wallColliderID = GameObject.Find("Wall").GetComponent<MeshCollider>().GetInstanceID();
        int canvasColliderID = GameObject.Find("Canvas").GetComponent<MeshCollider>().GetInstanceID();
        InputManager = new MouseAndKeyboardInput(wallColliderID, canvasColliderID);

        if (BENCHMARK_STEPS > 0)
        {
            Configuration.LoadBenchmark();
        }
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

        Canvas = new Canvas_(Configuration.TextureResolution, Configuration.NormalScale);

        Renderer renderer = GameObject.Find("Canvas").GetComponent<Renderer>();
        renderer.material.SetTexture("_MainTex", Canvas.Texture);
        renderer.material.EnableKeyword("_NORMALMAP");
        renderer.material.SetTexture("_BumpMap", Canvas.NormalMap);

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
                    Vector3 position = InputManager.Position;
                    if (Configuration.RakelConfiguration.PositionLocked.x)
                    {
                        position.x = Configuration.RakelConfiguration.Position.x;
                    }
                    if (Configuration.RakelConfiguration.PositionLocked.y)
                    {
                        position.y = Configuration.RakelConfiguration.Position.y;
                    }
                    if (Configuration.RakelConfiguration.PositionLocked.z)
                    {
                        position.z = Configuration.RakelConfiguration.Position.z;
                    }

                    float rotation = InputManager.Rotation;
                    if (Configuration.RakelConfiguration.RotationLocked)
                    {
                        rotation = Configuration.RakelConfiguration.Rotation;
                    }

                    float tilt = InputManager.Tilt;
                    if (Configuration.RakelConfiguration.TiltLocked)
                    {
                        tilt = Configuration.RakelConfiguration.Tilt;
                    }

                    InputInterpolator.AddNode(
                        position,
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
        if (Rakel != null)
        {
            //Debug.Log("Disposing Rakel");
            Rakel.Dispose();
        }
    }

    private void DisposeCanvas()
    {
        if (Canvas != null)
        {
            //Debug.Log("Disposing Canvas");
            Canvas.Dispose();
        }
    }

    // ****************************************************************************************
    // ***                                      TOP LEFT                                    ***
    // ****************************************************************************************

    public void UpdateRakelPositionX(float value)
    {
        Configuration.RakelConfiguration.Position.x = value;
    }

    public void UpdateRakelPositionXLocked(bool locked)
    {
        Configuration.RakelConfiguration.PositionLocked.x = locked;
    }

    public void UpdateRakelPositionY(float value)
    {
        Configuration.RakelConfiguration.Position.y = value;
    }

    public void UpdateRakelPositionYLocked(bool locked)
    {
        Configuration.RakelConfiguration.PositionLocked.y = locked;
    }

    public void UpdateRakelPositionZ(float value)
    {
        Configuration.RakelConfiguration.Position.z = value;
    }

    public void UpdateRakelPositionZLocked(bool locked)
    {
        Configuration.RakelConfiguration.PositionLocked.z = locked;
    }

    public void UpdateRakelRotation(float rotation)
    {
        Configuration.RakelConfiguration.Rotation = rotation;
    }

    public void UpdateRakelRotationLocked(bool locked)
    {
        Configuration.RakelConfiguration.RotationLocked = locked;
    }

    public void UpdateRakelTilt(float tilt)
    {
        Configuration.RakelConfiguration.Tilt = tilt;
    }

    public void UpdateRakelTiltLocked(bool locked)
    {
        Configuration.RakelConfiguration.TiltLocked = locked;
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
            colorFiller = new FlatColorFiller(fillConfig.Color);
        } else
        {
            colorFiller = new GradientColorFiller(fillConfig.ColorMode);
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
        Configuration.TransferConfiguration.EmitVolumeApplicationReservoir_MAX = value;
    }

    public void UpdateEmitVolumePickupReservoir(float value)
    {
        Configuration.TransferConfiguration.EmitVolumePickupReservoir_MAX = value;
    }

    public void UpdatePickupVolume(float value)
    {
        Configuration.TransferConfiguration.PickupVolume_MAX = value;
    }

    public void UpdateNormalScale(float value)
    {
        Configuration.NormalScale = value;
        Canvas.NormalScale = value;
        Canvas.Render(Canvas.GetFullShaderRegion());
    }

    public void DoMacroAction()
    {
        //Rakel.Fill(new Paint(new Color(0 / 255f, 107 / 255f, 60 / 255f), 240), new PerlinNoiseFiller());
        Rakel.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen), new FlatVolumeFiller(1)));

        InputInterpolator.NewStroke();
        InputInterpolator.AddNode(
            new Vector3(-3, 0, -0.10f),
            0,
            //60,
            41.4096f,
            Configuration.TransferConfiguration,
            Configuration.TextureResolution);

        //RakelInterpolator.AddNode(new Vector3(-4, 0, -0.10f), 0, 0, TextureResolution);
        //RakelInterpolator.AddNode(new Vector3(-3, 0, -0.10f), 0, 0, TextureResolution);
        //RakelInterpolator.AddNode(new Vector3(-2, 0, -0.10f), 0, 0, TextureResolution);
        //RakelInterpolator.AddNode(new Vector3(-1, 0, -0.10f), 0, 0, TextureResolution);
        //RakelInterpolator.AddNode(new Vector3(-0, 0, -0.10f), 0, 0, TextureResolution);
        //RakelInterpolator.AddNode(new Vector3(1, 0, -0.10f), 0, 0, TextureResolution);
        //RakelInterpolator.AddNode(new Vector3(2, 0, -0.10f), 0, 0, TextureResolution);
        //RakelInterpolator.AddNode(new Vector3(3, 0, -0.10f), 0, 0, TextureResolution);
        //RakelInterpolator.AddNode(new Vector3(4, 0, -0.10f), 0, 0, TextureResolution);
        //RakelInterpolator.AddNode(new Vector3(5, 0, -0.10f), 0, 0, TextureResolution);

        //RakelInterpolator.AddNode(new Vector3(6, 3, -0.10f), 0, 0, TextureResolution);

        //InputInterpolator.AddNode(
        //    new Vector3(6, 0, -0.10f),
        //    0,
        //    0,
        //    Configuration.TransferConfiguration,
        //    Configuration.CanvasResolution);
    }

    public void DoMacro2Action()
    {
        //Rakel.Fill(new Paint(new Color(0 / 255f, 107 / 255f, 60 / 255f), 240), new PerlinNoiseFiller());
        //RakelInterpolator.NewStroke();
        //RakelInterpolator.AddNode(new Vector3(-5, 0, -0.10f), 45, 0, TextureResolution);


        //Rakel.Fill(Color_.CadmiumGreen, 240, new FlatFiller());
        //RakelInterpolator.NewStroke();
        //RakelInterpolator.AddNode(
        //    new Vector3(-5, 0, -0.10f),
        //    0,
        //    0,
        //    Configuration.TransferConfiguration,
        //    Configuration.CanvasResolution);


        Rakel.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen), new FlatVolumeFiller(1)));

        TransferEngine.SimulateStep(
            new Vector3(-5, 0, -0.10f),
            0,
            45,
            Configuration.TransferConfiguration,
            Rakel,
            Canvas);

        UpdateRakelWidth(3);
        UpdateRakelLength(4);

        TransferEngine.SimulateStep(
            new Vector3(-4, 0, -0.10f),
            0,
            0,
            Configuration.TransferConfiguration,
            Rakel,
            Canvas);

        //TransferEngine.SimulateStep(
        //    new Vector3(-4, 0, -0.10f),
        //    0,
        //    0,
        //    Configuration.TransferConfiguration,
        //    Rakel,
        //    OilPaintCanvas);

        //TransferEngine.SimulateStep(
        //    new Vector3(-4, 0, -0.10f),
        //    0,
        //    0,
        //    Configuration.TransferConfiguration,
        //    Rakel,
        //    OilPaintCanvas);
    }
}