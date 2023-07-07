using System.Collections.Generic;
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

    private Vector2Int RESERVOIR_PIXEL_PICKUP_RADIUS = new Vector2Int(1, 1);

    // It is assumed that the canvas is perpendicular to the z axis
    // Position is the center of the canvas
    public Canvas_(float width, float height, int layers, float cellVolume, int diffuseDepth, float diffuseRatio, Vector3 position, int textureResolution, float normalScale, ColorSpace colorSpace)
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

        Reservoir = new Reservoir(textureResolution, TextureSize.x, TextureSize.y, layers, cellVolume, diffuseDepth, diffuseRatio);

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

        new ComputeShaderTask(
            "SetTexture",
            sr,
            new List<CSAttribute>()
            {
                new CSFloat4("Value", value),
                new CSTexture("Target", texture)
            },
            false
        ).Run();
    }

    public PaintGrid EmitPaint(
        Rakel rakel,
        ShaderRegion shaderRegion,
        //float pickupDistance_MAX,
        float pickupVolume_MIN
        //float pickupVolume_MAX,
        )
    {
        ComputeBuffer canvasMappedInfo = new ComputeBuffer(shaderRegion.PixelCount, MappedInfo.SizeInBytes);
        MappedInfo[] canvasMappedInfoData = new MappedInfo[shaderRegion.PixelCount];
        canvasMappedInfo.SetData(canvasMappedInfoData);

        new ComputeShaderTask(
            "Pickup/TransformToRakelPosition",
            shaderRegion,
            new List<CSAttribute>()
            {
                new CSInt("TextureResolution", Resolution),

                new CSComputeBuffer("RakelInfo", rakel.InfoBuffer),

                new CSComputeBuffer("CanvasMappedInfo", canvasMappedInfo),
            },
            false
        ).Run();

        new ComputeShaderTask(
            "Pickup/CanvasReservoirPixel",
            shaderRegion,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("RakelInfo", rakel.InfoBuffer),

                new CSFloat3("CanvasPosition", Position),
                new CSFloat2("CanvasSize", Size),
                new CSInt3("CanvasReservoirSize", Reservoir.Size),

                new CSComputeBuffer("CanvasMappedInfo", canvasMappedInfo),
            },
            false
        ).Run();

        new ComputeShaderTask(
            "Pickup/DistanceFromCanvas",
            shaderRegion,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("RakelInfo", rakel.InfoBuffer),

                new CSFloat3("CanvasPosition", Position),

                new CSComputeBuffer("CanvasMappedInfo", canvasMappedInfo),
            },
            false
        ).Run();

        new ComputeShaderTask(
            "Pickup/Overlap",
            shaderRegion,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("RakelInfo", rakel.InfoBuffer),
                new CSInt2("ReservoirPixelPickupRadius", RESERVOIR_PIXEL_PICKUP_RADIUS),
                new CSComputeBuffer("CanvasMappedInfo", canvasMappedInfo),
            },
            false
        ).Run();

        new ComputeShaderTask(
            "Pickup/VolumeToPickup",
            shaderRegion,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("RakelInfo", rakel.InfoBuffer),
                new CSComputeBuffer("RakelReservoirInfoDuplicate", rakel.Reservoir.PaintGridDuplicate.Info),
                new CSInt3("RakelReservoirSize", rakel.Reservoir.Size),
                new CSInt2("ReservoirPixelPickupRadius", RESERVOIR_PIXEL_PICKUP_RADIUS),
                new CSComputeBuffer("CanvasMappedInfo", canvasMappedInfo),

                new CSFloat3("CanvasPosition", Position),
                new CSComputeBuffer("CanvasReservoirInfoDuplicate", Reservoir.PaintGridDuplicate.Info),
                new CSInt3("CanvasReservoirSize", Reservoir.Size),

                //new CSFloat("RakelTilt_MAX", Rakel.MAX_SUPPORTED_TILT),
                //new CSFloat("PickupDistance_MAX", pickupDistance_MAX),
                new CSFloat("PickupVolume_MIN", pickupVolume_MIN),
                //new CSFloat("PickupVolume_MAX", pickupVolume_MAX),
            },
            false
        ).Run();

        // HACK canvasEmittedPaint is actually treated as a raw stack with no specified mixing parameters
        float UNUSED = 0;
        PaintGrid canvasEmittedPaint = new PaintGrid(new Vector3Int(shaderRegion.Size.x, shaderRegion.Size.y, Reservoir.Size.z), UNUSED, (int) UNUSED, UNUSED);

        Vector2Int reservoirPixelPickupArea = new Vector2Int(RESERVOIR_PIXEL_PICKUP_RADIUS.x * 2 + 1, RESERVOIR_PIXEL_PICKUP_RADIUS.y * 2 + 1);

        new ComputeShaderTask(
            "Pickup/EmitFromCanvas",
            shaderRegion,
            reservoirPixelPickupArea,
            new List<CSAttribute>()
            {
                new CSInt2("ReservoirPixelPickupRadius", RESERVOIR_PIXEL_PICKUP_RADIUS),
                new CSComputeBuffer("CanvasMappedInfo", canvasMappedInfo),

                new CSComputeBuffer("CanvasReservoirInfo", Reservoir.PaintGrid.Info),
                new CSComputeBuffer("CanvasReservoirContent", Reservoir.PaintGrid.Content),
                new CSComputeBuffer("CanvasReservoirInfoDuplicate", Reservoir.PaintGridDuplicate.Info),
                new CSComputeBuffer("CanvasReservoirContentDuplicate", Reservoir.PaintGridDuplicate.Content),
                new CSInt3("CanvasReservoirSize", Reservoir.Size),
                new CSFloat("CanvasReservoirCellVolume", Reservoir.PaintGrid.CellVolume),

                new CSComputeBuffer("CanvasEmittedPaintInfo", canvasEmittedPaint.Info),
                new CSComputeBuffer("CanvasEmittedPaintContent", canvasEmittedPaint.Content),
                new CSInt3("CanvasEmittedPaintSize", canvasEmittedPaint.Size)
            },
            false
        ).Run();

        canvasMappedInfo.Dispose();

        return canvasEmittedPaint;
    }

    public void ApplyPaint(
        ShaderRegion shaderRegion,
        PaintGrid rakelEmittedPaint)
    {
        new ComputeShaderTask(
            "Emit/ApplyBufferToCanvas",
            shaderRegion,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("RakelEmittedPaintInfo", rakelEmittedPaint.Info),
                new CSComputeBuffer("RakelEmittedPaintContent", rakelEmittedPaint.Content),
                new CSInt3("RakelEmittedPaintSize", rakelEmittedPaint.Size),

                new CSComputeBuffer("CanvasReservoirInfo", Reservoir.PaintGrid.Info),
                new CSComputeBuffer("CanvasReservoirContent", Reservoir.PaintGrid.Content),
                new CSInt3("CanvasReservoirSize", Reservoir.Size),
                new CSFloat("CanvasReservoirCellVolume", Reservoir.PaintGrid.CellVolume),
                new CSInt("CanvasReservoirDiffuseDepth", Reservoir.PaintGrid.DiffuseDepth),
                new CSFloat("CanvasReservoirDiffuseRatio", Reservoir.PaintGrid.DiffuseRatio),
            },
            false
        ).Run();

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
        new ComputeShaderTask(
            "Render",
            shaderRegion,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("CanvasReservoirInfo", Reservoir.PaintGrid.Info),
                new CSComputeBuffer("CanvasReservoirContent", Reservoir.PaintGrid.Content),
                new CSInt3("CanvasReservoirSize", Reservoir.Size),
                new CSInt("ColorSpace", (int) ColorSpace),

                new CSTexture("Texture", Texture),
                new CSTexture("NormalMap", NormalMap),
                new CSFloat("NormalScale", NormalScale),
            },
            debugEnabled
        ).Run();
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
