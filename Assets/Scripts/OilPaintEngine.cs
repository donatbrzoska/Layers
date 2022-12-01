using UnityEngine;

public class OilPaintEngine : MonoBehaviour
{
    private Camera Camera;

    public int TextureResolution { get; private set; } = 100; // texture space pixels per 1 world space
    private RenderTexture Texture;

    private int CanvasColliderID;
    private Renderer CanvasRenderer;
    private float CanvasWidth; // world space
    private float CanvasHeight; // world space
    private Vector3 CanvasPosition; // world space
    private WorldSpaceCanvas WorldSpaceCanvas;

    //private IRakelPaintReservoir RakelPaintReservoir;
    //public Paint RakelPaint { get; private set; }

    public Vector2 RakelNormal { get; private set; } = new Vector2(1, 0);
    public float RakelLength { get; private set; } = 8f; // world space
    public float RakelWidth { get; private set; } = 0.3f; // world space
    private IRakel Rakel;

    private RakelInterpolator RakelInterpolator;


    void Awake()
    {
        Camera = GameObject.Find("Main Camera").GetComponent<Camera>();

        CanvasColliderID = GameObject.Find("Canvas").GetComponent<MeshCollider>().GetInstanceID();
        CanvasRenderer = GameObject.Find("Canvas").GetComponent<Renderer>();
        CanvasWidth = GameObject.Find("Canvas").GetComponent<Transform>().localScale.x * 10; // convert scale attribute to world space
        CanvasHeight = GameObject.Find("Canvas").GetComponent<Transform>().localScale.y * 10; // convert scale attribute to world space
        CanvasPosition = GameObject.Find("Canvas").GetComponent<Transform>().position;

        CreateCanvasAndTools();
    }

    void Start()
    {
    }

    void CreateCanvasAndTools()
    {
        WorldSpaceCanvas = new WorldSpaceCanvas(CanvasHeight, CanvasWidth, TextureResolution, CanvasPosition);

        Texture = new RenderTexture(WorldSpaceCanvas.PixelsX, WorldSpaceCanvas.PixelsY, 1);
        Texture.filterMode = FilterMode.Point;
        Texture.enableRandomWrite = true;
        Texture.Create();
        CanvasRenderer.material.SetTexture("_MainTex", Texture);

        //NormalMap = new FastTexture2D(TextureWidth, TextureHeight);
        //CanvasRenderer.material.EnableKeyword("_NORMALMAP");
        //CanvasRenderer.material.SetTexture("_BumpMap", NormalMap.Texture);

        CreateRakelDrawer();
    }

    void CreateRakelDrawer()
    {
        //RakelPaintReservoir = new RakelPaintReservoir(
        //    WorldSpaceLengthToTextureSpaceLength(RakelLength, TextureResolution),
        //    WorldSpaceLengthToTextureSpaceLength(RakelWidth, TextureResolution));

        Rakel = new Rakel(RakelLength, RakelWidth, TextureResolution, WorldSpaceCanvas);
        int rakelPixelsLength = MathUtil.ToTextureSpaceLength(RakelLength, TextureResolution);
        int rakelPixelsWidth = MathUtil.ToTextureSpaceLength(RakelWidth, TextureResolution);
        Debug.Log("Rakel is " + rakelPixelsLength + "x" + rakelPixelsWidth + " = " + rakelPixelsLength * rakelPixelsWidth);

        RakelInterpolator = new RakelInterpolator(Rakel, Texture);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 worldSpaceHit = InputUtil.GetMouseHit(Camera, CanvasColliderID);
        if (!worldSpaceHit.Equals(Vector3.negativeInfinity))
        {
            if (Input.GetMouseButtonDown(0))
            {
                RakelInterpolator.NewStroke();
            }
            RakelInterpolator.AddNode(worldSpaceHit, 0, 0, TextureResolution / 2);
        }
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

    public void UpdateRakelNormal(Vector2 normal)
    {
        RakelNormal = normal;
    }

    //public void UpdateRakelPaint(Paint paint)
    //{
    //    RakelPaint = paint;
    //    RakelPaintReservoir.Fill(paint);
    //}

    public void UpdateTextureResolution(int pixelsPerWorldSpaceUnit)
    {
        TextureResolution = pixelsPerWorldSpaceUnit;
        CreateCanvasAndTools();
    }
}