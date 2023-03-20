using UnityEngine;

public class OilPaintEngine : MonoBehaviour
{
    public int BENCHMARK_STEPS = 0;
    public int TH_GROUP_SIZE_X = 1;
    public int TH_GROUP_SIZE_Y = 8;
    
    public Configuration Configuration { get; private set; }
    private ShaderRegionFactory ShaderRegionFactory;
    public RakelMouseInputManager RakelMouseInputManager { get; private set; }
    public float RakelRotation // used only for UI fetching
    {
        get
        {
            if (Configuration.RakelRotationLocked)
            {
                return Configuration.RakelRotation;
            }
            else
            {
                return RakelMouseInputManager.Rotation;
            }
        }
    }

    private TransferEngine TransferEngine;
    private OilPaintCanvas OilPaintCanvas;
    private Rakel Rakel;
    private InputInterpolator InputInterpolator;

    private ComputeShaderEngine ComputeShaderEngine;

    void Awake()
    {
        Configuration = new Configuration();
        //Configuration.LoadDebug();
        //Configuration.RakelRotation = 22;
        //Configuration.FillConfiguration.Mode = FillMode.Flat;
        //Configuration.FillConfiguration.Volume = 10;

        int wallColliderID = GameObject.Find("Wall").GetComponent<MeshCollider>().GetInstanceID();
        int canvasColliderID = GameObject.Find("Canvas").GetComponent<MeshCollider>().GetInstanceID();
        RakelMouseInputManager = new RakelMouseInputManager(wallColliderID, canvasColliderID);

        if (BENCHMARK_STEPS > 0)
        {
            Configuration.LoadBenchmark();
        }
    }

    void Start()
    {
        ComputeShaderEngine = new ComputeShaderEngine();

        ShaderRegionFactory = new ShaderRegionFactory(new Vector2Int(TH_GROUP_SIZE_X, TH_GROUP_SIZE_Y));

        CreateCanvas();
        CreateRakel();
        CreateOilPaintTransferEngine();
        CreateInputInterpolator();
    }

    void CreateCanvas()
    {
        DisposeCanvas();

        OilPaintCanvas = new OilPaintCanvas(Configuration.CanvasResolution, ShaderRegionFactory, ComputeShaderEngine);

        Debug.Log("Texture is "
                  + OilPaintCanvas.Texture.width + "x" + OilPaintCanvas.Texture.height
                  + " = " + OilPaintCanvas.Texture.width * OilPaintCanvas.Texture.height);
    }

    void CreateRakel()
    {
        DisposeRakel();

        Rakel = new Rakel(Configuration.RakelConfiguration, ShaderRegionFactory, ComputeShaderEngine);

        Debug.Log("Rakel is "
                  + Configuration.RakelConfiguration.Length * Configuration.RakelConfiguration.Resolution + "x" + Configuration.RakelConfiguration.Width * Configuration.RakelConfiguration.Resolution
                  + " = " + Configuration.RakelConfiguration.Length * Configuration.RakelConfiguration.Resolution * Configuration.RakelConfiguration.Width * Configuration.RakelConfiguration.Resolution);
    }

    void CreateOilPaintTransferEngine()
    {
        TransferEngine = new TransferEngine(ShaderRegionFactory);
    }

    void CreateInputInterpolator()
    {
        InputInterpolator = new InputInterpolator(TransferEngine, Rakel, OilPaintCanvas);
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
                    OilPaintCanvas);
            }
        }
        else
        {
            float rotation;
            if (Configuration.RakelRotationLocked)
            {
                rotation = Configuration.RakelRotation;
            }
            else
            {
                rotation = RakelMouseInputManager.Rotation;
            }

            Vector3 position = RakelMouseInputManager.Position;
            if (!position.Equals(Vector3.negativeInfinity))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    InputInterpolator.NewStroke();
                }
                InputInterpolator.AddNode(
                    position,
                    rotation,
                    0,
                    Configuration.TransferConfiguration,
                    Configuration.CanvasResolution);
            }
        }

        ComputeShaderEngine.ProcessTasks();
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
        if (OilPaintCanvas != null)
        {
            //Debug.Log("Disposing Canvas");
            OilPaintCanvas.Dispose();
        }
    }

    // ****************************************************************************************
    // ***                                      TOP LEFT                                    ***
    // ****************************************************************************************

    public void UpdateRakelRotation(float rotation)
    {
        Configuration.RakelRotation = rotation;
    }

    public void UpdateRakelRotationLocked(bool locked)
    {
        Configuration.RakelRotationLocked = locked;
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
        Configuration.CanvasResolution = pixelsPerWorldSpaceUnit;
        CreateCanvas();
        CreateInputInterpolator();
    }

    public void UpdateRakelResolution(int pixelsPerWorldSpaceUnit)
    {
        Configuration.RakelConfiguration.Resolution = pixelsPerWorldSpaceUnit;
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

    public void UpdateFillVolume(int volume)
    {
        Configuration.FillConfiguration.Volume = volume;
    }

    public void UpdateFillMode(FillMode mode)
    {
        Configuration.FillConfiguration.Mode = mode;
    }

    public void FillApply()
    {
        ReservoirFiller filler;
        switch (Configuration.FillConfiguration.Mode)
        {
            case FillMode.Perlin:
                filler = new PerlinNoiseFiller();
                break;
            case FillMode.Flat:
                filler = new FlatFiller();
                break;
            case FillMode.PerlinColored:
                filler = new PerlinNoiseFiller(true);
                break;
            case FillMode.FlatColored:
                filler = new FlatFiller(true);
                break;
            default:
                filler = new FlatFiller();
                break;
        }
        Rakel.Fill(Configuration.FillConfiguration.Color, Configuration.FillConfiguration.Volume, filler);
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
        Configuration.TransferConfiguration.EmitVolumeApplicationReservoir = value;
    }

    public void UpdateEmitVolumePickupReservoir(float value)
    {
        Configuration.TransferConfiguration.EmitVolumePickupReservoir = value;
    }

    public void UpdatePickupVolume(float value)
    {
        Configuration.TransferConfiguration.PickupVolume = value;
    }

    public void UpdateReservoirSmoothingKernelSize(int value)
    {
        Configuration.TransferConfiguration.ReservoirSmoothingKernelSize = value;
    }

    public void UpdateReservoirDiscardVolumeThreshold(int value)
    {
        Configuration.TransferConfiguration.ReservoirDiscardVolumeThreshold = value;
    }

    public void UpdateRakelEmitMode(TransferMapMode transferMapMode)
    {
        Configuration.TransferConfiguration.MapMode = transferMapMode;
    }

    public void UpdateRakelYPositionLocked(bool locked)
    {
        RakelMouseInputManager.YPositionLocked = locked;
    }

    public void DoMacroAction()
    {
        //Rakel.Fill(new Paint(new Color(0 / 255f, 107 / 255f, 60 / 255f), 240), new PerlinNoiseFiller());
        Rakel.Fill(Color_.CadmiumGreen, 240, new FlatFiller());

        InputInterpolator.NewStroke();
        InputInterpolator.AddNode(
            new Vector3(-5, 0, -0.10f),
            0,
            0,
            Configuration.TransferConfiguration,
            Configuration.CanvasResolution);

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

        InputInterpolator.AddNode(
            new Vector3(6, 0, -0.10f),
            0,
            0,
            Configuration.TransferConfiguration,
            Configuration.CanvasResolution);
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


        Rakel.Fill(Color_.CadmiumGreen, 1, new FlatFiller());

        TransferEngine.SimulateStep(
            new Vector3(-5, 0, -0.10f),
            0,
            0,
            Configuration.TransferConfiguration,
            Rakel,
            OilPaintCanvas);

        UpdateRakelWidth(3);
        UpdateRakelLength(4);

        TransferEngine.SimulateStep(
            new Vector3(-4, 0, -0.10f),
            0,
            0,
            Configuration.TransferConfiguration,
            Rakel,
            OilPaintCanvas);

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