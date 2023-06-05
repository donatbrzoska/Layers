using System.Collections.Generic;
using UnityEngine;

public class Canvas_
{
    public WorldSpaceCanvas WorldSpaceCanvas { get; private set; }
    public Reservoir Reservoir { get; private set; }
    public RenderTexture Texture { get; private set; }
    public RenderTexture NormalMap { get; private set; }

    public float NormalScale { get; set; }

    public Canvas_(int textureResolution, float normalScale)
    {
        NormalScale = normalScale;

        float width = GameObject.Find("Canvas").GetComponent<Transform>().localScale.x * 10; // convert scale attribute to world space
        float height = GameObject.Find("Canvas").GetComponent<Transform>().localScale.y * 10; // convert scale attribute to world space
        Vector3 position = GameObject.Find("Canvas").GetComponent<Transform>().position;

        WorldSpaceCanvas = new WorldSpaceCanvas(height, width, textureResolution, position);

        Reservoir = new Reservoir(textureResolution, WorldSpaceCanvas.TextureSize.x, WorldSpaceCanvas.TextureSize.y);

        Texture = new RenderTexture(WorldSpaceCanvas.TextureSize.x, WorldSpaceCanvas.TextureSize.y, 1);
        Texture.filterMode = FilterMode.Point;
        Texture.enableRandomWrite = true;
        Texture.Create();

        NormalMap = new RenderTexture(WorldSpaceCanvas.TextureSize.x, WorldSpaceCanvas.TextureSize.y, 1);
        NormalMap.filterMode = FilterMode.Point;
        NormalMap.enableRandomWrite = true;
        NormalMap.Create();

        InitializeTexture(Texture, Vector4.one);
        InitializeTexture(NormalMap, (new Vector4(0, 0, 1, 0) + Vector4.one) / 2);
    }

    private void InitializeTexture(RenderTexture texture, Vector4 value)
    {
        ShaderRegion sr = new ShaderRegion(
            new Vector2Int(texture.height-1, 0),
            new Vector2Int(texture.height-1, texture.width-1),
            new Vector2Int(0, 0),
            new Vector2Int(texture.width-1, 0)
        );

        ComputeShaderTask cst = new ComputeShaderTask(
            "SetTexture",
            sr,
            new List<CSAttribute>() {
                new CSFloat4("Value", value),
                new CSTexture("Target", texture)
            },
            false
        );

        cst.Run();
    }

    public ComputeBuffer EmitPaint(
        Rakel rakel,
        ShaderRegion shaderRegion,
        float pickupDistance_MAX,
        float pickupVolume_MIN,
        float pickupVolume_MAX,
        bool debugEnabled = false)
    {
        ComputeBuffer canvasEmittedPaint = new ComputeBuffer(shaderRegion.PixelCount, Paint.SizeInBytes);
        // initialize buffer to empty values (Intel does this for you, nvidia doesn't)
        Paint[] initPaint = new Paint[shaderRegion.PixelCount];
        canvasEmittedPaint.SetData(initPaint);

        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSInt2("CanvasReservoirSize", WorldSpaceCanvas.TextureSize),
            new CSFloat3("CanvasPosition", WorldSpaceCanvas.Position),
            new CSFloat2("CanvasSize", WorldSpaceCanvas.Size),
            new CSInt("TextureResolution", WorldSpaceCanvas.Resolution),
            new CSFloat3("RakelAnchor", rakel.Anchor),
            new CSFloat3("RakelPosition", rakel.Position),
            new CSFloat("RakelRotation", rakel.Rotation),
            new CSFloat("RakelLength", rakel.Length),
            new CSFloat("RakelWidth", rakel.Width),
            new CSFloat("RakelTilt", rakel.Tilt),
            new CSFloat("RakelTilt_MAX", Rakel.MAX_SUPPORTED_TILT),
            new CSFloat3("RakelULTilted", rakel.ulTilted),
            new CSFloat3("RakelURTilted", rakel.urTilted),
            new CSFloat3("RakelLLTilted", rakel.llTilted),
            new CSFloat3("RakelLRTilted", rakel.lrTilted),
            new CSComputeBuffer("CanvasReservoir", Reservoir.Buffer),
            new CSComputeBuffer("CanvasEmittedPaint", canvasEmittedPaint),
            new CSInt2("RakelReservoirSize", rakel.ApplicationReservoir.Size),
            new CSInt("RakelResolution", rakel.ApplicationReservoir.Resolution),
            new CSFloat("PickupDistance_MAX", pickupDistance_MAX),
            new CSFloat("PickupVolume_MIN", pickupVolume_MIN),
            new CSFloat("PickupVolume_MAX", pickupVolume_MAX),
        };

        ComputeShaderTask cst = new ComputeShaderTask(
            "EmitFromCanvas",
            shaderRegion,
            new Vector2Int(3, 3),
            attributes,
            debugEnabled
        );

        cst.Run();

        return canvasEmittedPaint;
    }

    public void ApplyPaint(
        ShaderRegion shaderRegion,
        ComputeBuffer rakelEmittedPaint,
        bool debugEnabled = false)
    {
        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSComputeBuffer("RakelEmittedPaint", rakelEmittedPaint),
            new CSComputeBuffer("CanvasReservoir", Reservoir.Buffer),
            new CSInt("TextureWidth", Texture.width)
        };

        ComputeShaderTask cst = new ComputeShaderTask(
            "ApplyBufferToCanvas",
            shaderRegion,
            attributes,
            debugEnabled
        );

        cst.Run();

        rakelEmittedPaint.Dispose();
    }

    public ShaderRegion GetFullShaderRegion()
    {
        return new ShaderRegion(
            new Vector2Int(0, 0),
            new Vector2Int(WorldSpaceCanvas.TextureSize.x, 0),
            new Vector2Int(0, WorldSpaceCanvas.TextureSize.y),
            new Vector2Int(WorldSpaceCanvas.TextureSize.x, WorldSpaceCanvas.TextureSize.y));
    }

    public void Render(
        ShaderRegion shaderRegion,
        bool debugEnabled = false)
    {
        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSComputeBuffer("CanvasReservoir", Reservoir.Buffer),
            new CSInt2("TextureSize", WorldSpaceCanvas.TextureSize),
            new CSTexture("CanvasTexture", Texture),
            new CSTexture("NormalMap", NormalMap),
            new CSFloat("NormalScale", NormalScale),
        };

        ComputeShaderTask cst = new ComputeShaderTask(
            "Render",
            shaderRegion,
            attributes,
            debugEnabled
        );

        cst.Run();
    }

    public void Dispose()
    {
        Reservoir.Dispose();
    }
}
