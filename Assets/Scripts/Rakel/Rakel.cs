using UnityEngine;
using System.Collections.Generic;

public class Rakel : IRakel
{
    public float Length { get; private set; }
    public float Width { get; private set; }

    private Vector3 Anchor { get; }
    private int ReservoirResolution;

    private Vector2Int RakelReservoirSize;
    private Paint[] RakelApplicationReservoirData;
    private ComputeBuffer RakelApplicationReservoir; // 3D array, z=1 is for duplication for correct interpolation

    private Vector2Int PreviousApplyPosition = new Vector2Int(int.MinValue, int.MinValue);

    private Queue<ComputeShaderTask> ComputeShaderTasks;

    public Rakel(RakelConfiguration config, Queue<ComputeShaderTask> computeShaderTasks)
    {
        ReservoirResolution = config.Resolution;
        RakelReservoirSize.x = (int)(config.Width * config.Resolution);
        RakelReservoirSize.y = (int)(config.Length * config.Resolution);

        RakelApplicationReservoir = new ComputeBuffer(RakelReservoirSize.y * RakelReservoirSize.x * 2,
                                                      4 * sizeof(float) + sizeof(int));
        // initialize buffer to empty values (Intel does this for you, nvidia doesn't)
        RakelApplicationReservoirData = new Paint[RakelReservoirSize.y * RakelReservoirSize.x * 2];
        RakelApplicationReservoir.SetData(RakelApplicationReservoirData);


        // make sure Rakel is not bigger than its reservoir
        float reservoirPixelSize = 1 / (float)config.Resolution;
        Length = RakelReservoirSize.y * reservoirPixelSize;
        Width = RakelReservoirSize.x * reservoirPixelSize;

        // NOTE this has to be set after Width and Length were corrected
        Anchor = new Vector3(Width, Length / 2, 0);

        ComputeShaderTasks = computeShaderTasks;
    }

    public void Fill(Color_ color, int volume, ReservoirFiller filler)
    {
        filler.Fill(color, volume, RakelApplicationReservoirData, RakelReservoirSize);
        RakelApplicationReservoir.SetData(RakelApplicationReservoirData);
    }

    private void EnqueueOrRun(ComputeShaderTask cst)
    {
        if (ComputeShaderTasks != null)
        {
            ComputeShaderTasks.Enqueue(cst);
        }
        else
        {
            cst.Run();
        }
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

        DuplicateReservoir(transferConfiguration.ReservoirDiscardVolumeThreshold,
                           transferConfiguration.ReservoirSmoothingKernelSize);

        ComputeBuffer RakelEmittedPaint = EmitFromRakel(
            wsc,
            rakelPosition, rakelRotation, rakelTilt,
            transferConfiguration.MapMode);

        ApplyToCanvas(
            wsc,
            rakelPosition, rakelRotation, rakelTilt,
            RakelEmittedPaint,
            oilPaintCanvas.Reservoir);

        UpdateColorTexture(
            wsc,
            rakelPosition, rakelRotation, rakelTilt,
            oilPaintCanvas.Reservoir,
            oilPaintCanvas.Texture);

        UpdateNormalMap(
            wsc,
            rakelPosition, rakelRotation, rakelTilt,
            oilPaintCanvas.Reservoir,
            oilPaintCanvas.NormalMap);
    }

    private void DuplicateReservoir(int discardVolumeThreshold, int smoothingKernelSize)
    {
        IntelGPUShaderRegion duplicateSR = new IntelGPUShaderRegion(
            new Vector2Int(0, RakelReservoirSize.y - 1),
            new Vector2Int(RakelReservoirSize.x - 1, RakelReservoirSize.y - 1),
            new Vector2Int(0, 0),
            new Vector2Int(RakelReservoirSize.x - 1, 0)
        );

        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSComputeBuffer("Reservoir", RakelApplicationReservoir),
            new CSInt("DiscardVolumeThreshhold", discardVolumeThreshold),
            new CSInt("SmoothingKernelSize", smoothingKernelSize)
        };

        ComputeShaderTask cst = new ComputeShaderTask(
            "ReservoirDuplicationShader",
            duplicateSR,
            attributes,
            null,
            new List<ComputeBuffer>(),
            null
        //new List<int>() { duplicateSR.CalculationSize.x, duplicateSR.CalculationSize.y }
        );
        EnqueueOrRun(cst);
    }

    private ComputeBuffer EmitFromRakel(
        WorldSpaceCanvas wsc,
        Vector3 rakelPosition, float rakelRotation, float rakelTilt,
        TransferMapMode transferMapMode)
    {
        RakelSnapshot rakelSnapshot = new RakelSnapshot(Length, Width, Anchor, rakelPosition, rakelRotation, rakelTilt);
        IntelGPUShaderRegion emitSR = new IntelGPUShaderRegion(
            wsc.MapToPixelInRange(rakelSnapshot.UpperLeft),
            wsc.MapToPixelInRange(rakelSnapshot.UpperRight),
            wsc.MapToPixelInRange(rakelSnapshot.LowerLeft),
            wsc.MapToPixelInRange(rakelSnapshot.LowerRight),
            1 // Padding because interpolation reaches pixels that are not directly under the rakel
        );

        ComputeBuffer RakelEmittedPaint = new ComputeBuffer(emitSR.CalculationSize.x * emitSR.CalculationSize.y,
                                                            4 * sizeof(float) + sizeof(int));
        // initialize buffer to empty values (Intel does this for you, nvidia doesn't)
        Paint[] initPaint = new Paint[emitSR.CalculationSize.x * emitSR.CalculationSize.y];
        RakelEmittedPaint.SetData(initPaint);

        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSInts2("CalculationPosition", emitSR.CalculationPosition),
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
            new CSComputeBuffer("RakelApplicationReservoir", RakelApplicationReservoir),
            new CSComputeBuffer("RakelEmittedPaint", RakelEmittedPaint),
            new CSInts2("RakelReservoirSize", RakelReservoirSize),
            new CSInt("RakelReservoirResolution", ReservoirResolution),
            new CSInt("TransferMapMode", (int)transferMapMode)
        };

        ComputeShaderTask cst = new ComputeShaderTask(
            "EmitFromRakelShader",
            emitSR,
            attributes,
            null,
            new List<ComputeBuffer>(),
            null
        //new List<int>() { emitSR.CalculationSize.x, emitSR.CalculationSize.y }
        );
        EnqueueOrRun(cst);

        return RakelEmittedPaint;
    }

    private void ApplyToCanvas(
        WorldSpaceCanvas wsc,
        Vector3 rakelPosition, float rakelRotation, float rakelTilt,
        ComputeBuffer rakelEmittedPaint,
        ComputeBuffer canvasReservoir)
    {
        RakelSnapshot rakelSnapshot = new RakelSnapshot(Length, Width, Anchor, rakelPosition, rakelRotation, rakelTilt);
        IntelGPUShaderRegion applySR = new IntelGPUShaderRegion(
            wsc.MapToPixelInRange(rakelSnapshot.UpperLeft),
            wsc.MapToPixelInRange(rakelSnapshot.UpperRight),
            wsc.MapToPixelInRange(rakelSnapshot.LowerLeft),
            wsc.MapToPixelInRange(rakelSnapshot.LowerRight),
            1 // Padding because interpolation reaches pixels that are not directly under the rakel
        );

        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSInts2("CalculationPosition", applySR.CalculationPosition),
            new CSComputeBuffer("RakelEmittedPaint", rakelEmittedPaint),
            new CSComputeBuffer("CanvasReservoir", canvasReservoir),
            new CSInt("TextureWidth", wsc.TextureSize.x)
        };

        ComputeShaderTask cst = new ComputeShaderTask(
            "CopyBufferToCanvasShader",
            applySR,
            attributes,
            null,
            new List<ComputeBuffer>() { rakelEmittedPaint },
            null
        );
        EnqueueOrRun(cst);
    }

    private void UpdateColorTexture(
        WorldSpaceCanvas wsc,
        Vector3 rakelPosition, float rakelRotation, float rakelTilt,
        ComputeBuffer CanvasReservoir,
        RenderTexture CanvasTexture
        )
    {
        RakelSnapshot rakelSnapshot = new RakelSnapshot(Length, Width, Anchor, rakelPosition, rakelRotation, rakelTilt);
        IntelGPUShaderRegion colorsSR = new IntelGPUShaderRegion(
            wsc.MapToPixelInRange(rakelSnapshot.UpperLeft),
            wsc.MapToPixelInRange(rakelSnapshot.UpperRight),
            wsc.MapToPixelInRange(rakelSnapshot.LowerLeft),
            wsc.MapToPixelInRange(rakelSnapshot.LowerRight),
            1 // Padding because interpolation reaches pixels that are not directly under the rakel
        );

        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSInts2("CalculationPosition", colorsSR.CalculationPosition),
            new CSComputeBuffer("CanvasReservoir", CanvasReservoir),
            new CSInts2("TextureSize", wsc.TextureSize),
            new CSTexture("CanvasTexture", CanvasTexture)
        };
        
        ComputeShaderTask cst = new ComputeShaderTask(
            "ColorsShader",
            colorsSR,
            attributes,
            null,
            new List<ComputeBuffer>(),
            null
        );
        EnqueueOrRun(cst);
    }

    private void UpdateNormalMap(
        WorldSpaceCanvas wsc,
        Vector3 rakelPosition, float rakelRotation, float rakelTilt,
        ComputeBuffer canvasReservoir,
        RenderTexture canvasNormalMap)
    {
        RakelSnapshot rakelSnapshot = new RakelSnapshot(Length, Width, Anchor, rakelPosition, rakelRotation, rakelTilt);
        IntelGPUShaderRegion normalsSR = new IntelGPUShaderRegion(
            wsc.MapToPixelInRange(rakelSnapshot.UpperLeft),
            wsc.MapToPixelInRange(rakelSnapshot.UpperRight),
            wsc.MapToPixelInRange(rakelSnapshot.LowerLeft),
            wsc.MapToPixelInRange(rakelSnapshot.LowerRight),
            2 // Padding of 2 because normals of the previously set pixels around also have to be recalculated
        );

        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSInts2("CalculationPosition", normalsSR.CalculationPosition),
            new CSComputeBuffer("CanvasReservoir", canvasReservoir),
            new CSInts2("TextureSize", wsc.TextureSize),
            new CSTexture("NormalMap", canvasNormalMap)
        };

        ComputeShaderTask cst = new ComputeShaderTask(
            "NormalsShader",
            normalsSR,
            attributes,
            null,
            new List<ComputeBuffer>(),
            null
        );
        EnqueueOrRun(cst);
    }

    public void Dispose()
    {
        RakelApplicationReservoir.Dispose();
    }
}


