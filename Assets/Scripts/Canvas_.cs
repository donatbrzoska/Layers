﻿using System.Collections.Generic;
using UnityEngine;

public class Canvas_
{
    public Vector2 Size { get; private set; }
    // TODO It is not tested, whether positions other than the default (0,0,0) actually fully work
    public Vector3 Position { get; private set; }
    private float XMin;
    private float XMax;
    private float YMin;
    private float YMax;
    public int Resolution { get; private set; }
    public Vector2Int TextureSize { get; private set; }

    public Reservoir Reservoir { get; private set; }
    public RenderTexture Texture { get; private set; }
    public RenderTexture NormalMap { get; private set; }

    public float NormalScale { get; set; }

    private ColorSpace ColorSpace;

    // It is assumed that the canvas is perpendicular to the z axis
    // Position is the center of the canvas
    public Canvas_(float width, float height, Vector3 position, int textureResolution, float normalScale, ColorSpace colorSpace)
    {
        Size = new Vector2(width, height);
        Position = position;
        XMin = Position.x - width / 2;
        XMax = Position.x + width / 2;
        YMin = Position.y - height / 2;
        YMax = Position.y + height / 2;

        Resolution = textureResolution;
        TextureSize = new Vector2Int(
            (int)(Resolution * Size.x),
            (int)(Resolution * Size.y)
        );

        NormalScale = normalScale;
        ColorSpace = colorSpace;

        Reservoir = new Reservoir(textureResolution, TextureSize.x, TextureSize.y);

        Texture = new RenderTexture(TextureSize.x, TextureSize.y, 1);
        Texture.filterMode = FilterMode.Point;
        Texture.enableRandomWrite = true;
        Texture.Create();

        NormalMap = new RenderTexture(TextureSize.x, TextureSize.y, 1);
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

        WriteCanvasMappedInfo(canvasEmittedPaint, rakel, shaderRegion, debugEnabled);

        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSInt("TextureResolution", Resolution),

            new CSFloat("RakelLength", rakel.Length),
            new CSFloat("RakelWidth", rakel.Width),
            new CSFloat3("RakelPosition", rakel.Position),
            new CSFloat3("RakelAnchor", rakel.Anchor),
            new CSFloat("RakelRotation", rakel.Rotation),
            new CSFloat("RakelTilt", rakel.Tilt),
            new CSFloat("RakelEdgeZ", rakel.LowerLeft.z),
            new CSComputeBuffer("RakelApplicationReservoir", rakel.ApplicationReservoir.Buffer),
            new CSComputeBuffer("RakelPickupReservoir", rakel.PickupReservoir.Buffer),
            new CSInt2("RakelReservoirSize", rakel.ApplicationReservoir.Size),

            new CSFloat3("CanvasPosition", Position),
            new CSFloat2("CanvasSize", Size),
            new CSComputeBuffer("CanvasReservoir", Reservoir.Buffer),
            new CSInt2("CanvasReservoirSize", TextureSize),

            new CSFloat("RakelTilt_MAX", Rakel.MAX_SUPPORTED_TILT),
            new CSFloat("PickupDistance_MAX", pickupDistance_MAX),
            new CSFloat("PickupVolume_MIN", pickupVolume_MIN),
            new CSFloat("PickupVolume_MAX", pickupVolume_MAX),

            new CSComputeBuffer("CanvasEmittedPaint", canvasEmittedPaint),
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

    private void WriteCanvasMappedInfo(
        ComputeBuffer canvasMappedInfoTarget,
        Rakel rakel,
        ShaderRegion shaderRegion,
        bool debugEnabled = false)
    {
        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSInt("TextureResolution", Resolution),

            new CSFloat("RakelLength", rakel.Length),
            new CSFloat("RakelWidth", rakel.Width),
            new CSFloat3("RakelPosition", rakel.Position),
            new CSFloat3("RakelAnchor", rakel.Anchor),
            new CSFloat("RakelRotation", rakel.Rotation),
            new CSFloat("RakelTilt", rakel.Tilt),

            new CSFloat3("CanvasPosition", Position),
            new CSFloat2("CanvasSize", Size),
            new CSInt2("CanvasReservoirSize", TextureSize),

            new CSComputeBuffer("CanvasMappedInfoTarget", canvasMappedInfoTarget),
        };

        ComputeShaderTask cst = new ComputeShaderTask(
            "CanvasMappedInfo",
            shaderRegion,
            attributes,
            debugEnabled
        );

        cst.Run();
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
            new Vector2Int(TextureSize.x, 0),
            new Vector2Int(0, TextureSize.y),
            new Vector2Int(TextureSize.x, TextureSize.y));
    }

    public void Render(
        ShaderRegion shaderRegion,
        bool debugEnabled = false)
    {
        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSComputeBuffer("CanvasReservoir", Reservoir.Buffer),
            new CSInt("ColorSpace", (int) ColorSpace),

            new CSInt2("TextureSize", TextureSize),
            new CSTexture("Texture", Texture),
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

    public Vector3 AlignToPixelGrid(Vector3 point)
    {
        Vector2Int pixel = MapToPixel(point);
        Vector3 gridAlignedIncomplete = MapToWorldSpace(pixel);
        gridAlignedIncomplete.z = point.z;
        return gridAlignedIncomplete;
    }

    private Vector3 MapToWorldSpace(Vector2Int pixel)
    {
        float pixelSize = 1 / (float)Resolution;
        Vector3 positiveCanvasAligned = new Vector3(0.5f * pixelSize + pixel.x * pixelSize,
                                                    0.5f * pixelSize + pixel.y * pixelSize,
                                                    0);

        Vector3 canvasLowerLeft = Position - new Vector3(Size.x / 2, Size.y / 2, 0);
        Vector3 canvasAligned = positiveCanvasAligned + canvasLowerLeft;

        return canvasAligned;
    }

    public Vector2Int MapToPixel(Vector3 worldSpace)
    {
        Vector3 lowerLeftOriented = worldSpace + new Vector3(Size.x / 2, Size.y / 2, 0) - Position;
        // really lowerLeftOriented / pixelSize, but that is lowerLeftOriented / (1/Resolution)
        Vector2 floatPixel = lowerLeftOriented * Resolution;
        return new Vector2Int((int)Mathf.Ceil(floatPixel.x), (int)Mathf.Ceil(floatPixel.y)) - new Vector2Int(1, 1);
    }

    public Vector2Int MapToPixelInRange(Vector3 worldSpace)
    {
        bool yTopOOB = worldSpace.y > YMax;
        if (yTopOOB)
            worldSpace.y = YMax;

        bool yBottomOOB = worldSpace.y <= YMin;
        if (yBottomOOB)
            worldSpace.y = YMin + 0.0001f;

        bool yRightOOB = worldSpace.x > XMax;
        if (yRightOOB)
            worldSpace.x = XMax;

        bool xLeftOOB = worldSpace.x <= XMin;
        if (xLeftOOB)
            worldSpace.x = XMin + 0.0001f;

        return MapToPixel(worldSpace);
    }
}
