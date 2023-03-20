using System.Collections.Generic;
using UnityEngine;

public class OilPaintCanvas : ComputeShaderCreator
{
    public WorldSpaceCanvas WorldSpaceCanvas { get; private set; }
    public Reservoir Reservoir { get; private set; }
    public RenderTexture Texture { get; private set; }
    public RenderTexture NormalMap { get; private set; }

    public OilPaintCanvas(int textureResolution, ShaderRegionFactory shaderRegionFactory, ComputeShaderEngine computeShaderEngine)
        : base(shaderRegionFactory, computeShaderEngine)
    {
        Renderer renderer = GameObject.Find("Canvas").GetComponent<Renderer>();
        float width = GameObject.Find("Canvas").GetComponent<Transform>().localScale.x * 10; // convert scale attribute to world space
        float height = GameObject.Find("Canvas").GetComponent<Transform>().localScale.y * 10; // convert scale attribute to world space
        Vector3 position = GameObject.Find("Canvas").GetComponent<Transform>().position;

        WorldSpaceCanvas = new WorldSpaceCanvas(height, width, textureResolution, position);

        Reservoir = new Reservoir(textureResolution, WorldSpaceCanvas.TextureSize.x, WorldSpaceCanvas.TextureSize.y, shaderRegionFactory, computeShaderEngine);

        Texture = new RenderTexture(WorldSpaceCanvas.TextureSize.x, WorldSpaceCanvas.TextureSize.y, 1);
        Texture.filterMode = FilterMode.Point;
        Texture.enableRandomWrite = true;
        Texture.Create();
        renderer.material.SetTexture("_MainTex", Texture);

        NormalMap = new RenderTexture(WorldSpaceCanvas.TextureSize.x, WorldSpaceCanvas.TextureSize.y, 1);
        NormalMap.filterMode = FilterMode.Point;
        NormalMap.enableRandomWrite = true;
        NormalMap.Create();
        renderer.material.EnableKeyword("_NORMALMAP");
        renderer.material.SetTexture("_BumpMap", NormalMap);

        InitializeTexture(Texture, Vector4.one);
        InitializeTexture(NormalMap, (new Vector4(0, 0, 1, 0) + Vector4.one) / 2);
    }

    private void InitializeTexture(RenderTexture texture, Vector4 value)
    {
        ShaderRegion sr = ShaderRegionFactory.Create(
            new Vector2Int(texture.height-1, 0),
            new Vector2Int(texture.height-1, texture.width-1),
            new Vector2Int(0, 0),
            new Vector2Int(texture.width-1, 0)
        );

        ComputeShaderTask cst = new ComputeShaderTask(
            "SetTextureShader",
            sr,
            new List<CSAttribute>() {
                new CSFloat4("Value", value),
                new CSTexture("Target", texture)
            },
            null,
            new List<ComputeBuffer>(),
            false
        );

        cst.Run();
    }

    public void EmitPaint(
        Rakel rakel,
        ShaderRegion shaderRegion,
        TransferMapMode transferMapMode,
        float emitVolume,
        bool debugEnabled = false)
    {
        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSInt2("CalculationPosition", shaderRegion.CalculationPosition),
            new CSInt2("CanvasReservoirSize", WorldSpaceCanvas.TextureSize),
            new CSInt("CanvasReservoirResolution", WorldSpaceCanvas.Resolution),
            new CSComputeBuffer("CanvasReservoir", Reservoir.Buffer),
            new CSInt("CRReadIndex", Reservoir.ReadIndex),
            new CSInt("CRWriteIndex", Reservoir.WriteIndex),
            new CSFloat3("CanvasPosition", WorldSpaceCanvas.Position),
            new CSFloat2("CanvasSize", WorldSpaceCanvas.Size),
            new CSFloat3("RakelAnchor", rakel.Anchor),
            new CSFloat3("RakelPosition", rakel.Position),
            new CSFloat("RakelRotation", rakel.Rotation),
            new CSFloat3("RakelULTilted", rakel.ulTilted),
            new CSFloat3("RakelURTilted", rakel.urTilted),
            new CSFloat3("RakelLLTilted", rakel.llTilted),
            new CSFloat3("RakelLRTilted", rakel.lrTilted),
            new CSInt2("RakelReservoirSize", rakel.ApplicationReservoir.Size),
            new CSInt("RakelReservoirResolution", rakel.ApplicationReservoir.Resolution),
            new CSComputeBuffer("RakelPickupReservoir", rakel.PickupReservoir.Buffer),
            new CSInt("RRReadIndex", rakel.ApplicationReservoir.ReadIndex),
            new CSInt("RRWriteIndex", rakel.ApplicationReservoir.WriteIndex),
            new CSInt("TransferMapMode", (int)transferMapMode),
            new CSFloat("EmitVolume", emitVolume)
        };

        ComputeShaderTask cst = new ComputeShaderTask(
            "EmitFromCanvasShader",
            shaderRegion,
            attributes,
            null,
            new List<ComputeBuffer>(),
            debugEnabled
        );

        ComputeShaderEngine.EnqueueOrRun(cst);

        Reservoir.ToggleDoubleBuffering();
        rakel.ApplicationReservoir.ToggleDoubleBuffering();
        rakel.PickupReservoir.ToggleDoubleBuffering();
    }

    public void UpdateColorTexture(
        ShaderRegion shaderRegion,
        bool debugEnabled = false)
    {
        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSInt2("CalculationPosition", shaderRegion.CalculationPosition),
            new CSComputeBuffer("CanvasReservoir", Reservoir.Buffer),
            new CSInt("CRReadIndex", Reservoir.ReadIndex),
            new CSInt2("TextureSize", WorldSpaceCanvas.TextureSize),
            new CSTexture("CanvasTexture", Texture)
        };

        ComputeShaderTask cst = new ComputeShaderTask(
            "ColorsShader",
            shaderRegion,
            attributes,
            null,
            new List<ComputeBuffer>(),
            debugEnabled
        );

        ComputeShaderEngine.EnqueueOrRun(cst);
    }

    public void UpdateNormalMap(
        ShaderRegion shaderRegion,
        bool debugEnabled = false)
    {
        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSInt2("CalculationPosition", shaderRegion.CalculationPosition),
            new CSComputeBuffer("CanvasReservoir", Reservoir.Buffer),
            new CSInt("CRReadIndex", Reservoir.ReadIndex),
            new CSInt2("TextureSize", WorldSpaceCanvas.TextureSize),
            new CSTexture("NormalMap", NormalMap)
        };

        ComputeShaderTask cst = new ComputeShaderTask(
            "NormalsShader",
            shaderRegion,
            attributes,
            null,
            new List<ComputeBuffer>(),
            debugEnabled
        );

        ComputeShaderEngine.EnqueueOrRun(cst);
    }

    public void Dispose()
    {
        Reservoir.Dispose();
    }
}
