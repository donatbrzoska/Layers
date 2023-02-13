using UnityEngine;
using System.Collections.Generic;

public class OilPaintEngine : MonoBehaviour
{
    public bool BENCHMARK = false;

    public Configuration Configuration { get; private set; }
    public RakelInputManager RakelMouseInputManager { get; private set; }
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

    private OilPaintCanvas OilPaintCanvas;
    private IRakel Rakel;
    private RakelInterpolator RakelInterpolator;

    private Queue<ComputeShaderTask> ComputeShaderTasks;

    void Awake()
    {
        Configuration = new Configuration();

        int wallColliderID = GameObject.Find("Wall").GetComponent<MeshCollider>().GetInstanceID();
        int canvasColliderID = GameObject.Find("Canvas").GetComponent<MeshCollider>().GetInstanceID();
        RakelMouseInputManager = new RakelInputManager(wallColliderID, canvasColliderID);
    }

    void Start()
    {
        CreateCanvas();
        CreateRakel();
        CreateRakelDrawer();

        ComputeShaderTasks = new Queue<ComputeShaderTask>();
    }

    void CreateCanvas()
    {
        DisposeCanvas();

        OilPaintCanvas = new OilPaintCanvas(Configuration.CanvasResolution);

        Debug.Log("Texture is "
                  + OilPaintCanvas.Texture.width + "x" + OilPaintCanvas.Texture.height
                  + " = " + OilPaintCanvas.Texture.width * OilPaintCanvas.Texture.height);
    }

    void CreateRakel()
    {
        DisposeRakel();

        Rakel = new Rakel(Configuration.RakelConfiguration, ComputeShaderTasks);

        Debug.Log("Rakel is "
                  + Configuration.RakelConfiguration.Length * Configuration.RakelConfiguration.Resolution + "x" + Configuration.RakelConfiguration.Width * Configuration.RakelConfiguration.Resolution
                  + " = " + Configuration.RakelConfiguration.Length * Configuration.RakelConfiguration.Resolution * Configuration.RakelConfiguration.Width * Configuration.RakelConfiguration.Resolution);
    }

    void CreateRakelDrawer()
    {
        RakelInterpolator = new RakelInterpolator(Rakel, OilPaintCanvas);
    }

    void Update()
    {
        if (BENCHMARK){
            for (int i=0; i<50; i++){
                float x = Random.Range(-5f, 5f);
                float y = Random.Range(-3f, 3f);
                Rakel.Apply(
                    new Vector3(x,y,0),
                    0,
                    0,
                    Configuration.TransferConfiguration,
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
                    RakelInterpolator.NewStroke();
                }
                RakelInterpolator.AddNode(
                    position,
                    rotation,
                    0,
                    Configuration.TransferConfiguration,
                    Configuration.CanvasResolution);
            }
        }

        processComputeShaderTasks();
    }

    private void processComputeShaderTasks(int n=int.MaxValue)
    {
        if (ComputeShaderTasks != null)
        {
            while (ComputeShaderTasks.Count > 0 && n-- >= 0)
            {
                ComputeShaderTask cst = ComputeShaderTasks.Dequeue();
                cst.Run();
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
        CreateRakelDrawer();
    }

    public void UpdateRakelWidth(float worldSpaceWidth)
    {
        Configuration.RakelConfiguration.Width = worldSpaceWidth;
        CreateRakel();
        CreateRakelDrawer();
    }

    public void UpdateTextureResolution(int pixelsPerWorldSpaceUnit)
    {
        Configuration.CanvasResolution = pixelsPerWorldSpaceUnit;
        CreateCanvas();
        CreateRakelDrawer();
    }

    public void UpdateRakelResolution(int pixelsPerWorldSpaceUnit)
    {
        Configuration.RakelConfiguration.Resolution = pixelsPerWorldSpaceUnit;
        CreateRakel();
        CreateRakelDrawer();
    }

    // ****************************************************************************************
    // ***                                     TOP RIGHT                                    ***
    // ****************************************************************************************

    public void UpdateRakelPaint(_Color color, int volume, FillMode fillMode)
    {
        Configuration.FillPaint = new Paint(Colors.GetColor(color), volume);
        Configuration.FillMode = fillMode;

        ReservoirFiller filler;
        switch (Configuration.FillMode)
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
        Rakel.Fill(Configuration.FillPaint, filler);
    }

    // ****************************************************************************************
    // ***                                    BOTTOM LEFT                                   ***
    // ****************************************************************************************

    public void ClearCanvas()
    {
        CreateCanvas();
        CreateRakel();
        CreateRakelDrawer(); // TODO make Rakelinterpolator not dependent on only one canvas
    }

    // ****************************************************************************************
    // ***                                   BOTTOM RIGHT                                   ***
    // ****************************************************************************************

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
        Rakel.Fill(new Paint(new Color(0 / 255f, 107 / 255f, 60 / 255f), 240), new FlatFiller());

        RakelInterpolator.NewStroke();
        RakelInterpolator.AddNode(
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

        RakelInterpolator.AddNode(
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

        Rakel.Fill(new Paint(new Color(0 / 255f, 107 / 255f, 60 / 255f), 1), new FlatFiller());
        RakelInterpolator.NewStroke();
        RakelInterpolator.AddNode(
            new Vector3(-5, 0, -0.10f),
            0,
            0,
            Configuration.TransferConfiguration,
            Configuration.CanvasResolution);
    }
}