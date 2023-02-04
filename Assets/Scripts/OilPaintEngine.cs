using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System.Threading;

public class OilPaintEngine : MonoBehaviour
{
    public bool BENCHMARK = false;
    private Camera Camera;

    public int TextureResolution { get; private set; } // texture space pixels per 1 world space
    private RenderTexture Texture;
    private RenderTexture NormalMap;

    private int CanvasColliderID;
    private Renderer CanvasRenderer;
    private float CanvasWidth; // world space
    private float CanvasHeight; // world space
    private Vector3 CanvasPosition; // world space
    private WorldSpaceCanvas WorldSpaceCanvas;
    private ComputeBuffer Canvas;

    //private IRakelPaintReservoir RakelPaintReservoir;
    //public Paint RakelPaint { get; private set; }
    public FillMode FillMode { get; private set; }

    private static Vector2 NO_VECTOR2 = new Vector2(float.NaN, float.NaN);
    private bool PreviousMousePositionInitialized = false;
    private Vector2 PreviousMousePosition = NO_VECTOR2;

    public float RakelRotation { get; private set; }
    public bool RakelRotationLocked { get; private set; }
    public float RakelLength { get; private set; } // world space
    public float RakelWidth { get; private set; } // world space
    public bool RakelYPositionLocked { get; private set; }
    private IRakel Rakel;

    private RakelInterpolator RakelInterpolator;


    private Queue<ComputeShaderTask> ComputeShaderTasks = new Queue<ComputeShaderTask>();

    void Awake()
    {
        Camera = GameObject.Find("Main Camera").GetComponent<Camera>();

        CanvasColliderID = GameObject.Find("Canvas").GetComponent<MeshCollider>().GetInstanceID();
        CanvasRenderer = GameObject.Find("Canvas").GetComponent<Renderer>();
        CanvasWidth = GameObject.Find("Canvas").GetComponent<Transform>().localScale.x * 10; // convert scale attribute to world space
        CanvasHeight = GameObject.Find("Canvas").GetComponent<Transform>().localScale.y * 10; // convert scale attribute to world space
        CanvasPosition = GameObject.Find("Canvas").GetComponent<Transform>().position;
    }

    void LoadDefaultConfig()
    {
        RakelRotation = 0;
        RakelRotationLocked = false;
        RakelLength = 8f;
        RakelWidth = 0.3f;
        TextureResolution = 100;

        FillMode = FillMode.Perlin;

        RakelYPositionLocked = false;
    }

    void LoadDefaultConfig2()
    {
        RakelRotation = 0;
        RakelRotationLocked = true;
        RakelLength = 5f;
        RakelWidth = 2f;
        TextureResolution = 40;

        FillMode = FillMode.Perlin;

        RakelYPositionLocked = false;
    }

    void LoadDefaultConfig_SmallRakel()
    {
        RakelRotation = 0;
        RakelRotationLocked = true;
        RakelLength = 2f;
        RakelWidth = 0.2f;
        TextureResolution = 80;

        FillMode = FillMode.Perlin;

        RakelYPositionLocked = false;
    }

    void LoadDebugConfig()
    {
        RakelRotation = 12;
        RakelRotationLocked = true;
        RakelLength = 2f;
        RakelWidth = 0.2f;
        TextureResolution = 40;

        FillMode = FillMode.Flat;

        RakelYPositionLocked = false;
    }

    void LoadDebugConfig2()
    {
        RakelRotation = 45;
        RakelRotationLocked = true;
        RakelLength = 5f;
        RakelWidth = 2f;
        TextureResolution = 5;

        FillMode = FillMode.Perlin;

        RakelYPositionLocked = false;
    }

    void Start()
    {
        LoadDefaultConfig();
        LoadDefaultConfig2();
        LoadDefaultConfig_SmallRakel();
        //LoadDebugConfig();
        //LoadDebugConfig2();

        CreateCanvas();
        CreateRakelDrawer();
    }

    void CreateCanvas()
    {
        WorldSpaceCanvas = new WorldSpaceCanvas(CanvasHeight, CanvasWidth, TextureResolution, CanvasPosition);
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

    void CreateRakelDrawer()
    {
        //RakelPaintReservoir = new RakelPaintReservoir(
        //    WorldSpaceLengthToTextureSpaceLength(RakelLength, TextureResolution),
        //    WorldSpaceLengthToTextureSpaceLength(RakelWidth, TextureResolution));
        if (Rakel != null)
            Rakel.Dispose();
                    
        Rakel = new Rakel(RakelLength, RakelWidth, TextureResolution, ComputeShaderTasks);
        int rakelPixelsLength = MathUtil.ToTextureSpaceLength(RakelLength, TextureResolution);
        int rakelPixelsWidth = MathUtil.ToTextureSpaceLength(RakelWidth, TextureResolution);
        Debug.Log("Rakel is " + rakelPixelsLength + "x" + rakelPixelsWidth + " = " + rakelPixelsLength * rakelPixelsWidth);

        RakelInterpolator = new RakelInterpolator(Rakel, WorldSpaceCanvas, Canvas, Texture, NormalMap);
    }

    void Update()
    {
        if (BENCHMARK){
            for (int i=0; i<50; i++){
                float x = Random.Range(-5f, 5f);
                float y = Random.Range(-3f, 3f);
                Rakel.Apply(new Vector3(x,y,0), 0, 0, WorldSpaceCanvas, Canvas, Texture, NormalMap);
            }
        } else {
            if (!RakelRotationLocked)
            {
                Vector2 currentMousePosition = Input.mousePosition;
                if (!PreviousMousePositionInitialized)
                {
                    PreviousMousePosition = currentMousePosition;
                    PreviousMousePositionInitialized = true;
                }
                else
                {
                    Vector2 direction = currentMousePosition - PreviousMousePosition;

                    if (direction.magnitude > 8)
                    {
                        float angle = MathUtil.Angle360(Vector2.right, direction);
                        RakelRotation = angle;

                        PreviousMousePosition = currentMousePosition;
                    }
                }
            }

            Vector3 worldSpaceHit = InputUtil.GetMouseHit(Camera, CanvasColliderID);
            if (!worldSpaceHit.Equals(Vector3.negativeInfinity))
            {
                //Debug.Log("world space hit at " + worldSpaceHit);
                if (Input.GetMouseButtonDown(0))
                {
                    RakelInterpolator.NewStroke();
                }
                if (RakelYPositionLocked)
                {
                    worldSpaceHit.y = 0;
                }
                RakelInterpolator.AddNode(worldSpaceHit, RakelRotation, 0, TextureResolution);
            }
        }

        processComputeShaderTasks();
    }

    private void processComputeShaderTasks(int n=int.MaxValue)
    {
        while (ComputeShaderTasks.Count > 0 && n-- >= 0)
        {
            ComputeShaderTask cst = ComputeShaderTasks.Dequeue();
            cst.Run();
        }
    }
    
    private void OnDestroy()
    {
        if (Rakel != null)
            Rakel.Dispose();

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
        RakelRotationLocked = locked;
    }

    public void UpdateRakelLength(float worldSpaceLength)
    {
        RakelLength = worldSpaceLength;
        CreateRakelDrawer();
    }

    public void UpdateRakelWidth(float worldSpaceWidth)
    {
        RakelWidth = worldSpaceWidth;
        CreateRakelDrawer();
    }

    public void UpdateTextureResolution(int pixelsPerWorldSpaceUnit)
    {
        OnDestroy();

        TextureResolution = pixelsPerWorldSpaceUnit;
        CreateCanvas();
        CreateRakelDrawer();
    }

    // ****************************************************************************************
    // ***                                     TOP RIGHT                                    ***
    // ****************************************************************************************

    public void UpdateRakelPaint(Paint paint, FillMode fillMode)
    {
        FillMode = fillMode;

        ReservoirFiller filler;
        switch (FillMode) {
            case FillMode.Perlin:
                filler = new PerlinNoiseFiller();
                break;
            case FillMode.Flat:
                filler = new FlatFiller();
                break;
            //case FillMode.FlatColored:
            //    filler = new PerlinNoiseFiller();
            //    break;
            default:
                filler = new FlatFiller();
                break;
        }
        //    RakelPaint = paint;
        Rakel.Fill(paint, filler);
    }

    // ****************************************************************************************
    // ***                                    BOTTOM LEFT                                   ***
    // ****************************************************************************************

    public void ClearCanvas()
    {
        OnDestroy();

        CreateCanvas();
        CreateRakelDrawer(); // TODO make Rakelinterpolator not dependent on only one canvas
    }

    // ****************************************************************************************
    // ***                                   BOTTOM RIGHT                                   ***
    // ****************************************************************************************

    public void UpdateRakelYPositionLocked(bool locked)
    {
        RakelYPositionLocked = locked;
    }

    public void DoMacroAction()
    {
        //Rakel.Fill(new Paint(new Color(0 / 255f, 107 / 255f, 60 / 255f), 240), new PerlinNoiseFiller());
        Rakel.Fill(new Paint(new Color(0 / 255f, 107 / 255f, 60 / 255f), 240), new FlatFiller());

        RakelInterpolator.NewStroke();
        RakelInterpolator.AddNode(new Vector3(-5, 0, -0.10f), 0, 0, TextureResolution);

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

        RakelInterpolator.AddNode(new Vector3(6, 0, -0.10f), 0, 0, TextureResolution);
    }

    public void DoMacro2Action()
    {
        //Rakel.Fill(new Paint(new Color(0 / 255f, 107 / 255f, 60 / 255f), 240), new PerlinNoiseFiller());
        //RakelInterpolator.NewStroke();
        //RakelInterpolator.AddNode(new Vector3(-5, 0, -0.10f), 45, 0, TextureResolution);

        Rakel.Fill(new Paint(new Color(0 / 255f, 107 / 255f, 60 / 255f), 1), new FlatFiller());
        RakelInterpolator.NewStroke();
        RakelInterpolator.AddNode(new Vector3(-5, 0, -0.10f), 30, 0, TextureResolution);
    }
}