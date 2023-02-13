using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System.Threading;

public class OilPaintEngine : MonoBehaviour
{
    public bool BENCHMARK = false;

    public int TextureResolution { get; private set; } // texture space pixels per 1 world space
    private RenderTexture Texture;
    private RenderTexture NormalMap;

    private Renderer CanvasRenderer;
    private float CanvasWidth; // world space
    private float CanvasHeight; // world space
    private Vector3 CanvasPosition; // world space
    private WorldSpaceCanvas WorldSpaceCanvas;
    private ComputeBuffer Canvas;

    //private IRakelPaintReservoir RakelPaintReservoir;
    //public Paint RakelPaint { get; private set; }
    public Paint FillPaint { get; private set; }
    public FillMode FillMode { get; private set; }

    public RakelInputManager RakelInputManager { get; private set; }
    public float RakelRotation { get; private set; }
    public float RakelLength { get; private set; } // world space
    public float RakelWidth { get; private set; } // world space
    public int RakelResolution { get; private set; }
    public EmitMode RakelEmitMode { get; private set; }
    public int ReservoirSmoothingKernelSize { get; private set; }
    public int ReservoirDiscardVolumeThreshold { get; private set; }
    public bool RakelYPositionLocked { get; private set; }
    private Vector3 RakelPosition;
    private IRakel Rakel;

    private RakelInterpolator RakelInterpolator;


    private Queue<ComputeShaderTask> ComputeShaderTasks = new Queue<ComputeShaderTask>();

    void Awake()
    {
        CanvasRenderer = GameObject.Find("Canvas").GetComponent<Renderer>();
        CanvasWidth = GameObject.Find("Canvas").GetComponent<Transform>().localScale.x * 10; // convert scale attribute to world space
        CanvasHeight = GameObject.Find("Canvas").GetComponent<Transform>().localScale.y * 10; // convert scale attribute to world space
        CanvasPosition = GameObject.Find("Canvas").GetComponent<Transform>().position;

        int wallColliderID = GameObject.Find("Wall").GetComponent<MeshCollider>().GetInstanceID();
        int canvasColliderID = GameObject.Find("Canvas").GetComponent<MeshCollider>().GetInstanceID();
        RakelInputManager = new RakelInputManager(wallColliderID, canvasColliderID);

        LoadDefaultConfig();
        LoadDefaultConfig2();
        //LoadDefaultConfig_SmallRakel();
        //LoadDebugConfig();
        //LoadDebugConfig2();
    }

    void LoadDefaultConfig()
    {
        RakelRotation = 0;
        RakelLength = 8f;
        RakelWidth = 0.3f;
        TextureResolution = 100;
        RakelResolution = TextureResolution;

        FillPaint = new Paint(Colors.GetColor(_Color.CadmiumGreen), 240);
        FillMode = FillMode.Perlin;

        ReservoirSmoothingKernelSize = 1;
        ReservoirDiscardVolumeThreshold = 10;
        RakelEmitMode = EmitMode.PolygonClipping;
        RakelYPositionLocked = false;
    }

    void LoadDefaultConfig2()
    {
        RakelRotation = 0;
        RakelLength = 4f;
        RakelWidth = 0.5f;
        TextureResolution = 60;
        RakelResolution = TextureResolution;

        FillPaint = new Paint(Colors.GetColor(_Color.CadmiumGreen), 600);
        FillMode = FillMode.PerlinColored;

        RakelEmitMode = EmitMode.Bilinear;
        RakelYPositionLocked = false;
    }

    void LoadDefaultConfig_SmallRakel()
    {
        RakelRotation = 0;
        RakelInputManager.RotationLocked = true;
        RakelLength = 2f;
        RakelWidth = 0.5f;
        TextureResolution = 80;
        RakelResolution = TextureResolution;

        FillMode = FillMode.PerlinColored;

        RakelEmitMode = EmitMode.PolygonClipping;
        RakelYPositionLocked = false;
    }

    void LoadDebugConfig()
    {
        RakelRotation = 45;
        RakelInputManager.RotationLocked = true;
        RakelLength = 4;
        RakelWidth = 1;
        TextureResolution = 1;
        RakelResolution = TextureResolution;

        FillMode = FillMode.Flat;

        RakelEmitMode = EmitMode.NearestNeighbour;
        RakelYPositionLocked = false;
    }

    void LoadDebugConfig2()
    {
        RakelRotation = 45;
        RakelInputManager.RotationLocked = true;
        RakelLength = 2f;
        RakelWidth = 0.5f;
        TextureResolution = 20;
        RakelResolution = TextureResolution;

        FillMode = FillMode.FlatColored;
        FillPaint = new Paint(Colors.GetColor(_Color.CadmiumGreen), 50);

        RakelEmitMode = EmitMode.NearestNeighbour;
        RakelYPositionLocked = false;
    }

    void Start()
    {
        CreateCanvas();
        CreateRakel();
        CreateRakelDrawer();
    }

    void CreateCanvas()
    {
        WorldSpaceCanvas = new WorldSpaceCanvas(CanvasHeight, CanvasWidth, TextureResolution, CanvasPosition);
        DisposeCanvas();
        Canvas = new ComputeBuffer(WorldSpaceCanvas.TextureSize.x * WorldSpaceCanvas.TextureSize.y, sizeof(float) * 4 + sizeof(int));
        Debug.Log("Texture is " + WorldSpaceCanvas.TextureSize.x + "x" + WorldSpaceCanvas.TextureSize.y + " = " + WorldSpaceCanvas.TextureSize.x * WorldSpaceCanvas.TextureSize.y);


        Texture = new RenderTexture(WorldSpaceCanvas.TextureSize.x, WorldSpaceCanvas.TextureSize.y, 1);
        Texture.filterMode = FilterMode.Point;
        Texture.enableRandomWrite = true;
        Texture.Create();
        CanvasRenderer.material.SetTexture("_MainTex", Texture);

        // set colors to white
        IntelGPUShaderRegion sr = new IntelGPUShaderRegion(
            new Vector2Int(Texture.height, 0),
            new Vector2Int(Texture.height, Texture.width),
            new Vector2Int(0, 0),
            new Vector2Int(Texture.width, 0)
        );
        ComputeShaderTask cst = new ComputeShaderTask(
            "SetTextureShader",
            ComputeShaderUtil.LoadComputeShader("SetTextureShader"),
            new List<CSAttribute>() {
                new CSInts2("CalculationSize", WorldSpaceCanvas.TextureSize),
                new CSFloats4("Value", Vector4.one),
                new CSTexture("Target", Texture)
            },
            sr.ThreadGroups,
            null,
            new List<ComputeBuffer>(),
            null
        );
        cst.Run();


        NormalMap = new RenderTexture(WorldSpaceCanvas.TextureSize.x, WorldSpaceCanvas.TextureSize.y, 1);
        NormalMap.filterMode = FilterMode.Point;
        NormalMap.enableRandomWrite = true;
        NormalMap.Create();
        CanvasRenderer.material.EnableKeyword("_NORMALMAP");
        CanvasRenderer.material.SetTexture("_BumpMap", NormalMap);

        // set normals to up
        cst = new ComputeShaderTask(
            "SetTextureShader",
            ComputeShaderUtil.LoadComputeShader("SetTextureShader"),
            new List<CSAttribute>() {
                new CSInts2("CalculationSize", WorldSpaceCanvas.TextureSize),
                new CSFloats4("Value", (new Vector4(0, 0, 1, 0) + Vector4.one) / 2),
                new CSTexture("Target", NormalMap)
            },
            sr.ThreadGroups,
            null,
            new List<ComputeBuffer>(),
            null
        );
        cst.Run();
    }

    void CreateRakel()
    {
        //RakelPaintReservoir = new RakelPaintReservoir(
        //    WorldSpaceLengthToTextureSpaceLength(RakelLength, TextureResolution),
        //    WorldSpaceLengthToTextureSpaceLength(RakelWidth, TextureResolution));
        DisposeRakel();

        Rakel = new Rakel(RakelLength, RakelWidth, RakelResolution, ComputeShaderTasks);
        int rakelPixelsLength = MathUtil.ToTextureSpaceLength(RakelLength, RakelResolution);
        int rakelPixelsWidth = MathUtil.ToTextureSpaceLength(RakelWidth, RakelResolution);
        Debug.Log("Rakel is " + rakelPixelsLength + "x" + rakelPixelsWidth + " = " + rakelPixelsLength * rakelPixelsWidth);
    }

    void CreateRakelDrawer()
    {
        RakelInterpolator = new RakelInterpolator(Rakel, WorldSpaceCanvas, Canvas, Texture, NormalMap);
    }

    void Update()
    {
        if (BENCHMARK){
            for (int i=0; i<50; i++){
                float x = Random.Range(-5f, 5f);
                float y = Random.Range(-3f, 3f);
                Rakel.Apply(new Vector3(x,y,0), 0, 0, RakelEmitMode, ReservoirDiscardVolumeThreshold, ReservoirSmoothingKernelSize, WorldSpaceCanvas, Canvas, Texture, NormalMap);
            }
        } else {
            RakelRotation = RakelInputManager.GetRotation();
            RakelPosition = RakelInputManager.GetPosition();

            if (!RakelPosition.Equals(Vector3.negativeInfinity))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    RakelInterpolator.NewStroke();
                }
                if (RakelYPositionLocked)
                {
                    RakelPosition.y = 0;
                }
                RakelInterpolator.AddNode(RakelPosition, RakelRotation, 0, RakelEmitMode, ReservoirDiscardVolumeThreshold, ReservoirSmoothingKernelSize, TextureResolution);
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
            Rakel.Dispose();
    }

    private void DisposeCanvas()
    {
        if (Canvas != null)
            Canvas.Dispose();
    }

    // ****************************************************************************************
    // ***                                      TOP LEFT                                    ***
    // ****************************************************************************************

    public void UpdateRakelRotation(float rotation)
    {
        RakelRotation = rotation;
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
        DisposeCanvas();

        TextureResolution = pixelsPerWorldSpaceUnit;
        CreateCanvas();
        CreateRakelDrawer();
    }

    public void UpdateRakelResolution(int pixelsPerWorldSpaceUnit)
    {
        DisposeRakel();

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
        OnDestroy();

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
        RakelYPositionLocked = locked;
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