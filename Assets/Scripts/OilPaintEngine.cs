using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System.Threading;

public class OilPaintEngine : MonoBehaviour
{
    public bool BENCHMARK = false;

    public int TextureResolution { get; private set; } // texture space pixels per 1 world space
    private OilPaintCanvas OilPaintCanvas;

    public Paint FillPaint { get; private set; }
    public FillMode FillMode { get; private set; }

    public RakelInputManager RakelInputManager { get; private set; }
    public float RakelLength { get; private set; } // world space
    public float RakelWidth { get; private set; } // world space
    public int RakelResolution { get; private set; }
    public EmitMode RakelEmitMode { get; private set; }
    public int ReservoirSmoothingKernelSize { get; private set; }
    public int ReservoirDiscardVolumeThreshold { get; private set; }
    private Vector3 RakelPosition;
    private IRakel Rakel;

    private RakelInterpolator RakelInterpolator;


    private Queue<ComputeShaderTask> ComputeShaderTasks = new Queue<ComputeShaderTask>();

    void Awake()
    {
        int wallColliderID = GameObject.Find("Wall").GetComponent<MeshCollider>().GetInstanceID();
        int canvasColliderID = GameObject.Find("Canvas").GetComponent<MeshCollider>().GetInstanceID();
        RakelInputManager = new RakelInputManager(wallColliderID, canvasColliderID);

        LoadDefaultConfig();
        LoadDefaultConfig2();
        LoadDefaultConfig_SmallRakel();
        //LoadDebugConfig();
        //LoadDebugConfig2();
    }

    void LoadDefaultConfig()
    {
        RakelInputManager.Rotation = 0;
        RakelLength = 8f;
        RakelWidth = 0.3f;
        TextureResolution = 100;
        RakelResolution = TextureResolution;

        FillPaint = new Paint(Colors.GetColor(_Color.CadmiumGreen), 240);
        FillMode = FillMode.Perlin;

        ReservoirSmoothingKernelSize = 1;
        ReservoirDiscardVolumeThreshold = 10;
        RakelEmitMode = EmitMode.PolygonClipping;
    }

    void LoadDefaultConfig2()
    {
        RakelInputManager.Rotation = 0;
        RakelLength = 4f;
        RakelWidth = 0.5f;
        TextureResolution = 60;
        RakelResolution = TextureResolution;

        FillPaint = new Paint(Colors.GetColor(_Color.CadmiumGreen), 600);
        FillMode = FillMode.PerlinColored;

        RakelEmitMode = EmitMode.Bilinear;
    }

    void LoadDefaultConfig_SmallRakel()
    {
        RakelInputManager.Rotation = 20;
        RakelInputManager.RotationLocked = true;
        RakelLength = 2f;
        RakelWidth = 0.5f;
        TextureResolution = 60;
        RakelResolution = TextureResolution;

        FillMode = FillMode.PerlinColored;

        RakelEmitMode = EmitMode.PolygonClipping;
    }

    void LoadDebugConfig()
    {
        RakelInputManager.Rotation = 45;
        RakelInputManager.RotationLocked = true;
        RakelLength = 4;
        RakelWidth = 1;
        TextureResolution = 1;
        RakelResolution = TextureResolution;

        FillMode = FillMode.Flat;

        RakelEmitMode = EmitMode.NearestNeighbour;
    }

    void LoadDebugConfig2()
    {
        RakelInputManager.Rotation = 45;
        RakelInputManager.RotationLocked = true;
        RakelLength = 2f;
        RakelWidth = 0.5f;
        TextureResolution = 20;
        RakelResolution = TextureResolution;

        FillMode = FillMode.FlatColored;
        FillPaint = new Paint(Colors.GetColor(_Color.CadmiumGreen), 50);

        RakelEmitMode = EmitMode.NearestNeighbour;
    }

    void Start()
    {
        CreateCanvas();
        CreateRakel();
        CreateRakelDrawer();
    }

    void CreateCanvas()
    {
        DisposeCanvas();

        OilPaintCanvas = new OilPaintCanvas(TextureResolution);

        Debug.Log("Texture is " + OilPaintCanvas.Texture.width + "x" + OilPaintCanvas.Texture.height + " = " + OilPaintCanvas.Texture.width * OilPaintCanvas.Texture.height);
    }

    void CreateRakel()
    {
        DisposeRakel();

        Rakel = new Rakel(RakelLength, RakelWidth, RakelResolution, ComputeShaderTasks);

        Debug.Log("Rakel is " + RakelLength * RakelResolution + "x" + RakelWidth * RakelResolution + " = " + RakelLength * RakelResolution * RakelWidth * RakelResolution);
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
                Rakel.Apply(new Vector3(x,y,0), 0, 0, RakelEmitMode, ReservoirDiscardVolumeThreshold, ReservoirSmoothingKernelSize, OilPaintCanvas);
            }
        } else {
            float rotation = RakelInputManager.Rotation;

            RakelPosition = RakelInputManager.Position;
            if (!RakelPosition.Equals(Vector3.negativeInfinity))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    RakelInterpolator.NewStroke();
                }
                RakelInterpolator.AddNode(RakelPosition, rotation, 0, RakelEmitMode, ReservoirDiscardVolumeThreshold, ReservoirSmoothingKernelSize, TextureResolution);
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
        RakelInputManager.Rotation = rotation;
    }

    public void UpdateRakelRotationLocked(bool locked)
    {
        RakelInputManager.RotationLocked = locked;
    }

    public void UpdateRakelLength(float worldSpaceLength)
    {
        RakelLength = worldSpaceLength;
        CreateRakel();
        CreateRakelDrawer();
    }

    public void UpdateRakelWidth(float worldSpaceWidth)
    {
        RakelWidth = worldSpaceWidth;
        CreateRakel();
        CreateRakelDrawer();
    }

    public void UpdateTextureResolution(int pixelsPerWorldSpaceUnit)
    {
        TextureResolution = pixelsPerWorldSpaceUnit;
        CreateCanvas();
        CreateRakelDrawer();
    }

    public void UpdateRakelResolution(int pixelsPerWorldSpaceUnit)
    {
        RakelResolution = pixelsPerWorldSpaceUnit;
        CreateRakel();
        CreateRakelDrawer();
    }

    // ****************************************************************************************
    // ***                                     TOP RIGHT                                    ***
    // ****************************************************************************************

    public void UpdateRakelPaint(_Color color, int volume, FillMode fillMode)
    {
        FillPaint = new Paint(Colors.GetColor(color), volume);
        FillMode = fillMode;

        ReservoirFiller filler;
        switch (FillMode)
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
        Rakel.Fill(FillPaint, filler);
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
        ReservoirSmoothingKernelSize = value;
    }

    public void UpdateReservoirDiscardVolumeThreshold(int value)
    {
        ReservoirDiscardVolumeThreshold = value;
    }

    public void UpdateRakelEmitMode(EmitMode emitMode)
    {
        RakelEmitMode = emitMode;
    }

    public void UpdateRakelYPositionLocked(bool locked)
    {
        RakelInputManager.YPositionLocked = locked;
    }

    public void DoMacroAction()
    {
        //Rakel.Fill(new Paint(new Color(0 / 255f, 107 / 255f, 60 / 255f), 240), new PerlinNoiseFiller());
        Rakel.Fill(new Paint(new Color(0 / 255f, 107 / 255f, 60 / 255f), 240), new FlatFiller());

        RakelInterpolator.NewStroke();
        RakelInterpolator.AddNode(new Vector3(-5, 0, -0.10f), 0, 0, RakelEmitMode, ReservoirDiscardVolumeThreshold, ReservoirSmoothingKernelSize, TextureResolution);

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

        RakelInterpolator.AddNode(new Vector3(6, 0, -0.10f), 0, 0, RakelEmitMode, ReservoirDiscardVolumeThreshold, ReservoirSmoothingKernelSize, TextureResolution);
    }

    public void DoMacro2Action()
    {
        //Rakel.Fill(new Paint(new Color(0 / 255f, 107 / 255f, 60 / 255f), 240), new PerlinNoiseFiller());
        //RakelInterpolator.NewStroke();
        //RakelInterpolator.AddNode(new Vector3(-5, 0, -0.10f), 45, 0, TextureResolution);

        Rakel.Fill(new Paint(new Color(0 / 255f, 107 / 255f, 60 / 255f), 1), new FlatFiller());
        RakelInterpolator.NewStroke();
        RakelInterpolator.AddNode(new Vector3(-5, 0, -0.10f), 0, 0, RakelEmitMode, ReservoirDiscardVolumeThreshold, ReservoirSmoothingKernelSize, TextureResolution);
    }
}