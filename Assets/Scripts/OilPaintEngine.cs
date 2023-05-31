using UnityEngine;

public class OilPaintEngine : MonoBehaviour
{
    public int BENCHMARK_STEPS = 0;
    public int TH_GROUP_SIZE_X = 32;
    public int TH_GROUP_SIZE_Y = 1;

    public Configuration Configuration { get; private set; }
    public MouseInput InputManager { get; private set; }
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
                return InputManager.Rotation;
            }
        }
    }

    private TransferEngine TransferEngine;
    private Canvas_ Canvas;
    private Rakel Rakel;
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
        InputManager = new MouseInput(wallColliderID, canvasColliderID);

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

        Canvas = new Canvas_(Configuration.TextureResolution);

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

        Rakel = new Rakel(Configuration.RakelConfiguration.Length, Configuration.RakelConfiguration.Width, Configuration.TextureResolution);

        Debug.Log("Rakel is "
                  + Configuration.RakelConfiguration.Length * Configuration.TextureResolution + "x" + Configuration.RakelConfiguration.Width * Configuration.TextureResolution
                  + " = " + Configuration.RakelConfiguration.Length * Configuration.TextureResolution * Configuration.RakelConfiguration.Width * Configuration.TextureResolution);
    }

    void CreateOilPaintTransferEngine()
    {
        TransferEngine = new TransferEngine();
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

            if (InputManager.CanvasHit)
            {

                Vector3 position = InputManager.Position;
                if (Configuration.RakelYPositionLocked)
                {
                    position.y = 0;
                }

                float rotation = InputManager.Rotation;
                if (Configuration.RakelRotationLocked)
                {
                    rotation = Configuration.RakelRotation;
                }

                if (InputManager.StrokeBegin)
                {
                    InputInterpolator.NewStroke();
                }

                InputInterpolator.AddNode(
                    position,
                    rotation,
                    Configuration.RakelTilt,
                    Configuration.TransferConfiguration,
                    Configuration.TextureResolution);
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

    public void UpdateRakelRotation(float rotation)
    {
        Configuration.RakelRotation = rotation;
    }

    public void UpdateRakelRotationLocked(bool locked)
    {
        Configuration.RakelRotationLocked = locked;
    }

    public void UpdateRakelTilt(float tilt)
    {
        Configuration.RakelTilt = tilt;
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

    public void UpdateRakelYPositionLocked(bool locked)
    {
        Configuration.RakelYPositionLocked = locked;
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