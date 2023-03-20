using UnityEngine;
using System.Collections.Generic;

public class Rakel : ComputeShaderCreator
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

    public Rakel(RakelConfiguration config, ShaderRegionFactory shaderRegionFactory, ComputeShaderEngine computeShaderEngine)
        : base(shaderRegionFactory, computeShaderEngine)
    {
        ApplicationReservoir = new Reservoir(
            config.Resolution,
            (int)(config.Width * config.Resolution),
            (int)(config.Length * config.Resolution),
            shaderRegionFactory,
            computeShaderEngine);

        PickupReservoir = new Reservoir(
            config.Resolution,
            (int)(config.Width * config.Resolution),
            (int)(config.Length * config.Resolution),
            shaderRegionFactory,
            computeShaderEngine);

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

    public void Fill(Color_ color, int volume, ReservoirFiller filler)
    {
        ApplicationReservoir.Fill(color, volume, filler);

        //PickupReservoir.Fill(Color_.CadmiumRed, volume / 2, filler);
    }

    public void EmitPaint(
        ShaderRegion shaderRegion,
        OilPaintCanvas oilPaintCanvas,
        TransferMapMode transferMapMode,
        float emitVolumeApplicationReservoir,
        float emitVolumePickupReservoir,
        bool debugEnabled = false)
    {
        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSInt2("CalculationPosition", shaderRegion.CalculationPosition),
            new CSFloat3("CanvasPosition", oilPaintCanvas.WorldSpaceCanvas.Position),
            new CSFloat2("CanvasSize", oilPaintCanvas.WorldSpaceCanvas.Size),
            new CSComputeBuffer("CanvasReservoir", oilPaintCanvas.Reservoir.Buffer),
            new CSInt2("CanvasReservoirSize", oilPaintCanvas.WorldSpaceCanvas.TextureSize),
            new CSInt("CanvasReservoirResolution", oilPaintCanvas.WorldSpaceCanvas.Resolution),
            new CSInt("CRReadIndex", oilPaintCanvas.Reservoir.ReadIndex),
            new CSInt("CRWriteIndex", oilPaintCanvas.Reservoir.WriteIndex),
            new CSFloat3("RakelAnchor", Anchor),
            new CSFloat3("RakelPosition", Position),
            new CSFloat("RakelLength", Length),
            new CSFloat("RakelWidth", Width),
            new CSFloat("RakelRotation", Rotation),
            new CSFloat3("RakelULTilted", ulTilted),
            new CSFloat3("RakelURTilted", urTilted),
            new CSFloat3("RakelLLTilted", llTilted),
            new CSFloat3("RakelLRTilted", lrTilted),
            new CSComputeBuffer("RakelApplicationReservoir", ApplicationReservoir.Buffer),
            new CSComputeBuffer("RakelPickupReservoir", PickupReservoir.Buffer),
            new CSInt2("RakelReservoirSize", ApplicationReservoir.Size),
            new CSInt("RakelReservoirResolution", ApplicationReservoir.Resolution),
            new CSInt("RRReadIndex", ApplicationReservoir.ReadIndex),
            new CSInt("RRWriteIndex", ApplicationReservoir.WriteIndex),
            new CSInt("TransferMapMode", (int)transferMapMode),
            new CSFloat("EmitVolumeApplicationReservoir", emitVolumeApplicationReservoir),
            new CSFloat("EmitVolumePickupReservoir", emitVolumePickupReservoir),
        };

        ComputeShaderTask cst = new ComputeShaderTask(
            "EmitFromRakelShader",
            shaderRegion,
            attributes,
            null,
            new List<ComputeBuffer>(),
            debugEnabled
        );

        ComputeShaderEngine.EnqueueOrRun(cst);

        oilPaintCanvas.Reservoir.ToggleDoubleBuffering();
        ApplicationReservoir.ToggleDoubleBuffering();
        PickupReservoir.ToggleDoubleBuffering(); // not really necessary but we better be future proof
    }

    public void Dispose()
    {
        ApplicationReservoir.Dispose();
        PickupReservoir.Dispose();
    }
}


