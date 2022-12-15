using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class Rakel : IRakel
{
    private const bool DEBUG = false;
    public Vector3 Anchor { get; private set; }
    public float Length { get; private set; } // world space
    public float Width { get; private set; } // world space
    private int Resolution; // pixels per 1 world space

    Vector2Int RakelReservoirSize;
    Paint[] RakelApplicationReservoirData;
    ComputeBuffer RakelApplicationReservoir; // 3D array, z=1 is for duplication for correct interpolation
    // Paint[] RakelEmittedPaintData;
    //ComputeBuffer RakelPickupReservoir;
    ComputeBuffer Canvas;

    Queue<ComputeShaderTask> ComputeShaderTasks;

    public Rakel(float length, float width, int resolution, Queue<ComputeShaderTask> computeShaderTasks)
    {
        Length = length;
        Width = width;
        Resolution = resolution;
        Anchor = new Vector3(Width, Length / 2, 0);

        RakelReservoirSize.y = (int)(length * resolution);
        RakelReservoirSize.x = (int)(width * resolution);

        ComputeShaderTasks = computeShaderTasks;
        
        RakelApplicationReservoir = new ComputeBuffer(RakelReservoirSize.y * RakelReservoirSize.x * 2, 4 * sizeof(float) + sizeof(int)); // sizeof(Paint)
        RakelApplicationReservoirData = new Paint[RakelReservoirSize.y * RakelReservoirSize.x * 2];
        RakelApplicationReservoir.SetData(RakelApplicationReservoirData);

        // RakelEmittedPaintData = new Paint[RakelReservoirSize.y*RakelReservoirSize.x];
        // RakelEmittedPaint.SetData(RakelEmittedPaintData);

        // TODO add layers
         //Canvas = new ComputeBuffer(wsc.TextureSize.x * wsc.TextureSize.y * 3, sizeof(float));

    }

    public void Fill(Paint paint, ReservoirFiller filler)
    {
        filler.Fill(paint, RakelApplicationReservoirData, RakelReservoirSize);
        RakelApplicationReservoir.SetData(RakelApplicationReservoirData);
    }

    // Position is located at Anchor
    // Rotation 0 means Rakel is directed to the right
    // Tilt 0 means Rakel is flat on canvas
    public void Apply(
        Vector3 rakelPosition,
        float rakelRotation,
        float rakelTilt,
        WorldSpaceCanvas wsc,
        ComputeBuffer Canvas,
        RenderTexture canvasTexture,
        RenderTexture canvasNormalMap)
    {
        // STEP 1
        // ... EMIT: Duplicate Reservoir
        IntelGPUShaderRegion duplicateSR = new IntelGPUShaderRegion(
            new Vector2Int(0, RakelReservoirSize.y),
            new Vector2Int(RakelReservoirSize.x, RakelReservoirSize.y),
            new Vector2Int(0, 0),
            new Vector2Int(RakelReservoirSize.x, 0)
        );

        ComputeShader reservoirDuplicationShader = ComputeShaderUtil.LoadComputeShader("ReservoirDuplicationShader");

        List<CSAttribute> attributes = ComputeShaderUtil.GenerateReservoirRegionShaderAttributes(duplicateSR);
        attributes.Add(new CSComputeBuffer("Reservoir", RakelApplicationReservoir));
        //ComputeBuffer finishedBuffer = new ComputeBuffer(1, sizeof(int));
        //attributes.Add(new CSComputeBuffer("Finished", finishedBuffer));

        ComputeShaderTask cst = new ComputeShaderTask(
            reservoirDuplicationShader,
            attributes,
            duplicateSR.ThreadGroups,
            //finishedBuffer,
            null,
            new List<ComputeBuffer>()
        );
        ComputeShaderTasks.Enqueue(cst);


        // ... EMIT: Extract interpolated volumes and resulting color from duplicate and delete from original
        RakelSnapshot rakelSnapshot = new RakelSnapshot(Length, Width, Anchor, rakelPosition, rakelRotation, rakelTilt);
        IntelGPUShaderRegion emitSR = new IntelGPUShaderRegion(
            wsc.MapToPixelInRange(rakelSnapshot.UpperLeft),
            wsc.MapToPixelInRange(rakelSnapshot.UpperRight),
            wsc.MapToPixelInRange(rakelSnapshot.LowerLeft),
            wsc.MapToPixelInRange(rakelSnapshot.LowerRight)
        );

        ComputeShader emitFromRakelShader = ComputeShaderUtil.LoadComputeShader("EmitFromRakelShader");
        attributes = ComputeShaderUtil.GenerateCanvasRegionShaderAttributes(
            rakelSnapshot,
            emitSR,
            wsc,
            this
        );

        attributes.Add(new CSComputeBuffer("RakelApplicationReservoir", RakelApplicationReservoir));
        ComputeBuffer RakelEmittedPaint = new ComputeBuffer(emitSR.CalculationSize.x * emitSR.CalculationSize.y, 4 * sizeof(float) + sizeof(int));
        attributes.Add(new CSComputeBuffer("RakelEmittedPaint", RakelEmittedPaint));
        Vector2Int lowerLeftRounded = wsc.MapToPixel(rakelSnapshot.LowerLeft);
        attributes.Add(new CSInts2("RakelLowerLeftRounded", lowerLeftRounded));
        attributes.Add(new CSInts2("RakelReservoirSize", RakelReservoirSize));
        //finishedBuffer = new ComputeBuffer(1, sizeof(int));
        //attributes.Add(new CSComputeBuffer("Finished", finishedBuffer));

        cst = new ComputeShaderTask(
            emitFromRakelShader,
            attributes,
            emitSR.ThreadGroups,
            //finishedBuffer,
            null,
            new List<ComputeBuffer>()
        );
        ComputeShaderTasks.Enqueue(cst);




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
        ComputeShader copyBufferToTextureShader = ComputeShaderUtil.LoadComputeShader("CopyBufferToCanvasShader");
        attributes = ComputeShaderUtil.GenerateCopyBufferToCanvasShaderAttributes(emitSR);
        attributes.Add(new CSComputeBuffer("RakelEmittedPaint", RakelEmittedPaint));
        attributes.Add(new CSComputeBuffer("Canvas", Canvas));
        attributes.Add(new CSInt("CanvasWidth", canvasTexture.width));
        //finishedBuffer = new ComputeBuffer(1, sizeof(int));
        //attributes.Add(new CSComputeBuffer("Finished", finishedBuffer));
        cst = new ComputeShaderTask(
            copyBufferToTextureShader,
            attributes,
            emitSR.ThreadGroups,
            //finishedBuffer,
            null,
            new List<ComputeBuffer>() { RakelEmittedPaint }
        );
        ComputeShaderTasks.Enqueue(cst);



        // STEP 4.1
        // ... COLORS
        ComputeShader colorsShader = ComputeShaderUtil.LoadComputeShader("ColorsShader");
        attributes = ComputeShaderUtil.GenerateCopyBufferToCanvasShaderAttributes(emitSR);
        attributes.Add(new CSComputeBuffer("Canvas", Canvas));
        attributes.Add(new CSInts2("CanvasSize", wsc.TextureSize));
        attributes.Add(new CSTexture("Texture", canvasTexture));
        //finishedBuffer = new ComputeBuffer(1, sizeof(int));
        //attributes.Add(new CSComputeBuffer("Finished", finishedBuffer));
        cst = new ComputeShaderTask(
            colorsShader,
            attributes,
            emitSR.ThreadGroups,
            //finishedBuffer,
            null,
            new List<ComputeBuffer>()
        );
        ComputeShaderTasks.Enqueue(cst);


        // STEP 4.2
        // ... NORMALS
        IntelGPUShaderRegion paddedEmitSR = new IntelGPUShaderRegion(
            wsc.MapToPixelInRange(rakelSnapshot.UpperLeft),
            wsc.MapToPixelInRange(rakelSnapshot.UpperRight),
            wsc.MapToPixelInRange(rakelSnapshot.LowerLeft),
            wsc.MapToPixelInRange(rakelSnapshot.LowerRight),
            1 // Padding because normals of the pixels around also have to be recalculated
        );

        ComputeShader normalsShader = ComputeShaderUtil.LoadComputeShader("NormalsShader");
        attributes = ComputeShaderUtil.GenerateCopyBufferToCanvasShaderAttributes(paddedEmitSR);
        attributes.Add(new CSComputeBuffer("Canvas", Canvas));
        attributes.Add(new CSInts2("CanvasSize", wsc.TextureSize));
        attributes.Add(new CSTexture("NormalMap", canvasNormalMap));
        //finishedBuffer = new ComputeBuffer(1, sizeof(int));
        //attributes.Add(new CSComputeBuffer("Finished", finishedBuffer));
        cst = new ComputeShaderTask(
            normalsShader,
            attributes,
            paddedEmitSR.ThreadGroups,
            //finishedBuffer,
            null,
            new List<ComputeBuffer>()
        );
        ComputeShaderTasks.Enqueue(cst);
    }

    public void Dispose()
    {
        RakelApplicationReservoir.Dispose();
    }
}



// ComputeBuffer debugBuffer;
// Vector3[] debugValues;
// if (true){
//     debugBuffer = new ComputeBuffer(emitSR.CalculationSize.x * emitSR.CalculationSize.y * 3, sizeof(float));
//     debugValues = new Vector3[emitSR.CalculationSize.x * emitSR.CalculationSize.y];
//     debugBuffer.SetData(debugValues);
//     emitFromRakelShader.SetBuffer(0, "Debug", debugBuffer);
// }


// if (true){
//     debugBuffer.GetData(debugValues);
//     LogUtil.Log(debugValues, emitSR.CalculationSize.y, "debug");
//     debugBuffer.Dispose();  
// }