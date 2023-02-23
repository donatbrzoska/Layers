using UnityEngine;
using System.Collections.Generic;

public class Rakel : IRakel
{
    public float Length { get; private set; }
    public float Width { get; private set; }

    private Vector3 Anchor { get; }
    private int ReservoirResolution;

    private Vector2Int ReservoirSize;
    private Paint[] ApplicationReservoirData;
    private ComputeBuffer ApplicationReservoir; // 3D array, z=1 is for duplication for correct interpolation
    private Paint[] PickupReservoirData;
    private ComputeBuffer PickupReservoir; // 3D array, z=1 is for duplication for correct interpolation

    private Vector2Int PreviousApplyPosition = new Vector2Int(int.MinValue, int.MinValue);

    private ShaderRegionFactory ShaderRegionFactory;
    private Queue<ComputeShaderTask> ComputeShaderTasks;

    public Rakel(RakelConfiguration config, ShaderRegionFactory shaderRegionFactory, Queue<ComputeShaderTask> computeShaderTasks)
    {
        ReservoirResolution = config.Resolution;
        ReservoirSize.x = (int)(config.Width * config.Resolution);
        ReservoirSize.y = (int)(config.Length * config.Resolution);

        ApplicationReservoir = new ComputeBuffer(ReservoirSize.y * ReservoirSize.x * 2,
                                                      4 * sizeof(float) + sizeof(int));
        // initialize buffer to empty values (Intel does this for you, nvidia doesn't)
        ApplicationReservoirData = new Paint[ReservoirSize.y * ReservoirSize.x * 2];
        ApplicationReservoir.SetData(ApplicationReservoirData);

        PickupReservoir = new ComputeBuffer(ReservoirSize.y * ReservoirSize.x * 2,
                                                      4 * sizeof(float) + sizeof(int));
        // initialize buffer to empty values (Intel does this for you, nvidia doesn't)
        PickupReservoirData = new Paint[ReservoirSize.y * ReservoirSize.x * 2];
        PickupReservoir.SetData(PickupReservoirData);


        // make sure Rakel is not bigger than its reservoir
        float reservoirPixelSize = 1 / (float)config.Resolution;
        Length = ReservoirSize.y * reservoirPixelSize;
        Width = ReservoirSize.x * reservoirPixelSize;

        // NOTE this has to be set after Width and Length were corrected
        Anchor = new Vector3(Width, Length / 2, 0);

        ShaderRegionFactory = shaderRegionFactory;
        ComputeShaderTasks = computeShaderTasks;
    }

    public void Fill(Color_ color, int volume, ReservoirFiller filler)
    {
        filler.Fill(color, volume, ApplicationReservoirData, ReservoirSize);
        ApplicationReservoir.SetData(ApplicationReservoirData);

        //new FlatFiller().Fill(Color_.CadmiumRed, volume / 2, PickupReservoirData, ReservoirSize);
        //PickupReservoir.SetData(PickupReservoirData);
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

        ShaderRegion duplicateSR = ShaderRegionFactory.Create(
            new Vector2Int(0, ReservoirSize.y - 1),
            new Vector2Int(ReservoirSize.x - 1, ReservoirSize.y - 1),
            new Vector2Int(0, 0),
            new Vector2Int(ReservoirSize.x - 1, 0)
        );

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

        DuplicateReservoir(duplicateSR,
                           ApplicationReservoir,
                           transferConfiguration.ReservoirDiscardVolumeThreshold,
                           transferConfiguration.ReservoirSmoothingKernelSize);

        DuplicateReservoir(duplicateSR,
                           PickupReservoir,
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
            oilPaintCanvas.Reservoir);

        UpdateColorTexture(
            emitSR,
            wsc.TextureSize,
            oilPaintCanvas.Reservoir,
            oilPaintCanvas.Texture);

        UpdateNormalMap(
            normalsSR,
            wsc.TextureSize,
            oilPaintCanvas.Reservoir,
            oilPaintCanvas.NormalMap);
    }

    private void DuplicateReservoir(
        ShaderRegion shaderRegion,
        ComputeBuffer reservoir,
        int discardVolumeThreshold,
        int smoothingKernelSize,
        bool debugEnabled = false)
    {
        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSComputeBuffer("Reservoir", reservoir),
            new CSInt("DiscardVolumeThreshhold", discardVolumeThreshold),
            new CSInt("SmoothingKernelSize", smoothingKernelSize)
        };

        ComputeShaderTask cst = new ComputeShaderTask(
            "ReservoirDuplicationShader",
            shaderRegion,
            attributes,
            null,
            new List<ComputeBuffer>(),
            debugEnabled
        );

        EnqueueOrRun(cst);
    }

    private ComputeBuffer EmitFromRakel(
        RakelSnapshot rakelSnapshot,
        ShaderRegion shaderRegion,
        WorldSpaceCanvas wsc,
        TransferMapMode transferMapMode,
        bool debugEnabled = false)
    {
        ComputeBuffer RakelEmittedPaint = new ComputeBuffer(shaderRegion.CalculationSize.x * shaderRegion.CalculationSize.y,
                                                            4 * sizeof(float) + sizeof(int));
        // initialize buffer to empty values (Intel does this for you, nvidia doesn't)
        Paint[] initPaint = new Paint[shaderRegion.CalculationSize.x * shaderRegion.CalculationSize.y];
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
            new CSComputeBuffer("RakelApplicationReservoir", ApplicationReservoir),
            new CSComputeBuffer("RakelPickupReservoir", PickupReservoir),
            new CSComputeBuffer("RakelEmittedPaint", RakelEmittedPaint),
            new CSInts2("RakelReservoirSize", ReservoirSize),
            new CSInt("RakelReservoirResolution", ReservoirResolution),
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

        EnqueueOrRun(cst);

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

        EnqueueOrRun(cst);
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

        EnqueueOrRun(cst);
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

        EnqueueOrRun(cst);
    }

    public void Dispose()
    {
        ApplicationReservoir.Dispose();
        PickupReservoir.Dispose();
    }
}


