using UnityEngine;
using System.Collections.Generic;

public class Rakel : ComputeShaderCreator, IRakel
{
    public float Length { get; private set; }
    public float Width { get; private set; }

    private Vector3 Anchor { get; }

    private Reservoir ApplicationReservoir;
    private Reservoir PickupReservoir;

    private Vector2Int PreviousApplyPosition = new Vector2Int(int.MinValue, int.MinValue);

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

    public void Fill(Color_ color, int volume, ReservoirFiller filler)
    {
        ApplicationReservoir.Fill(color, volume, filler);

        //PickupReservoir.Fill(Color_.CadmiumRed, volume / 2, filler);
    }

    // Position is located at Anchor
    // Rotation 0 means Rakel is directed to the right
    // Tilt 0 means Rakel is flat on canvas
    public void Apply(
        Vector3 rakelPosition, float rakelRotation, float rakelTilt,
        TransferConfiguration transferConfiguration,
        OilPaintCanvas oilPaintCanvas)
    {
        WorldSpaceCanvas wsc = oilPaintCanvas.WorldSpaceCanvas;

        // prevent double application on the same pixel
        rakelPosition = wsc.AlignToPixelGrid(rakelPosition);
        if (wsc.MapToPixel(rakelPosition).Equals(PreviousApplyPosition))
        {
            return;
        }
        else
        {
            PreviousApplyPosition = wsc.MapToPixel(rakelPosition);
        }

        //Debug.Log("Applying at x=" + wsc.MapToPixel(rakelPosition));

        RakelSnapshot rakelSnapshot = new RakelSnapshot(Length, Width, Anchor, rakelPosition, rakelRotation, rakelTilt);
        ShaderRegion emitSR = ShaderRegionFactory.Create(
            wsc.MapToPixelInRange(rakelSnapshot.UpperLeft),
            wsc.MapToPixelInRange(rakelSnapshot.UpperRight),
            wsc.MapToPixelInRange(rakelSnapshot.LowerLeft),
            wsc.MapToPixelInRange(rakelSnapshot.LowerRight),
            1 // Padding because interpolation reaches pixels that are not directly under the rakel
        );

        ShaderRegion normalsSR = ShaderRegionFactory.Create(
            wsc.MapToPixelInRange(rakelSnapshot.UpperLeft),
            wsc.MapToPixelInRange(rakelSnapshot.UpperRight),
            wsc.MapToPixelInRange(rakelSnapshot.LowerLeft),
            wsc.MapToPixelInRange(rakelSnapshot.LowerRight),
            2 // Padding of 2 because normals of the previously set pixels around also have to be recalculated
        );

        ApplicationReservoir.Duplicate(
            transferConfiguration.ReservoirDiscardVolumeThreshold,
            transferConfiguration.ReservoirSmoothingKernelSize);

        PickupReservoir.Duplicate(
            transferConfiguration.ReservoirDiscardVolumeThreshold,
            transferConfiguration.ReservoirSmoothingKernelSize);

        ComputeBuffer RakelEmittedPaint = EmitFromRakel(
            rakelSnapshot,
            emitSR,
            wsc,
            transferConfiguration.MapMode);

        ApplyToCanvas(
            emitSR,
            wsc.TextureSize.x,
            RakelEmittedPaint,
            oilPaintCanvas.Reservoir.Buffer);

        UpdateColorTexture(
            emitSR,
            wsc.TextureSize,
            oilPaintCanvas.Reservoir.Buffer,
            oilPaintCanvas.Texture);

        UpdateNormalMap(
            normalsSR,
            wsc.TextureSize,
            oilPaintCanvas.Reservoir.Buffer,
            oilPaintCanvas.NormalMap);
    }

    private ComputeBuffer EmitFromRakel(
        RakelSnapshot rakelSnapshot,
        ShaderRegion shaderRegion,
        WorldSpaceCanvas wsc,
        TransferMapMode transferMapMode,
        bool debugEnabled = false)
    {
        ComputeBuffer RakelEmittedPaint = new ComputeBuffer(shaderRegion.PixelCount, Paint.SizeInBytes);
        // initialize buffer to empty values (Intel does this for you, nvidia doesn't)
        Paint[] initPaint = new Paint[shaderRegion.PixelCount];
        RakelEmittedPaint.SetData(initPaint);

        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSInts2("CalculationPosition", shaderRegion.CalculationPosition),
            new CSInts2("TextureSize", wsc.TextureSize),
            new CSInt("TextureResolution", wsc.Resolution),
            new CSFloats3("CanvasPosition", wsc.Position),
            new CSFloats2("CanvasSize", wsc.Size),
            new CSFloats3("RakelAnchor", Anchor),
            new CSFloats3("RakelPosition", rakelSnapshot.Position),
            new CSFloat("RakelLength", Length),
            new CSFloat("RakelWidth", Width),
            new CSFloat("RakelRotation", rakelSnapshot.Rotation),
            new CSFloats3("RakelULTilted", rakelSnapshot.ulTilted),
            new CSFloats3("RakelURTilted", rakelSnapshot.urTilted),
            new CSFloats3("RakelLLTilted", rakelSnapshot.llTilted),
            new CSFloats3("RakelLRTilted", rakelSnapshot.lrTilted),
            new CSComputeBuffer("RakelApplicationReservoir", ApplicationReservoir.Buffer),
            new CSComputeBuffer("RakelPickupReservoir", PickupReservoir.Buffer),
            new CSComputeBuffer("RakelEmittedPaint", RakelEmittedPaint),
            new CSInts2("RakelReservoirSize", ApplicationReservoir.Size),
            new CSInt("RakelReservoirResolution", ApplicationReservoir.Resolution),
            new CSInt("TransferMapMode", (int)transferMapMode)
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

        return RakelEmittedPaint;
    }

    private void ApplyToCanvas(
        ShaderRegion shaderRegion,
        int textureWidth,
        ComputeBuffer rakelEmittedPaint,
        ComputeBuffer canvasReservoir,
        bool debugEnabled = false)
    {
        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSInts2("CalculationPosition", shaderRegion.CalculationPosition),
            new CSComputeBuffer("RakelEmittedPaint", rakelEmittedPaint),
            new CSComputeBuffer("CanvasReservoir", canvasReservoir),
            new CSInt("TextureWidth", textureWidth)
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

    private void UpdateColorTexture(
        ShaderRegion shaderRegion,
        Vector2Int textureSize,
        ComputeBuffer CanvasReservoir,
        RenderTexture CanvasTexture,
        bool debugEnabled = false)
    {
        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSInts2("CalculationPosition", shaderRegion.CalculationPosition),
            new CSComputeBuffer("CanvasReservoir", CanvasReservoir),
            new CSInts2("TextureSize", textureSize),
            new CSTexture("CanvasTexture", CanvasTexture)
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

    private void UpdateNormalMap(
        ShaderRegion shaderRegion,
        Vector2Int textureSize,
        ComputeBuffer canvasReservoir,
        RenderTexture canvasNormalMap,
        bool debugEnabled = false)
    {
        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSInts2("CalculationPosition", shaderRegion.CalculationPosition),
            new CSComputeBuffer("CanvasReservoir", canvasReservoir),
            new CSInts2("TextureSize", textureSize),
            new CSTexture("NormalMap", canvasNormalMap)
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
        ApplicationReservoir.Dispose();
        PickupReservoir.Dispose();
    }
}


