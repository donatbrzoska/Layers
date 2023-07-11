using UnityEngine;
using System.Collections.Generic;

public struct RakelInfo
{
    public const int SizeInBytes = 8 * sizeof(float) + 10 * 3 * sizeof(float);

    public float Length;
    public float Width;

    public Vector3 Anchor;

    public Vector3 Position;
    public int AutoZEnabled;
    public float PositionBaseZ;
    public float Pressure;
    public float Rotation;
    public float Tilt;

    public float EdgeZ;

    public Vector3 UpperLeft;
    public Vector3 UpperRight;
    public Vector3 LowerLeft;
    public Vector3 LowerRight;

    public Vector3 ULTilted;
    public Vector3 URTilted;
    public Vector3 LLTilted;
    public Vector3 LRTilted;
}

public class Rakel
{
    // see EmitFromRakel shader for details (look for "79 degree tilt")
    private const int MIN_SUPPORTED_TILT = 0;
    public const int MAX_SUPPORTED_TILT = 79;

    public static float ClampTilt(float tilt)
    {
        return Mathf.Clamp(tilt, MIN_SUPPORTED_TILT, MAX_SUPPORTED_TILT);
    }

    private bool StrokeBegin;

    public ComputeBuffer InfoBuffer;
    public RakelInfo Info;
    ComputeBuffer ReducedCanvasVolume;
    ComputeBuffer ReducedRakelVolume;

    public Reservoir Reservoir;

    private ComputeBuffer DistortionMap;
    private int DistortionMapIndex;
    private const int MAX_STROKE_LENGTH = 3000; // should always be bigger than Rakel reservoir size.x
    private Vector2Int DistortionMapSize;

    private Vector2Int ReservoirPixelEmitRadius;
    private Vector2Int DELETE_CONFLICT_AREA = new Vector2Int(3,3);

    public Rakel(float length, float width, int resolution, int layers_MAX, float cellVolume, float anchorRatioLength = 0.5f, float anchorRatioWidth = 1)
    {
        Vector3Int reservoirSize = new Vector3Int((int)(width * resolution), (int)(length * resolution), layers_MAX);

        Reservoir = new Reservoir(
            resolution,
            reservoirSize.x,
            reservoirSize.y,
            reservoirSize.z,
            cellVolume);

        DistortionMapSize = new Vector2Int(MAX_STROKE_LENGTH, reservoirSize.y);
        DistortionMap = new ComputeBuffer(DistortionMapSize.x * DistortionMapSize.y , sizeof(float));

        // make sure Rakel is not bigger than its reservoir
        Info.Length = Reservoir.Size.y * Reservoir.PixelSize;
        Info.Width = Reservoir.Size.x * Reservoir.PixelSize;

        // NOTE this has to be set after Width and Length were corrected
        Info.Anchor = new Vector3(anchorRatioWidth * Info.Width, anchorRatioLength * Info.Length, 0);

        InfoBuffer = new ComputeBuffer(1, RakelInfo.SizeInBytes);
        InfoBuffer.SetData(new RakelInfo[] { Info });

        ReducedCanvasVolume = new ComputeBuffer(1, sizeof(float));
        ReducedRakelVolume = new ComputeBuffer(1, sizeof(float));
        float[] reducedVolumeData = new float[1];
        ReducedCanvasVolume.SetData(reducedVolumeData);
        ReducedRakelVolume.SetData(reducedVolumeData);
    }

    public void NewStroke()
    {
        StrokeBegin = true;

        //float[] distortionMapData = new float[DistortionMapSize.x * DistortionMapSize.y];

        ////float noiseCapRatio = 0.6f;
        ////float noiseCapSubtract = 0.5f;

        //PerlinNoise perlinNoise = new PerlinNoise(DistortionMapSize, new Vector2(10, 800));
        //for (int x = 0; x < DistortionMapSize.x; x++)
        //{
        //    for (int y = 0; y < DistortionMapSize.y; y++)
        //    {
        //        float value = perlinNoise.ValueAt(x, y);
        //        //distortionMapData[IndexUtil.XY(x, y, DistortionMapSize.x)] = Mathf.Clamp01(Mathf.Clamp(value, 0, noiseCapRatio) - noiseCapSubtract);
        //        distortionMapData[IndexUtil.XY(x, y, DistortionMapSize.x)] = value;
        //    }
        //}

        //DistortionMap.SetData(distortionMapData);
    }

    private int IncrementDistortionMapIndex()
    {
        int oldValue = DistortionMapIndex;

        DistortionMapIndex++;
        // wrap around
        if (DistortionMapIndex > DistortionMapSize.x - Reservoir.Size.x - 1)
        {
            DistortionMapIndex = 0;
        }

        return oldValue;
    }

    public void UpdateState(Vector3 position, float baseSink_MAX, float tiltSink_MAX, int autoZEnabled, int zZero, float pressure, float rotation, float tilt)
    {
        // Update info on CPU for rakel rendering
        Info.Position = position;
        Info.AutoZEnabled = autoZEnabled;
        Info.Pressure = pressure;
        Info.Rotation = rotation;
        Info.Tilt = ClampTilt(Mathf.Max(Mathf.Min(tilt, MAX_SUPPORTED_TILT), MIN_SUPPORTED_TILT));

        Vector3 ulOrigin = new Vector3(0, Info.Length, 0);
        Vector3 urOrigin = new Vector3(Info.Width, Info.Length, 0);
        Vector3 llOrigin = new Vector3(0, 0, 0);
        Vector3 lrOrigin = new Vector3(Info.Width, 0, 0);

        Quaternion tiltQuaternion = Quaternion.AngleAxis(Info.Tilt, Vector3.up);
        Info.ULTilted = tiltQuaternion * (ulOrigin - Info.Anchor) + Info.Anchor; // tilt around anchor
        Info.URTilted = tiltQuaternion * (urOrigin - Info.Anchor) + Info.Anchor; // tilt around anchor
        Info.LLTilted = tiltQuaternion * (llOrigin - Info.Anchor) + Info.Anchor; // tilt around anchor
        Info.LRTilted = tiltQuaternion * (lrOrigin - Info.Anchor) + Info.Anchor; // tilt around anchor

        Quaternion rotationQuaternion = Quaternion.AngleAxis(Info.Rotation, Vector3.back);
        Vector3 ulRotated = rotationQuaternion * (Info.ULTilted - Info.Anchor) + Info.Anchor; // rotate around anchor
        Vector3 urRotated = rotationQuaternion * (Info.URTilted - Info.Anchor) + Info.Anchor; // rotate around anchor
        Vector3 llRotated = rotationQuaternion * (Info.LLTilted - Info.Anchor) + Info.Anchor; // rotate around anchor
        Vector3 lrRotated = rotationQuaternion * (Info.LRTilted - Info.Anchor) + Info.Anchor; // rotate around anchor

        Vector3 positionTranslation = Info.Position - Info.Anchor;
        Info.UpperLeft = ulRotated + positionTranslation;
        Info.UpperRight = urRotated + positionTranslation;
        Info.LowerLeft = llRotated + positionTranslation;
        Info.LowerRight = lrRotated + positionTranslation;

        float pixelSize = 1 / (float)Reservoir.Resolution;
        float pixelDiag = pixelSize * Mathf.Sqrt(2);
        float tiltedPixelShortSide = Mathf.Cos(Info.Tilt * Mathf.Deg2Rad) * pixelSize;
        ReservoirPixelEmitRadius = new Vector2Int((int)Mathf.Ceil((pixelDiag / 2) / tiltedPixelShortSide), 1);


        // Update info on GPU for paint transfer calculations
        new ComputeShaderTask(
            "RakelState/UpdateRakelState",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            new List<CSAttribute>()
            {
                new CSFloat3("Position", Info.Position),
                new CSInt("AutoZEnabled", Info.AutoZEnabled),
                new CSInt("ZZero", zZero),
                new CSFloat("Pressure", Info.Pressure),
                new CSFloat("Rotation", Info.Rotation),
                new CSFloat("Tilt", Info.Tilt),
                new CSFloat("MAX_SUPPORTED_TILT", MAX_SUPPORTED_TILT),
                new CSFloat("BaseSink_MAX", baseSink_MAX),
                new CSFloat("TiltSink_MAX", tiltSink_MAX),

                new CSComputeBuffer("RakelInfo", InfoBuffer),
            },
            false
        ).Run();
    }

    public ComputeBuffer CalculateRakelMappedInfo(
        ShaderRegion shaderRegion,
        Canvas_ canvas)
    {
        ComputeBuffer rakelMappedInfo = new ComputeBuffer(shaderRegion.PixelCount, MappedInfo.SizeInBytes);
        MappedInfo[] rakelMappedInfoData = new MappedInfo[shaderRegion.PixelCount];
        rakelMappedInfo.SetData(rakelMappedInfoData);

        new ComputeShaderTask(
            "Emit/TransformToRakelOrigin",
            shaderRegion,
            new List<CSAttribute>()
            {
                new CSInt("TextureResolution", canvas.Resolution),

                new CSFloat3("CanvasPosition", canvas.Position),
                new CSFloat2("CanvasSize", canvas.Size),

                new CSComputeBuffer("RakelInfo", InfoBuffer),

                new CSComputeBuffer("RakelMappedInfo", rakelMappedInfo),
            },
            false
        ).Run();

        new ComputeShaderTask(
            "Emit/RakelReservoirPixel",
            shaderRegion,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("RakelInfo", InfoBuffer),
                new CSInt3("RakelReservoirSize", Reservoir.Size),

                new CSComputeBuffer("RakelMappedInfo", rakelMappedInfo),
            },
            false
        ).Run();

        new ComputeShaderTask(
            "Emit/Overlap",
            shaderRegion,
            new List<CSAttribute>()
            {
                new CSInt("TextureResolution", canvas.Resolution),

                new CSComputeBuffer("RakelInfo", InfoBuffer),

                new CSInt2("ReservoirPixelEmitRadius", ReservoirPixelEmitRadius),
                new CSComputeBuffer("RakelMappedInfo", rakelMappedInfo),
            },
            false
        ).Run();

        return rakelMappedInfo;
    }

    public void RecalculatePositionBaseZ(
        Canvas_ canvas,
        ComputeBuffer rakelMappedInfo,
        ShaderRegion emitSR,
        float layerThickness_MAX,
        float baseSink_MAX,
        float tiltSink_MAX)
    {
        // TODO do canvas volume based on last n steps?
        if (StrokeBegin && Info.AutoZEnabled == 1)
        {
            // reduce canvas volume
            canvas.Reservoir.Duplicate();
            canvas.Reservoir.ReduceVolumeAvg(
                rakelMappedInfo,
                new Vector2Int(Reservoir.Size.x, Reservoir.Size.y),
                emitSR,
                ReducedCanvasVolume);

            // reduce rakel volume
            new ComputeShaderTask(
                "RakelState/WriteSampledRakelVolumesToDuplicate",
                emitSR,
                new List<CSAttribute>()
                {
                    new CSComputeBuffer("RakelReservoirInfo", Reservoir.PaintGrid.Info),
                    new CSInt3("RakelReservoirSize", Reservoir.Size),

                    new CSInt2("ReservoirPixelEmitRadius", ReservoirPixelEmitRadius),
                    new CSComputeBuffer("RakelMappedInfo", rakelMappedInfo),
                    new CSComputeBuffer("CanvasReservoirInfoDuplicate", canvas.Reservoir.PaintGridDuplicate.Info),
                    new CSInt3("CanvasReservoirSize", canvas.Reservoir.Size),
                },
                false
            ).Run();
            // reset Z so that distance between rakel edge and canvas is 0
            // (simplifies overshoot calculation)
            UpdateState(Info.Position, baseSink_MAX, tiltSink_MAX, Info.AutoZEnabled, 1, Info.Pressure, Info.Rotation, Info.Tilt);
            new ComputeShaderTask(
                "Emit/DistanceFromRakel",
                emitSR,
                new List<CSAttribute>()
                {
                new CSComputeBuffer("RakelInfo", InfoBuffer),

                new CSComputeBuffer("RakelMappedInfo", rakelMappedInfo),
                },
                false
            ).Run();
            new ComputeShaderTask(
                "RakelState/CalculateRakelVolumeOvershoot",
                emitSR,
                new List<CSAttribute>()
                {
                    new CSComputeBuffer("RakelMappedInfo", rakelMappedInfo),
                    new CSComputeBuffer("CanvasReservoirInfoDuplicate", canvas.Reservoir.PaintGridDuplicate.Info),
                    new CSInt3("CanvasReservoirSize", canvas.Reservoir.Size),
                },
                false
            ).Run();
            canvas.Reservoir.ReduceVolumeAvg(
                rakelMappedInfo,
                new Vector2Int(Reservoir.Size.x, Reservoir.Size.y),
                emitSR,
                ReducedRakelVolume);

            // update rakel position base z
            new ComputeShaderTask(
                "RakelState/UpdateRakelPositionBaseZ",
                new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
                new List<CSAttribute>()
                {
                    new CSComputeBuffer("ReducedCanvasVolumeSource", ReducedCanvasVolume),
                    new CSComputeBuffer("ReducedRakelVolumeSource", ReducedRakelVolume),

                    new CSFloat("LayerThickness_MAX", layerThickness_MAX),

                    new CSComputeBuffer("RakelInfo", InfoBuffer),
                },
                false
            ).Run();

            // position base z was updated, so we need to recalculate
            UpdateState(Info.Position, baseSink_MAX, tiltSink_MAX, Info.AutoZEnabled, 0, Info.Pressure, Info.Rotation, Info.Tilt);

            StrokeBegin = false;
        }
    }

    public void CalculateRakelMappedInfo_Part2(
        Canvas_ canvas,
        ComputeBuffer rakelMappedInfo,
        ShaderRegion shaderRegion,
        float emitVolume_MIN)
    {
        new ComputeShaderTask(
            "Emit/DistanceFromRakel",
            shaderRegion,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("RakelInfo", InfoBuffer),

                new CSComputeBuffer("RakelMappedInfo", rakelMappedInfo),
            },
            false
        ).Run();

        new ComputeShaderTask(
            "Emit/VolumeToEmit",
            shaderRegion,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("RakelInfo", InfoBuffer),
                new CSComputeBuffer("RakelReservoirInfoDuplicate", Reservoir.PaintGridDuplicate.Info),
                new CSInt3("RakelReservoirSize", Reservoir.Size),

                new CSFloat("CanvasPositionZ", canvas.Position.z),
                new CSComputeBuffer("CanvasReservoirInfoDuplicate", canvas.Reservoir.PaintGridDuplicate.Info),
                new CSInt3("CanvasReservoirSize", canvas.Reservoir.Size),
                new CSComputeBuffer("RakelMappedInfo", rakelMappedInfo),

                new CSInt2("ReservoirPixelEmitRadius", ReservoirPixelEmitRadius),
                //new CSFloat("RakelTilt_MAX", MAX_SUPPORTED_TILT),
                //new CSFloat("EmitDistance_MAX", emitDistance_MAX),
                new CSFloat("EmitVolume_MIN", emitVolume_MIN),
                //new CSFloat("EmitVolume_MAX", emitVolume_MAX),

                //new CSComputeBuffer("DistortionMap", DistortionMap),
                //new CSInt2("DistortionMapSize", DistortionMapSize),
                //new CSInt("DistortionMapIndex", IncrementDistortionMapIndex()),
            },
            false
        ).Run();
    }

    public override string ToString()
    {
        return base.ToString() + string.Format("\nPosition={0}, Rotation={1}, Tilt={2}\nUL={3}, UR={4}\nLL={5}, LR={6}, ", Info.Position, Info.Rotation, Info.Tilt, Info.UpperLeft, Info.UpperRight, Info.LowerLeft, Info.LowerRight);
    }

    public void Fill(ReservoirFiller filler)
    {
        Reservoir.Fill(filler);

        //PickupReservoir.Fill(Color_.CadmiumRed, volume / 2, filler);
    }

    public PaintGrid EmitPaint(
        ShaderRegion shaderRegion,
        Canvas_ canvas,
        ComputeBuffer rakelMappedInfo)
    {
        // HACK rakelEmittedPaint is actually treated as a raw stack with no specified cell volume
        float cellVolume_UNUSED = 0;
        PaintGrid rakelEmittedPaint = new PaintGrid(new Vector3Int(shaderRegion.Size.x, shaderRegion.Size.y, Reservoir.Size.z), cellVolume_UNUSED);

        new ComputeShaderTask(
            "Emit/EmitFromRakel",
            shaderRegion,
            DELETE_CONFLICT_AREA,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("RakelReservoirInfo", Reservoir.PaintGrid.Info),
                new CSComputeBuffer("RakelReservoirContent", Reservoir.PaintGrid.Content),
                new CSComputeBuffer("RakelReservoirInfoDuplicate", Reservoir.PaintGridDuplicate.Info),
                new CSComputeBuffer("RakelReservoirContentDuplicate", Reservoir.PaintGridDuplicate.Content),
                new CSInt3("RakelReservoirSize", Reservoir.Size),
                new CSFloat("RakelReservoirCellVolume", Reservoir.PaintGrid.CellVolume),

                new CSComputeBuffer("RakelMappedInfo", rakelMappedInfo),

                new CSInt2("ReservoirPixelEmitRadius", ReservoirPixelEmitRadius),

                new CSComputeBuffer("RakelEmittedPaintInfo", rakelEmittedPaint.Info),
                new CSComputeBuffer("RakelEmittedPaintContent", rakelEmittedPaint.Content),
                new CSInt3("RakelEmittedPaintSize", rakelEmittedPaint.Size),
            },
            false
        ).Run();

        rakelMappedInfo.Dispose();

        return rakelEmittedPaint;
    }

    public void ApplyPaint(
        ShaderRegion shaderRegion,
        PaintGrid canvasEmittedPaint)
    {
        new ComputeShaderTask(
            "Pickup/ApplyBufferToRakel",
            shaderRegion,
            new List<CSAttribute>
            {
                new CSComputeBuffer("CanvasEmittedPaintInfo", canvasEmittedPaint.Info),
                new CSComputeBuffer("CanvasEmittedPaintContent", canvasEmittedPaint.Content),
                new CSInt3("CanvasEmittedPaintSize", canvasEmittedPaint.Size),

                new CSComputeBuffer("RakelReservoirInfo", Reservoir.PaintGrid.Info),
                new CSComputeBuffer("RakelReservoirContent", Reservoir.PaintGrid.Content),
                new CSInt3("RakelReservoirSize", Reservoir.Size),
                new CSFloat("RakelReservoirCellVolume", Reservoir.PaintGrid.CellVolume),
            },
            false
        ).Run();

        canvasEmittedPaint.Dispose();
    }

    public void Dispose()
    {
        Reservoir.Dispose();
        InfoBuffer.Dispose();
        DistortionMap.Dispose();

        ReducedCanvasVolume.Dispose();
        ReducedRakelVolume.Dispose();
    }
}


