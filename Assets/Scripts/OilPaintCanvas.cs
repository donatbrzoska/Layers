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
                new CSFloats4("Value", value),
                new CSTexture("Target", texture)
            },
            null,
            new List<ComputeBuffer>(),
            false
        );

        cst.Run();
    }

    public ComputeBuffer EmitPaint(
        Rakel rakel,
        ShaderRegion shaderRegion,
        TransferMapMode transferMapMode,
        float emitVolume,
        bool debugEnabled = false)
    {
        ComputeBuffer canvasEmittedPaint = new ComputeBuffer(shaderRegion.PixelCount, Paint.SizeInBytes);
        // initialize buffer to empty values (Intel does this for you, nvidia doesn't)
        Paint[] initPaint = new Paint[shaderRegion.PixelCount];
        canvasEmittedPaint.SetData(initPaint);

        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSInts2("CalculationPosition", shaderRegion.CalculationPosition),
            new CSInts2("CanvasReservoirSize", WorldSpaceCanvas.TextureSize),
            new CSInt("CanvasResolution", WorldSpaceCanvas.Resolution),
            new CSFloats3("CanvasPosition", WorldSpaceCanvas.Position),
            new CSFloats2("CanvasSize", WorldSpaceCanvas.Size),
            new CSFloats3("RakelAnchor", rakel.Anchor),
            new CSFloats3("RakelPosition", rakel.Position),
            new CSFloat("RakelRotation", rakel.Rotation),
            new CSFloats3("RakelULTilted", rakel.ulTilted),
            new CSFloats3("RakelURTilted", rakel.urTilted),
            new CSFloats3("RakelLLTilted", rakel.llTilted),
            new CSFloats3("RakelLRTilted", rakel.lrTilted),
            new CSComputeBuffer("CanvasReservoir", Reservoir.Buffer),
            new CSComputeBuffer("CanvasEmittedPaint", canvasEmittedPaint),
            new CSInts2("RakelReservoirSize", rakel.ApplicationReservoir.Size),
            new CSInt("RakelResolution", rakel.ApplicationReservoir.Resolution),
            new CSInt("TransferMapMode", (int)transferMapMode),
            new CSFloat("EmitVolume", emitVolume),
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

        return canvasEmittedPaint;
    }

    public void ApplyPaint(
        ShaderRegion shaderRegion,
        ComputeBuffer rakelEmittedPaint,
        bool debugEnabled = false)
    {
        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSInts2("CalculationPosition", shaderRegion.CalculationPosition),
            new CSComputeBuffer("RakelEmittedPaint", rakelEmittedPaint),
            new CSComputeBuffer("CanvasReservoir", Reservoir.Buffer),
            new CSInt("TextureWidth", Texture.width)
        };

        ComputeShaderTask cst = new ComputeShaderTask(
            "ApplyBufferToCanvasShader",
            shaderRegion,
            attributes,
            null,
            new List<ComputeBuffer>() { rakelEmittedPaint },
            debugEnabled
        );

        ComputeShaderEngine.EnqueueOrRun(cst);
    }

    public void UpdateColorTexture(
        ShaderRegion shaderRegion,
        bool debugEnabled = false)
    {
        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSInts2("CalculationPosition", shaderRegion.CalculationPosition),
            new CSComputeBuffer("CanvasReservoir", Reservoir.Buffer),
            new CSInts2("TextureSize", WorldSpaceCanvas.TextureSize),
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
            new CSInts2("CalculationPosition", shaderRegion.CalculationPosition),
            new CSComputeBuffer("CanvasReservoir", Reservoir.Buffer),
            new CSInts2("TextureSize", WorldSpaceCanvas.TextureSize),
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
