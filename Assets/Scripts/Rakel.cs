using UnityEngine;
using System.Collections.Generic;

public class Rakel
{
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

    public Rakel(float length, float width, int resolution)
    {
        ApplicationReservoir = new Reservoir(
            resolution,
            (int)(width * resolution),
            (int)(length * resolution));

        PickupReservoir = new Reservoir(
            resolution,
            (int)(width * resolution),
            (int)(length * resolution));

        // make sure Rakel is not bigger than its reservoir
        Length = ApplicationReservoir.Size.y * ApplicationReservoir.PixelSize;
        Width = ApplicationReservoir.Size.x * ApplicationReservoir.PixelSize;

        // NOTE this has to be set after Width and Length were corrected
        Anchor = new Vector3(Width, Length / 2, 0);
    }

    public void UpdateState(Vector3 position, float rotation, float tilt)
    {
        Position = position;
        Rotation = rotation;
        Tilt = tilt;

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
        ShaderCalculation shaderCalculation,
        WorldSpaceCanvas wsc,
        float emitVolumeApplicationReservoir,
        float emitVolumePickupReservoir,
        bool debugEnabled = false)
    {
        ComputeBuffer RakelEmittedPaint = new ComputeBuffer(shaderCalculation.PixelCount, Paint.SizeInBytes);
        // initialize buffer to empty values (Intel does this for you, nvidia doesn't)
        Paint[] initPaint = new Paint[shaderCalculation.PixelCount];
        RakelEmittedPaint.SetData(initPaint);

        float pixelSize = 1 / (float) wsc.Resolution;
        float pixelDiag = pixelSize * Mathf.Sqrt(2);
        float tiltedPixelShortSide = Mathf.Cos(Tilt * Mathf.Deg2Rad) * pixelSize;
        int clipRadiusX = (int)Mathf.Ceil((pixelDiag / 2) / tiltedPixelShortSide);
        Vector2Int subgridGroupSize = new Vector2Int(clipRadiusX * 2 + 1, 3);

        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSInt2("TextureSize", wsc.TextureSize),
            new CSInt("TextureResolution", wsc.Resolution),
            new CSFloat3("CanvasPosition", wsc.Position),
            new CSFloat2("CanvasSize", wsc.Size),
            new CSFloat3("RakelAnchor", Anchor),
            new CSFloat3("RakelPosition", Position),
            new CSFloat("RakelLength", Length),
            new CSFloat("RakelWidth", Width),
            new CSFloat("RakelRotation", Rotation),
            new CSFloat("RakelTilt", Tilt),
            new CSInt("ClipRadiusX", clipRadiusX),
            new CSFloat3("RakelULTilted", ulTilted),
            new CSFloat3("RakelURTilted", urTilted),
            new CSFloat3("RakelLLTilted", llTilted),
            new CSFloat3("RakelLRTilted", lrTilted),
            new CSComputeBuffer("RakelApplicationReservoir", ApplicationReservoir.Buffer),
            new CSComputeBuffer("RakelPickupReservoir", PickupReservoir.Buffer),
            new CSComputeBuffer("RakelEmittedPaint", RakelEmittedPaint),
            new CSInt2("RakelReservoirSize", ApplicationReservoir.Size),
            new CSFloat("EmitVolumeApplicationReservoir", emitVolumeApplicationReservoir),
            new CSFloat("EmitVolumePickupReservoir", emitVolumePickupReservoir),
        };

        ComputeShaderTask cst = new ComputeShaderTask(
            "EmitFromRakel",
            shaderCalculation,
            subgridGroupSize,
            attributes,
            new List<ComputeBuffer>(),
            debugEnabled
        );

        cst.Run();

        return RakelEmittedPaint;
    }

    public void ApplyPaint(
        ShaderCalculation shaderCalculation,
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
            shaderCalculation,
            attributes,
            new List<ComputeBuffer>() { canvasEmittedPaint },
            debugEnabled
        );

        cst.Run();
    }

    public void Dispose()
    {
        ApplicationReservoir.Dispose();
        PickupReservoir.Dispose();
    }
}


