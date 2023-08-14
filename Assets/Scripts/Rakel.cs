using UnityEngine;
using System.Collections.Generic;

public struct RakelInfo
{
    public const int SizeInBytes = 1 * sizeof(int) + 8 * sizeof(float) + 10 * 3 * sizeof(float);

    public float Length;
    public float Width;

    public Vector3 Anchor;

    public Vector3 Position;
    public int AutoZEnabled;
    public float PositionBaseZ;
    public float ActualLayerThickness;
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

    public ComputeBuffer InfoBuffer;
    public RakelInfo Info;
    private ComputeBuffer PositionZAvgRingbuffer;
    ComputeBuffer ReducedCanvasVolume;
    ComputeBuffer ReducedRakelVolume;

    public Reservoir Reservoir;

    private ComputeBuffer DistortionMap;
    private int DistortionMapIndex;
    private const int MAX_STROKE_LENGTH = 3000; // should always be bigger than Rakel reservoir size.x
    private Vector2Int DistortionMapSize;

    private Vector2Int ReservoirPixelEmitRadius;
    private Vector2Int DELETE_CONFLICT_AREA = new Vector2Int(3,3);

    private bool StrokeBegin;

    private float CurrentStrokeLength;
    private bool TiltNoiseEnabled;
    private NoiseFilter1D TiltNoise;

    public Rakel(
        float length, float width, int resolution,
        int layers_MAX, float cellVolume,
        float anchorRatioLength = 0.5f, float anchorRatioWidth = 1)
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

    public void NewStroke(bool tiltNoiseEnabled, float tiltNoiseFrequency, float tiltNoiseAmplitude, float floatingZLength)
    {
        StrokeBegin = true;

        TiltNoiseEnabled = tiltNoiseEnabled;
        if (TiltNoiseEnabled)
        {
            CurrentStrokeLength = 0;
            TiltNoise = new NoiseFilter1D(tiltNoiseFrequency, tiltNoiseAmplitude);
        }

        int positionZAvgRingbufferSize = floatingZLength > 0 ? (int)(Reservoir.Resolution * floatingZLength) : 1;
        // this is actually a struct:
        // - float CurrentAvg;
        // - int Pointer;
        // - int Size;
        // - float[SIZE] Elements;
        // we don't do an actual struct, because then we can't parametrize SIZE dynamically
        PositionZAvgRingbuffer?.Dispose();
        PositionZAvgRingbuffer = new ComputeBuffer(3 + positionZAvgRingbufferSize, sizeof(float));
        float[] positionZAvgRingbufferData = new float[3 + positionZAvgRingbufferSize];
        positionZAvgRingbufferData[2] = positionZAvgRingbufferSize; // set size, rest is initialized in shader
        PositionZAvgRingbuffer.SetData(positionZAvgRingbufferData);

        //float[] distortionMapData = new float[DistortionMapSize.x * DistortionMapSize.y];

        ////float noiseCapRatio = 0.6f;
        ////float noiseCapSubtract = 0.5f;

        //PerlinNoise perlinNoise = new PerlinNoise(DistortionMapSize, new Vector2(10, 800));
        //for (int x = 0; x < DistortionMapSize.x; x++)
        //{
        //    for (int y = 0; y < DistortionMapSize.y; y++)
        //    {
        //        float value = perlinNoise.ValueAt(x, y);
        //        //distortionMapData[IndexUtil.XY(x, y, DistortionMapSize)] = Mathf.Clamp01(Mathf.Clamp(value, 0, noiseCapRatio) - noiseCapSubtract);
        //        distortionMapData[IndexUtil.XY(x, y, DistortionMapSize)] = value;
        //    }
        //}

        //DistortionMap.SetData(distortionMapData);

        // Do this, so when Fill is called next time, there is no data left
        // in Reservoir PaintGrid from last fill
        Reservoir.ResetPGData();
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

    public void UpdateState(
        Vector3 position,
       float baseSink_MAX, float layerSink_MAX_Ratio, float tiltSink_MAX,
       bool autoZEnabled, bool zZero, bool finalUpdateForStroke,
       float pressure, float rotation, float tilt)
    {
        Vector2 previousPosition = new Vector2(Info.Position.x, Info.Position.y);
        Vector2 newPosition = new Vector2(position.x, position.y);
        float distanceSinceLastPosition = (newPosition - previousPosition).magnitude;
        CurrentStrokeLength += distanceSinceLastPosition;

        // Update info on CPU for rakel rendering
        Info.Position = position;
        Info.AutoZEnabled = Cast.BoolToInt(autoZEnabled);
        Info.Pressure = pressure;
        Info.Rotation = rotation;
        if (TiltNoiseEnabled)
        {
            tilt = TiltNoise.Filter(tilt, CurrentStrokeLength);
        }
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
                new CSInt("ZZero", Cast.BoolToInt(zZero)),
                new CSFloat("Pressure", Info.Pressure),
                new CSFloat("Rotation", Info.Rotation),
                new CSFloat("Tilt", Info.Tilt),
                new CSFloat("MAX_SUPPORTED_TILT", MAX_SUPPORTED_TILT),
                new CSFloat("BaseSink_MAX", baseSink_MAX),
                new CSFloat("LayerSink_MAX_Ratio", layerSink_MAX_Ratio),
                new CSFloat("TiltSink_MAX", tiltSink_MAX),

                new CSInt("FinalUpdateForStroke", Cast.BoolToInt(finalUpdateForStroke)),
                new CSComputeBuffer("PositionZAvgRingbuffer", PositionZAvgRingbuffer),
                new CSInt("StrokeBegin", Cast.BoolToInt(StrokeBegin)),

                new CSComputeBuffer("RakelInfo", InfoBuffer),
            },
            false
        ).Run();
    }

    public ComputeBuffer CalculateRakelMappedInfo(
        ShaderRegion shaderRegion,
        Canvas_ canvas,
        ComputeBuffer rakelMappedInfo)
    {
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
                new CSInt2("RakelMappedInfoSize", canvas.Reservoir.Size2D),
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
                new CSInt2("RakelMappedInfoSize", canvas.Reservoir.Size2D),
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
                new CSInt3("RakelReservoirSize", Reservoir.Size),

                new CSInt2("ReservoirPixelEmitRadius", ReservoirPixelEmitRadius),
                new CSComputeBuffer("RakelMappedInfo", rakelMappedInfo),
                new CSInt2("RakelMappedInfoSize", canvas.Reservoir.Size2D),
            },
            false
        ).Run();

        return rakelMappedInfo;
    }

    public void RecalculatePositionBaseZ(
        Canvas_ canvas,
        ComputeBuffer rakelMappedInfo,
        ShaderRegion emitSR,
        ReduceFunction canvasVolumeReduceFunction,
        ReduceFunction rakelVolumeReduceFunction,
        bool readjustZToRakelVolume,
        bool readjustZToCanvasVolume,
        float layerThickness_MAX,
        bool tiltAdjustLayerThickness,
        float baseSink_MAX,
        float layerSink_MAX_Ratio,
        float tiltSink_MAX)
    {
        if (Info.AutoZEnabled == 1)
        {
            if (StrokeBegin)
            {
                // only snapshot will be used for paint height calculation
                // -> newly applied paint does not have an impact
                canvas.Reservoir.SnapshotInfo();
            }

            // reduce canvas volume
            if (StrokeBegin || readjustZToCanvasVolume)
            {
                canvas.Reservoir.CopySnapshotActiveInfoVolumesToWorkspace(
                    rakelMappedInfo,
                    canvas.Reservoir.Size2D,
                    Reservoir.Size2D,
                    emitSR);

                canvas.Reservoir.ReduceActiveWorkspace(
                    rakelMappedInfo,
                    canvas.Reservoir.Size2D,
                    Reservoir.Size2D,
                    emitSR,
                    canvasVolumeReduceFunction,
                    ReducedCanvasVolume);
            }

            // reduce rakel volume
            if (StrokeBegin || readjustZToRakelVolume)
            {
                new ComputeShaderTask(
                    "RakelState/WriteSampledRakelVolumesToWorkspace",
                    emitSR,
                    new List<CSAttribute>()
                    {
                        new CSComputeBuffer("RakelReservoirInfo", Reservoir.PaintGrid.Info),
                        new CSInt3("RakelReservoirSize", Reservoir.Size),

                        new CSInt2("ReservoirPixelEmitRadius", ReservoirPixelEmitRadius),
                        new CSComputeBuffer("RakelMappedInfo", rakelMappedInfo),
                        new CSInt2("RakelMappedInfoSize", canvas.Reservoir.Size2D),
                        new CSComputeBuffer("Workspace", canvas.Reservoir.Workspace),
                        new CSInt3("WorkspaceSize", canvas.Reservoir.Size),
                    },
                    false
                ).Run();
                // reset Z so that distance between rakel edge and canvas is 0
                // (simplifies overshoot calculation)
                UpdateState(
                    Info.Position,
                    baseSink_MAX, layerSink_MAX_Ratio, tiltSink_MAX,
                    Cast.IntToBool(Info.AutoZEnabled), true, false,
                    Info.Pressure, Info.Rotation, Info.Tilt);
                new ComputeShaderTask(
                    "Emit/DistanceFromRakel",
                    emitSR,
                    new List<CSAttribute>()
                    {
                        new CSComputeBuffer("RakelInfo", InfoBuffer),

                        new CSComputeBuffer("RakelMappedInfo", rakelMappedInfo),
                        new CSInt2("RakelMappedInfoSize", canvas.Reservoir.Size2D),
                    },
                    false
                ).Run();
                new ComputeShaderTask(
                    "RakelState/CalculateRakelVolumeOvershoot",
                    emitSR,
                    new List<CSAttribute>()
                    {
                        new CSComputeBuffer("RakelMappedInfo", rakelMappedInfo),
                        new CSInt2("RakelMappedInfoSize", canvas.Reservoir.Size2D),
                        new CSComputeBuffer("SampledRakelVolumes", canvas.Reservoir.Workspace),
                        new CSInt3("SampledRakelVolumesSize", canvas.Reservoir.Size),
                    },
                    false
                ).Run();
                canvas.Reservoir.ReduceActiveWorkspace(
                    rakelMappedInfo,
                    canvas.Reservoir.Size2D,
                    Reservoir.Size2D,
                    emitSR,
                    rakelVolumeReduceFunction,
                    ReducedRakelVolume);
            }

            if (StrokeBegin || readjustZToCanvasVolume || readjustZToRakelVolume)
            {
                // update rakel position base z
                new ComputeShaderTask(
                    "RakelState/UpdateRakelPositionBaseZ",
                    new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
                    new List<CSAttribute>()
                    {
                        new CSComputeBuffer("ReducedCanvasVolumeSource", ReducedCanvasVolume),
                        new CSComputeBuffer("ReducedRakelVolumeSource", ReducedRakelVolume),

                        new CSFloat("LayerThickness_MAX", layerThickness_MAX),
                        new CSInt("TiltAdjustLayerThickness", Cast.BoolToInt(tiltAdjustLayerThickness)),
                        new CSFloat("MAX_SUPPORTED_TILT", MAX_SUPPORTED_TILT),

                        new CSComputeBuffer("RakelInfo", InfoBuffer),
                    },
                    false
                ).Run();

                // position base z was updated, so we need to recalculate
                UpdateState(
                    Info.Position,
                    baseSink_MAX, layerSink_MAX_Ratio, tiltSink_MAX,
                    Cast.IntToBool(Info.AutoZEnabled), false, true,
                    Info.Pressure, Info.Rotation, Info.Tilt);
            }
        }

        StrokeBegin = false;
    }

    public void CalculateRakelMappedInfo_Part2(
        Canvas_ canvas,
        ComputeBuffer rakelMappedInfo,
        ShaderRegion shaderRegion,
        float emitDistance_MAX,
        float emitVolume_MIN,
        bool trueVolume_MIN_Calculation)
    {
        new ComputeShaderTask(
            "Emit/DistanceFromRakel",
            shaderRegion,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("RakelInfo", InfoBuffer),

                new CSComputeBuffer("RakelMappedInfo", rakelMappedInfo),
                new CSInt2("RakelMappedInfoSize", canvas.Reservoir.Size2D),
            },
            false
        ).Run();

        new ComputeShaderTask(
            "Emit/VolumeToEmit",
            shaderRegion,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("RakelInfo", InfoBuffer),
                new CSComputeBuffer("RakelReservoirInfoDuplicate", Reservoir.PaintGridImprintCopy.Info),
                new CSInt3("RakelReservoirSize", Reservoir.Size),

                new CSFloat("CanvasPositionZ", canvas.Position.z),
                new CSComputeBuffer("CanvasReservoirInfoDuplicate", canvas.Reservoir.PaintGridImprintCopy.Info),
                new CSInt3("CanvasReservoirSize", canvas.Reservoir.Size),
                new CSComputeBuffer("RakelMappedInfo", rakelMappedInfo),
                new CSInt2("RakelMappedInfoSize", canvas.Reservoir.Size2D),

                new CSInt2("ReservoirPixelEmitRadius", ReservoirPixelEmitRadius),
                new CSFloat("EmitDistance_MAX", emitDistance_MAX),
                new CSFloat("EmitVolume_MIN", emitVolume_MIN),
                new CSInt("TrueVolume_MIN_Calculation", Cast.BoolToInt(trueVolume_MIN_Calculation))

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

    public void EmitPaint(
        Canvas_ canvas,
        ComputeBuffer rakelMappedInfo,
        ShaderRegion shaderRegion)
    {
        new ComputeShaderTask(
            "Emit/EmitFromRakel",
            shaderRegion,
            DELETE_CONFLICT_AREA,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("RakelReservoirInfo", Reservoir.PaintGrid.Info),
                new CSComputeBuffer("RakelReservoirContent", Reservoir.PaintGrid.Content),
                new CSComputeBuffer("RakelReservoirInfoDuplicate", Reservoir.PaintGridImprintCopy.Info),
                new CSComputeBuffer("RakelReservoirContentDuplicate", Reservoir.PaintGridImprintCopy.Content),
                new CSInt3("RakelReservoirSize", Reservoir.Size),
                new CSFloat("RakelReservoirCellVolume", Reservoir.PaintGrid.CellVolume),

                new CSComputeBuffer("RakelMappedInfo", rakelMappedInfo),
                new CSInt2("RakelMappedInfoSize", canvas.Reservoir.Size2D),

                new CSInt2("ReservoirPixelEmitRadius", ReservoirPixelEmitRadius),

                new CSComputeBuffer("RakelEmittedPaintInfo", canvas.Reservoir.PaintGridInputBuffer.Info),
                new CSComputeBuffer("RakelEmittedPaintContent", canvas.Reservoir.PaintGridInputBuffer.Content),
                new CSInt3("RakelEmittedPaintSize", canvas.Reservoir.PaintGridInputBuffer.Size),
            },
            false
        ).Run();
    }

    public void ApplyInputBuffer(ShaderRegion shaderRegion, int diffuseDepth, float diffuseRatio)
    {
        new ComputeShaderTask(
            "Pickup/ApplyBufferToRakel",
            shaderRegion,
            new List<CSAttribute>
            {
                new CSComputeBuffer("CanvasEmittedPaintInfo", Reservoir.PaintGridInputBuffer.Info),
                new CSComputeBuffer("CanvasEmittedPaintContent", Reservoir.PaintGridInputBuffer.Content),
                new CSInt3("CanvasEmittedPaintSize", Reservoir.PaintGridInputBuffer.Size),

                new CSComputeBuffer("RakelReservoirInfo", Reservoir.PaintGrid.Info),
                new CSComputeBuffer("RakelReservoirContent", Reservoir.PaintGrid.Content),
                new CSInt3("RakelReservoirSize", Reservoir.Size),
                new CSFloat("RakelReservoirCellVolume", Reservoir.PaintGrid.CellVolume),
                new CSInt("RakelReservoirDiffuseDepth", diffuseDepth),
                new CSFloat("RakelReservoirDiffuseRatio", diffuseRatio),
            },
            false
        ).Run();
    }

    public void Dispose()
    {
        Reservoir.Dispose();
        InfoBuffer.Dispose();
        PositionZAvgRingbuffer?.Dispose();
        DistortionMap.Dispose();

        ReducedCanvasVolume.Dispose();
        ReducedRakelVolume.Dispose();
    }
}


