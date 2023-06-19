using UnityEngine;
using System.Collections.Generic;

public class Rakel
{

    // see EmitFromRakel shader for details (look for "79 degree tilt")
    private const int MIN_SUPPORTED_TILT = 0;
    public const int MAX_SUPPORTED_TILT = 79;

    private float SINK_BASE = 5 * Paint.VOLUME_THICKNESS;
    private float SINK_TILT = 10 * Paint.VOLUME_THICKNESS;

    public static float ClampTilt(float tilt)
    {
        return Mathf.Clamp(tilt, MIN_SUPPORTED_TILT, MAX_SUPPORTED_TILT);
    }

    public float Length { get; private set; }
    public float Width { get; private set; }

    public Vector3 Anchor { get; }

    public Vector3 Position;
    public float Rotation;
    public float Tilt;

    public Vector3 UpperLeft { get; private set; }
    public Vector3 UpperRight { get; private set; }
    public Vector3 LowerLeft { get; private set; }
    public Vector3 LowerRight { get; private set; }

    public Vector3 ulTilted { get; private set; }
    public Vector3 urTilted { get; private set; }
    public Vector3 llTilted { get; private set; }
    public Vector3 lrTilted { get; private set; }

    public Reservoir ApplicationReservoir;
    public Reservoir PickupReservoir;

    private ComputeBuffer DistortionMap;
    private int DistortionMapIndex;
    private const int MAX_STROKE_LENGTH = 3000; // should always be bigger than Rakel width
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
        Length = ApplicationReservoir.Size.y * ApplicationReservoir.PixelSize;
        Width = ApplicationReservoir.Size.x * ApplicationReservoir.PixelSize;

        // NOTE this has to be set after Width and Length were corrected
        Anchor = new Vector3(anchorRatioWidth * Width, anchorRatioLength * Length, 0);
    }

    public void NewStroke()
    {
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

    public void UpdateState(Vector3 position, float pressure, float rotation, float tilt)
    {
        float sink = SINK_BASE + tilt / MAX_SUPPORTED_TILT * SINK_TILT;
        position.z += pressure * sink;
        // prevent sink through canvas
        // TODO include canvas position -> this would also require info about the direction the canvas is oriented
        // TODO include anchor ratio, right now this only works for anchors located on rakel edge
        position.z = Mathf.Min(position.z, 0);
        Position = position;

        Rotation = rotation;
        Tilt = ClampTilt(Mathf.Max(Mathf.Min(tilt, MAX_SUPPORTED_TILT), MIN_SUPPORTED_TILT));

        Vector3 ulOrigin = new Vector3(0, Length, 0);
        Vector3 urOrigin = new Vector3(Width, Length, 0);
        Vector3 llOrigin = new Vector3(0, 0, 0);
        Vector3 lrOrigin = new Vector3(Width, 0, 0);

        Quaternion tiltQuaternion = Quaternion.AngleAxis(Tilt, Vector3.up);
        ulTilted = tiltQuaternion * (ulOrigin - Anchor) + Anchor; // tilt around anchor
        urTilted = tiltQuaternion * (urOrigin - Anchor) + Anchor; // tilt around anchor
        llTilted = tiltQuaternion * (llOrigin - Anchor) + Anchor; // tilt around anchor
        lrTilted = tiltQuaternion * (lrOrigin - Anchor) + Anchor; // tilt around anchor

        Quaternion rotationQuaternion = Quaternion.AngleAxis(Rotation, Vector3.back);
        Vector3 ulRotated = rotationQuaternion * (ulTilted - Anchor) + Anchor; // rotate around anchor
        Vector3 urRotated = rotationQuaternion * (urTilted - Anchor) + Anchor; // rotate around anchor
        Vector3 llRotated = rotationQuaternion * (llTilted - Anchor) + Anchor; // rotate around anchor
        Vector3 lrRotated = rotationQuaternion * (lrTilted - Anchor) + Anchor; // rotate around anchor

        Vector3 positionTranslation = Position - Anchor;
        UpperLeft = ulRotated + positionTranslation;
        UpperRight = urRotated + positionTranslation;
        LowerLeft = llRotated + positionTranslation;
        LowerRight = lrRotated + positionTranslation;
    }

    public override string ToString()
    {
        return base.ToString() + string.Format("\nPosition={0}, Rotation={1}, Tilt={2}\nUL={3}, UR={4}\nLL={5}, LR={6}, ", Position, Rotation, Tilt, UpperLeft, UpperRight, LowerLeft, LowerRight);
    }

    public void Fill(ReservoirFiller filler)
    {
        ApplicationReservoir.Fill(filler);

        //PickupReservoir.Fill(Color_.CadmiumRed, volume / 2, filler);
    }

    public ComputeBuffer EmitPaint(
        ShaderRegion shaderRegion,
        Canvas_ canvas,
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

        WriteRakelMappedInfo(rakelEmittedPaint, shaderRegion, canvas, debugEnabled);

        float pixelSize = 1 / (float) canvas.Resolution;
        float pixelDiag = pixelSize * Mathf.Sqrt(2);
        float tiltedPixelShortSide = Mathf.Cos(Tilt * Mathf.Deg2Rad) * pixelSize;
        int clipRadiusX = (int)Mathf.Ceil((pixelDiag / 2) / tiltedPixelShortSide);
        Vector2Int subgridGroupSize = new Vector2Int(clipRadiusX * 2 + 1, 3);

        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSInt("TextureResolution", canvas.Resolution),

            new CSFloat3("RakelAnchor", Anchor),
            new CSFloat("RakelRotation", Rotation),
            new CSFloat("RakelTilt", Tilt),
            new CSFloat("RakelEdgeZ", LowerLeft.z),
            new CSComputeBuffer("RakelApplicationReservoir", ApplicationReservoir.Buffer),
            new CSComputeBuffer("RakelPickupReservoir", PickupReservoir.Buffer),
            new CSInt2("RakelReservoirSize", ApplicationReservoir.Size),

            new CSFloat("CanvasPositionZ", canvas.Position.z),
            new CSComputeBuffer("CanvasReservoir", canvas.Reservoir.Buffer),
            new CSInt2("CanvasReservoirSize", canvas.Reservoir.Size),

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
        };

        ComputeShaderTask cst = new ComputeShaderTask(
            "EmitFromRakel",
            shaderRegion,
            subgridGroupSize,
            attributes,
            debugEnabled
        );

        cst.Run();

        return rakelEmittedPaint;
    }

    private void WriteRakelMappedInfo(
        ComputeBuffer rakelMappedInfoTarget,
        ShaderRegion shaderRegion,
        Canvas_ canvas,
        bool debugEnabled = false)
    {
        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSInt("TextureResolution", canvas.Resolution),

            new CSFloat3("CanvasPosition", canvas.Position),
            new CSFloat2("CanvasSize", canvas.Size),

            new CSFloat("RakelLength", Length),
            new CSFloat3("RakelPosition", Position),
            new CSFloat3("RakelAnchor", Anchor),
            new CSFloat("RakelRotation", Rotation),
            new CSFloat("RakelTilt", Tilt),
            new CSFloat3("RakelLLTilted", llTilted),
            new CSFloat3("RakelLRTilted", lrTilted),
            new CSInt2("RakelReservoirSize", ApplicationReservoir.Size),

            new CSComputeBuffer("RakelMappedInfoTarget", rakelMappedInfoTarget),
        };

        ComputeShaderTask cst = new ComputeShaderTask(
            "RakelMappedInfo",
            shaderRegion,
            attributes,
            debugEnabled
        );

        cst.Run();
    }

    public void ApplyPaint(
        ShaderRegion shaderRegion,
        ComputeBuffer canvasEmittedPaint,
        bool debugEnabled = false)
    {
        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSComputeBuffer("CanvasEmittedPaint", canvasEmittedPaint),

            new CSComputeBuffer("RakelPickupReservoir", PickupReservoir.Buffer),
            new CSInt("RakelReservoirWidth", PickupReservoir.Size.x)
        };

        ComputeShaderTask cst = new ComputeShaderTask(
            "ApplyBufferToRakel",
            shaderRegion,
            attributes,
            debugEnabled
        );

        cst.Run();

        canvasEmittedPaint.Dispose();
    }

    public void Dispose()
    {
        ApplicationReservoir.Dispose();
        PickupReservoir.Dispose();

        DistortionMap.Dispose();
    }
}


