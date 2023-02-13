using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class Rakel : IRakel
{
    private const bool DEBUG = false;

    public Vector3 Anchor { get; private set; }
    public float Length { get; private set; } // world space
    public float Width { get; private set; } // world space
    private int ReservoirResolution; // pixels per 1 world space

    Vector2Int RakelReservoirSize;
    Paint[] RakelApplicationReservoirData;
    ComputeBuffer RakelApplicationReservoir; // 3D array, z=1 is for duplication for correct interpolation
    // Paint[] RakelEmittedPaintData;
    //ComputeBuffer RakelPickupReservoir;
    ComputeBuffer Canvas;

    Queue<ComputeShaderTask> ComputeShaderTasks;

    Vector2Int PreviousApplyPosition = new Vector2Int(int.MinValue, int.MinValue);
    Vector3 PreviousApplyPosition_ = new Vector3(int.MinValue, int.MinValue);

    public Rakel(RakelConfiguration config, Queue<ComputeShaderTask> computeShaderTasks)
    {
        RakelReservoirSize.x = (int)(config.Width * config.Resolution);
        RakelReservoirSize.y = (int)(config.Length * config.Resolution);

        float reservoirPixelSize = 1 / (float)config.Resolution;
        Length = RakelReservoirSize.y * reservoirPixelSize; // make sure Rakel is not bigger than its reservoir
        Width = RakelReservoirSize.x * reservoirPixelSize; // make sure Rakel is not bigger than its reservoir
        ReservoirResolution = config.Resolution;
        Anchor = new Vector3(Width, Length / 2, 0);

        ComputeShaderTasks = computeShaderTasks;
        
        RakelApplicationReservoir = new ComputeBuffer(RakelReservoirSize.y * RakelReservoirSize.x * 2, 4 * sizeof(float) + sizeof(int)); // sizeof(Paint)
        RakelApplicationReservoirData = new Paint[RakelReservoirSize.y * RakelReservoirSize.x * 2];
        RakelApplicationReservoir.SetData(RakelApplicationReservoirData);

        // RakelEmittedPaintData = new Paint[RakelReservoirSize.y*RakelReservoirSize.x];
        // RakelEmittedPaint.SetData(RakelEmittedPaintData);

        // TODO add layers
         //Canvas = new ComputeBuffer(wsc.TextureSize.x * wsc.TextureSize.y * 3, sizeof(float));

    }

    public void Fill(_Color color, int volume, ReservoirFiller filler)
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
        Vector3 rakelPosition,
        float rakelRotation,
        float rakelTilt,
        TransferConfiguration transferConfiguration,
        OilPaintCanvas oilPaintCanvas)
    {
        WorldSpaceCanvas wsc = oilPaintCanvas.WorldSpaceCanvas;

        rakelPosition = wsc.AlignToPixelGrid(rakelPosition);// + new Vector3(0.2f, 0.2f, 0);
        // this is needed to prevent double application on the same pixel
        if (wsc.MapToPixel(rakelPosition).Equals(PreviousApplyPosition))
        {
            return;
        }
        else
        {
            PreviousApplyPosition = wsc.MapToPixel(rakelPosition);
        }

        // this is needed to prevent double application on the same pixel
        //float pixelSize = 1 / (float)Resolution;
        //float requiredDistanceBetweenApplications = 1 * pixelSize;
        //if ((rakelPosition - PreviousApplyPosition_).magnitude < requiredDistanceBetweenApplications)
        //{
        //    return;
        //}
        //else
        //{
        //    PreviousApplyPosition_ = rakelPosition;
        //}

        //Debug.Log("Applying at x=" + wsc.MapToPixel(rakelPosition));

        // STEP 1
        // ... EMIT: Duplicate Reservoir and smooth it while we are at it
        IntelGPUShaderRegion duplicateSR = new IntelGPUShaderRegion(
            new Vector2Int(0, RakelReservoirSize.y-1),
            new Vector2Int(RakelReservoirSize.x-1, RakelReservoirSize.y-1),
            new Vector2Int(0, 0),
            new Vector2Int(RakelReservoirSize.x-1, 0)
        );

        ComputeShader reservoirDuplicationShader = ComputeShaderUtil.LoadComputeShader("ReservoirDuplicationShader");

        List<CSAttribute> attributes = ComputeShaderUtil.GenerateReservoirRegionShaderAttributes(duplicateSR);
        attributes.Add(new CSComputeBuffer("Reservoir", RakelApplicationReservoir));
        attributes.Add(new CSInt("DiscardVolumeThreshhold", transferConfiguration.ReservoirDiscardVolumeThreshold));
        attributes.Add(new CSInt("SmoothingKernelSize", transferConfiguration.ReservoirSmoothingKernelSize));
        //ComputeBuffer finishedBuffer = new ComputeBuffer(1, sizeof(int));
        //attributes.Add(new CSComputeBuffer("Finished", finishedBuffer));

        ComputeShaderTask cst = new ComputeShaderTask(
            "ReservoirDuplicationShader",
            reservoirDuplicationShader,
            attributes,
            duplicateSR.ThreadGroups,
            //finishedBuffer,
            null,
            new List<ComputeBuffer>(),
            null
            //new List<int>() { duplicateSR.CalculationSize.x, duplicateSR.CalculationSize.y }
        );
        EnqueueOrRun(cst);

        // ... EMIT: Calculate interpolated volumes and resulting color from duplicate and delete from original
        RakelSnapshot rakelSnapshot = new RakelSnapshot(Length, Width, Anchor, rakelPosition, rakelRotation, rakelTilt);

        IntelGPUShaderRegion emitSR = new IntelGPUShaderRegion(
            wsc.MapToPixelInRange(rakelSnapshot.UpperLeft),
            wsc.MapToPixelInRange(rakelSnapshot.UpperRight),
            wsc.MapToPixelInRange(rakelSnapshot.LowerLeft),
            wsc.MapToPixelInRange(rakelSnapshot.LowerRight),
            1
        );
        //Debug.Log("ul " + wsc.MapToPixelInRange(rakelSnapshot.UpperLeft));
        //Debug.Log("ur " + wsc.MapToPixelInRange(rakelSnapshot.UpperRight));
        //Debug.Log("ll " + wsc.MapToPixelInRange(rakelSnapshot.LowerLeft));
        //Debug.Log("lr " + wsc.MapToPixelInRange(rakelSnapshot.LowerRight));

        ComputeShader emitFromRakelShader = ComputeShaderUtil.LoadComputeShader("EmitFromRakelShader");
        attributes = ComputeShaderUtil.GenerateCanvasRegionShaderAttributes(
            rakelSnapshot,
            emitSR,
            wsc,
            this
        );

        attributes.Add(new CSComputeBuffer("RakelApplicationReservoir", RakelApplicationReservoir));
        ComputeBuffer RakelEmittedPaint = new ComputeBuffer(emitSR.CalculationSize.x * emitSR.CalculationSize.y, 4 * sizeof(float) + sizeof(int));
        Paint[] initPaint = new Paint[emitSR.CalculationSize.x * emitSR.CalculationSize.y];
        RakelEmittedPaint.SetData(initPaint);
        attributes.Add(new CSComputeBuffer("RakelEmittedPaint", RakelEmittedPaint));
        attributes.Add(new CSInts2("RakelReservoirSize", RakelReservoirSize));
        attributes.Add(new CSInt("RakelReservoirResolution", ReservoirResolution));
        attributes.Add(new CSInt("TransferMapMode", (int)transferConfiguration.MapMode));

        cst = new ComputeShaderTask(
            "EmitFromRakelShader",
            emitFromRakelShader,
            attributes,
            emitSR.ThreadGroups,
            null,
            new List<ComputeBuffer>(),
            null
            //new List<int>() { emitSR.CalculationSize.x, emitSR.CalculationSize.y }
        );
        EnqueueOrRun(cst);

        //RakelApplicationReservoir.GetData(RakelApplicationReservoirData);
        //LogUtil.Log(RakelApplicationReservoirData, RakelReservoirSize.y);





        // ... PICKUP
        // ComputeShader pickupFromCanvasShader = (ComputeShader)Resources.Load("PickupFromCanvasShader");



        // STEP 1.1
        // ... SMOOTH RAKEL RESERVOIR
        // ComputeShader smoothRakelShader = (ComputeShader)Resources.Load("SmoothRakelShader");
        // ComputeShader smoothCanvasShader = (ComputeShader)Resources.Load("SmoothCanvasShader");

        // STEP 2
        // ComputeShader emitToCanvasShader = (ComputeShader)Resources.Load("EmitToCanvasShader");
        // ComputeShader pickupToRakelShader = (ComputeShader)Resources.Load("PickupToRakelShader");


        // STEP 3
        // ... PUT TO CANVAS
        ComputeShader copyBufferToCanvasShader = ComputeShaderUtil.LoadComputeShader("CopyBufferToCanvasShader");
        attributes = ComputeShaderUtil.GenerateCopyBufferToCanvasShaderAttributes(emitSR);
        attributes.Add(new CSComputeBuffer("RakelEmittedPaint", RakelEmittedPaint));
        attributes.Add(new CSComputeBuffer("Canvas", oilPaintCanvas.Reservoir));
        attributes.Add(new CSInt("CanvasWidth", oilPaintCanvas.Texture.width));
        cst = new ComputeShaderTask(
            "CopyBufferToCanvasShader",
            copyBufferToCanvasShader,
            attributes,
            emitSR.ThreadGroups,
            null,
            new List<ComputeBuffer>() { RakelEmittedPaint },
            null
        );
        EnqueueOrRun(cst);



        // STEP 4.1
        // ... COLORS
        ComputeShader colorsShader = ComputeShaderUtil.LoadComputeShader("ColorsShader");
        attributes = ComputeShaderUtil.GenerateCopyBufferToCanvasShaderAttributes(emitSR);
        attributes.Add(new CSComputeBuffer("Canvas", oilPaintCanvas.Reservoir));
        attributes.Add(new CSInts2("CanvasSize", wsc.TextureSize));
        attributes.Add(new CSTexture("Texture", oilPaintCanvas.Texture));
        cst = new ComputeShaderTask(
            "ColorsShader",
            colorsShader,
            attributes,
            emitSR.ThreadGroups,
            null,
            new List<ComputeBuffer>(),
            null
        );
        EnqueueOrRun(cst);


        // STEP 4.2
        // ... NORMALS
        IntelGPUShaderRegion paddedEmitSR = new IntelGPUShaderRegion(
            wsc.MapToPixelInRange(rakelSnapshot.UpperLeft),
            wsc.MapToPixelInRange(rakelSnapshot.UpperRight),
            wsc.MapToPixelInRange(rakelSnapshot.LowerLeft),
            wsc.MapToPixelInRange(rakelSnapshot.LowerRight),
            // TODO Padding 2, because emit already had padding 1?
            1 // Padding because normals of the pixels around also have to be recalculated
        );

        ComputeShader normalsShader = ComputeShaderUtil.LoadComputeShader("NormalsShader");
        attributes = ComputeShaderUtil.GenerateCopyBufferToCanvasShaderAttributes(paddedEmitSR);
        attributes.Add(new CSComputeBuffer("Canvas", oilPaintCanvas.Reservoir));
        attributes.Add(new CSInts2("CanvasSize", wsc.TextureSize));
        attributes.Add(new CSTexture("NormalMap", oilPaintCanvas.NormalMap));
        cst = new ComputeShaderTask(
            "NormalsShader",
            normalsShader,
            attributes,
            paddedEmitSR.ThreadGroups,
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


