using UnityEngine;
using System.Collections.Generic;

public struct RakelInfo
{
    public const int SizeInBytes = 6 * sizeof(float) + 10 * 3 * sizeof(float);

    public float Length;
    public float Width;

    public Vector3 Anchor;

    public Vector3 Position;
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

public struct MappedInfo
{
    public const int SizeInBytes = 3 * sizeof(float);

    public Vector2 ReservoirPixel;
    public float Distance;
}

public class Rakel
{
    // see EmitFromRakel shader for details (look for "79 degree tilt")
    private const int MIN_SUPPORTED_TILT = 0;
    public const int MAX_SUPPORTED_TILT = 79;

    private float SINK_BASE_MAX = 5 * Paint.VOLUME_THICKNESS;
    private float SINK_TILT_MAX = 10 * Paint.VOLUME_THICKNESS;

    public static float ClampTilt(float tilt)
    {
        return Mathf.Clamp(tilt, MIN_SUPPORTED_TILT, MAX_SUPPORTED_TILT);
    }

    private bool StrokeBegin;

    public ComputeBuffer InfoBuffer;
    public RakelInfo Info;

    public Reservoir ApplicationReservoir;
    public Reservoir PickupReservoir;

    private ComputeBuffer DistortionMap;
    private int DistortionMapIndex;
    private const int MAX_STROKE_LENGTH = 3000; // should always be bigger than Rakel reservoir size.x
    private Vector2Int DistortionMapSize;

    public Rakel(float length, float width, int resolution, float anchorRatioLength = 0.5f, float anchorRatioWidth = 1)
    {
        Vector2Int reservoirSize = new Vector2Int((int)(width * resolution), (int)(length * resolution));

        ApplicationReservoir = new Reservoir(
            resolution,
            reservoirSize.x,
            reservoirSize.y);

        PickupReservoir = new Reservoir(
            resolution,
            reservoirSize.x,
            reservoirSize.y);

        DistortionMapSize = new Vector2Int(MAX_STROKE_LENGTH, reservoirSize.y);
        DistortionMap = new ComputeBuffer(DistortionMapSize.x * DistortionMapSize.y , sizeof(float));

        // make sure Rakel is not bigger than its reservoir
        Info.Length = ApplicationReservoir.Size.y * ApplicationReservoir.PixelSize;
        Info.Width = ApplicationReservoir.Size.x * ApplicationReservoir.PixelSize;

        // NOTE this has to be set after Width and Length were corrected
        Info.Anchor = new Vector3(anchorRatioWidth * Info.Width, anchorRatioLength * Info.Length, 0);

        InfoBuffer = new ComputeBuffer(1, RakelInfo.SizeInBytes);
        InfoBuffer.SetData(new RakelInfo[] { Info });
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
        if (DistortionMapIndex > DistortionMapSize.x - ApplicationReservoir.Size.x - 1)
        {
            DistortionMapIndex = 0;
        }

        return oldValue;
    }

    public void UpdateState(Vector3 position, float pressure, float rotation, float tilt, bool debugEnabled = false)
    {
        // Update info on CPU for rakel rendering
        Info.Position = position;
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


        // Update info on GPU for paint transfer calculations
        new ComputeShaderTask(
            "UpdateRakelState",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            new List<CSAttribute>()
            {
                new CSFloat3("Position", Info.Position),
                new CSFloat("Pressure", Info.Pressure),
                new CSFloat("Rotation", Info.Rotation),
                new CSFloat("Tilt", Info.Tilt),
                new CSFloat("MAX_SUPPORTED_TILT", MAX_SUPPORTED_TILT),
                new CSFloat("SINK_BASE_MAX", SINK_BASE_MAX),
                new CSFloat("SINK_TILT_MAX", SINK_TILT_MAX),

                new CSComputeBuffer("RakelInfo", InfoBuffer),
            },
            debugEnabled
        ).Run();
    }

    public ComputeBuffer CalculateRakelMappedInfo(
        ShaderRegion shaderRegion,
        Canvas_ canvas,
        bool debugEnabled = false)
    {
        ComputeBuffer rakelMappedInfo = new ComputeBuffer(shaderRegion.PixelCount, MappedInfo.SizeInBytes);
        MappedInfo[] rakelMappedInfoData = new MappedInfo[shaderRegion.PixelCount];
        rakelMappedInfo.SetData(rakelMappedInfoData);

        new ComputeShaderTask(
            "RakelMappedInfo",
            shaderRegion,
            new List<CSAttribute>()
            {
                new CSInt("TextureResolution", canvas.Resolution),

                new CSFloat3("CanvasPosition", canvas.Position),
                new CSFloat2("CanvasSize", canvas.Size),

                new CSComputeBuffer("RakelInfo", InfoBuffer),
                new CSInt2("RakelReservoirSize", ApplicationReservoir.Size),

                new CSComputeBuffer("RakelMappedInfo", rakelMappedInfo),
            },
            debugEnabled
        ).Run();

        return rakelMappedInfo;
    }

    public void RecalculatePositionZ(
        Canvas_ canvas,
        ComputeBuffer rakelMappedInfo,
        ShaderRegion emitSR,
        float layerThickness_MAX,
        bool debugEnabled = false)
    {
        if (StrokeBegin)
        {
            canvas.Reservoir.DuplicateActive(
                rakelMappedInfo,
                ApplicationReservoir.Size,
                emitSR,
                debugEnabled);

            canvas.Reservoir.ReduceVolume(
                emitSR,
                ReduceFunction.Max,
                debugEnabled);

            UpdatePositionZ(
                canvas.Reservoir.Buffer,
                canvas.Reservoir.Size,
                emitSR.Position,
                layerThickness_MAX,
                debugEnabled);

            // position was updated, so we need to recalculate
            UpdateState(Info.Position, Info.Pressure, Info.Rotation, Info.Tilt, debugEnabled);

            StrokeBegin = false;
        }
    }

    private void UpdatePositionZ(
        ComputeBuffer maxVolumeSource,
        Vector2Int maxVolumeSourceSize,
        Vector2Int maxVolumeSourceIndex,
        float layerThickness_MAX,
        bool debugEnabled = false)
    {
        new ComputeShaderTask(
            "UpdateRakelPositionZ",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            new List<CSAttribute>()
            {
                new CSComputeBuffer("MaxVolumeSource", maxVolumeSource),
                new CSInt2("MaxVolumeSourceSize", maxVolumeSourceSize),
                new CSInt2("MaxVolumeSourceIndex", maxVolumeSourceIndex),

                new CSFloat("LayerThickness_MAX", layerThickness_MAX),
                new CSFloat("Pressure", Info.Pressure),
                new CSFloat("Tilt", Info.Tilt),
                new CSFloat("MAX_SUPPORTED_TILT", MAX_SUPPORTED_TILT),
                new CSFloat("SINK_BASE_MAX", SINK_BASE_MAX),
                new CSFloat("SINK_TILT_MAX", SINK_TILT_MAX),

                new CSComputeBuffer("RakelInfo", InfoBuffer),
            },
            debugEnabled
        ).Run();
    }

    public override string ToString()
    {
        return base.ToString() + string.Format("\nPosition={0}, Rotation={1}, Tilt={2}\nUL={3}, UR={4}\nLL={5}, LR={6}, ", Info.Position, Info.Rotation, Info.Tilt, Info.UpperLeft, Info.UpperRight, Info.LowerLeft, Info.LowerRight);
    }

    public void Fill(ReservoirFiller filler)
    {
        ApplicationReservoir.Fill(filler);

        //PickupReservoir.Fill(Color_.CadmiumRed, volume / 2, filler);
    }

    public ComputeBuffer EmitPaint(
        ShaderRegion shaderRegion,
        Canvas_ canvas,
        ComputeBuffer rakelMappedInfo,
        float emitDistance_MAX,
        float emitVolume_MIN,
        float emitVolume_MAX,
        float emitVolumeApplicationReservoirRate,
        float emitVolumePickupReservoirRate,
        bool debugEnabled = false)
    {
        ComputeBuffer rakelEmittedPaint = new ComputeBuffer(shaderRegion.PixelCount, Paint.SizeInBytes);
        // initialize buffer to empty values (Intel does this for you, nvidia doesn't)
        Paint[] initPaint = new Paint[shaderRegion.PixelCount];
        rakelEmittedPaint.SetData(initPaint);

        float pixelSize = 1 / (float) canvas.Resolution;
        float pixelDiag = pixelSize * Mathf.Sqrt(2);
        float tiltedPixelShortSide = Mathf.Cos(Info.Tilt * Mathf.Deg2Rad) * pixelSize;
        int clipRadiusX = (int)Mathf.Ceil((pixelDiag / 2) / tiltedPixelShortSide);
        Vector2Int subgridGroupSize = new Vector2Int(clipRadiusX * 2 + 1, 3);

        new ComputeShaderTask(
            "EmitFromRakel",
            shaderRegion,
            subgridGroupSize,
            new List<CSAttribute>()
            {
                new CSInt("TextureResolution", canvas.Resolution),

                new CSComputeBuffer("RakelInfo", InfoBuffer),
                new CSComputeBuffer("RakelApplicationReservoir", ApplicationReservoir.Buffer),
                new CSComputeBuffer("RakelPickupReservoir", PickupReservoir.Buffer),
                new CSInt2("RakelReservoirSize", ApplicationReservoir.Size),

                new CSFloat("CanvasPositionZ", canvas.Position.z),
                new CSComputeBuffer("CanvasReservoir", canvas.Reservoir.Buffer),
                new CSInt2("CanvasReservoirSize", canvas.Reservoir.Size),
                new CSComputeBuffer("RakelMappedInfo", rakelMappedInfo),

                new CSInt("ClipRadiusX", clipRadiusX),
                new CSFloat("RakelTilt_MAX", MAX_SUPPORTED_TILT),
                new CSFloat("EmitDistance_MAX", emitDistance_MAX),
                new CSFloat("EmitVolume_MIN", emitVolume_MIN),
                new CSFloat("EmitVolume_MAX", emitVolume_MAX),
                new CSFloat("EmitVolumeApplicationReservoirRate", emitVolumeApplicationReservoirRate),
                new CSFloat("EmitVolumePickupReservoirRate", emitVolumePickupReservoirRate),

                new CSComputeBuffer("DistortionMap", DistortionMap),
                new CSInt2("DistortionMapSize", DistortionMapSize),
                new CSInt("DistortionMapIndex", IncrementDistortionMapIndex()),

                new CSComputeBuffer("RakelEmittedPaint", rakelEmittedPaint),
            },
            debugEnabled
        ).Run();

        rakelMappedInfo.Dispose();

        return rakelEmittedPaint;
    }

    public void ApplyPaint(
        ShaderRegion shaderRegion,
        ComputeBuffer canvasEmittedPaint,
        bool debugEnabled = false)
    {
        new ComputeShaderTask(
            "ApplyBufferToRakel",
            shaderRegion,
            new List<CSAttribute>
            {
                new CSComputeBuffer("CanvasEmittedPaint", canvasEmittedPaint),

                new CSComputeBuffer("RakelPickupReservoir", PickupReservoir.Buffer),
                new CSInt("RakelReservoirWidth", PickupReservoir.Size.x)
            },
            debugEnabled
        ).Run();

        canvasEmittedPaint.Dispose();
    }

    public void Dispose()
    {
        ApplicationReservoir.Dispose();
        PickupReservoir.Dispose();
        InfoBuffer.Dispose();
        DistortionMap.Dispose();
    }
}


