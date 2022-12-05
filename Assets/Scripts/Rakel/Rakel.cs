using UnityEngine;

public class Rakel : IRakel
{
    private const bool DEBUG = false;
    public Vector3 Anchor { get; private set; }
    public float Length { get; private set; } // world space
    public float Width { get; private set; } // world space
    private int Resolution; // pixels per 1 world space

    Vector2Int RakelReservoirSize;
    Paint[] RakelApplicationReservoirData;
    ComputeBuffer RakelApplicationReservoir;
    Paint[] RakelEmittedPaintData;
    ComputeBuffer RakelEmittedPaint;
    ComputeBuffer RakelPickupReservoir;
    // ComputeBuffer CanvasReservoir;

    public Rakel(float length, float width, int resolution)
    {
        Length = length;
        Width = width;
        Resolution = resolution;
        Anchor = new Vector3(Width, Length / 2, 0);

        RakelReservoirSize.y = (int)(length * resolution);
        RakelReservoirSize.x = (int)(width * resolution);
        
        RakelApplicationReservoir = new ComputeBuffer(RakelReservoirSize.y * RakelReservoirSize.x, 4 * sizeof(float) + sizeof(int)); // sizeof(Paint)
        RakelApplicationReservoirData = new Paint[RakelReservoirSize.y*RakelReservoirSize.x];
        RakelApplicationReservoir.SetData(RakelApplicationReservoirData);

        // RakelEmittedPaint = new ComputeBuffer(RakelReservoirSize.y * RakelReservoirSize.x, 3 * sizeof(float) + sizeof(int));
        // RakelEmittedPaintData = new Paint[RakelReservoirSize.y*RakelReservoirSize.x];
        // RakelEmittedPaint.SetData(RakelEmittedPaintData);

        // TODO add layers
        // CanvasReservoir = new ComputeBuffer(wsc.TextureSize.x * wsc.TextureSize.y * 3, sizeof(float));

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
        RenderTexture canvasTexture)
    {
        RakelSnapshot rakelSnapshot = new RakelSnapshot(Length, Width, Anchor, rakelPosition, rakelRotation, rakelTilt);

        IntelGPUShaderRegion sr = new IntelGPUShaderRegion(
            wsc.MapToPixelInRange(rakelSnapshot.UpperLeft),
            wsc.MapToPixelInRange(rakelSnapshot.UpperRight),
            wsc.MapToPixelInRange(rakelSnapshot.LowerLeft),
            wsc.MapToPixelInRange(rakelSnapshot.LowerRight)
        );


        // STEP 1
        // ... EMIT
        ComputeShader emitFromRakelShader = ComputeShaderUtil.GenerateCanvasRegionShader(
            "EmitFromRakelShader",
            rakelSnapshot,
            sr,
            wsc,
            this
        );
        emitFromRakelShader.SetBuffer(0, "RakelApplicationReservoir", RakelApplicationReservoir);
        Vector2Int lowerLeftRounded = wsc.MapToPixel(rakelSnapshot.LowerLeft);
        // Debug.Log("lower left roudned is " + lowerLeftRounded);
        // Debug.Log("rotation is " + rakelRotation);
        emitFromRakelShader.SetInts("RakelLowerLeftRounded", lowerLeftRounded.x, lowerLeftRounded.y);
        emitFromRakelShader.SetInts("RakelReservoirSize", RakelReservoirSize.x, RakelReservoirSize.y);
        emitFromRakelShader.SetTexture(0, "Texture", canvasTexture); // TODO move away later

        // ComputeBuffer debugBuffer;
        // Vector3[] debugValues;
        // if (true){
        //     debugBuffer = new ComputeBuffer(sr.CalculationSize.x * sr.CalculationSize.y * 3, sizeof(float));
        //     debugValues = new Vector3[sr.CalculationSize.x * sr.CalculationSize.y];
        //     debugBuffer.SetData(debugValues);
        //     emitFromRakelShader.SetBuffer(0, "Debug", debugBuffer);
        // }

        emitFromRakelShader.Dispatch(0, sr.ThreadGroups.x, sr.ThreadGroups.y, 1);

        // if (true){
        //     debugBuffer.GetData(debugValues);
        //     LogUtil.Log(debugValues, sr.CalculationSize.y, "debug");
        //     debugBuffer.Dispose();  
        // }

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
        // ComputeShader normalMapShader = (ComputeShader)Resources.Load("NormalMapShader");








        // RakelSnapshot rakelSnapshot = new RakelSnapshot(Length, Width, Anchor, rakelPosition, rakelRotation, rakelTilt);

        // IntelGPUShaderRegion sr = new IntelGPUShaderRegion(
        //     WorldSpaceCanvas.MapToPixelInRange(rakelSnapshot.UpperLeft),
        //     WorldSpaceCanvas.MapToPixelInRange(rakelSnapshot.UpperRight),
        //     WorldSpaceCanvas.MapToPixelInRange(rakelSnapshot.LowerLeft),
        //     WorldSpaceCanvas.MapToPixelInRange(rakelSnapshot.LowerRight)
        // );

        // ComputeShader applyShader = (ComputeShader)Resources.Load("ApplyShader");

        // ComputeBuffer debugBuffer;
        // Vector3[] debugValues;
        // if (DEBUG){
        //     debugBuffer = new ComputeBuffer(sr.CalculationSize.x * sr.CalculationSize.y * 3, sizeof(float));
        //     debugValues = new Vector3[sr.CalculationSize.x * sr.CalculationSize.y];
        //     debugBuffer.SetData(debugValues);
        //     applyShader.SetBuffer(0, "Debug", debugBuffer);
        // }


        // // Filter #1: Is the current thread even relevant or just spawned because size must be multiple of THREAD_GROUP_SIZE
        // applyShader.SetInts("CalculationSize", new int[] { sr.CalculationSize.x, sr.CalculationSize.y });
        

        // // Filter #2: Is the pixel belonging to the current thread underneath the rakel?
        
        // // Values for pixel to world space back conversion
        // applyShader.SetInts("CalculationPosition", new int[] { sr.CalculationPosition.x, sr.CalculationPosition.y }); // ... Lowest left pixel on canvas that is modified though this shader computation
        
        // applyShader.SetInts("TextureSize", new int[] { canvasTexture.width, canvasTexture.height });
        // applyShader.SetFloats("CanvasPosition", new float[] { canvasPosition.x, canvasPosition.y, canvasPosition.z });
        // applyShader.SetFloats("CanvasSize", new float[] { canvasSize.x, canvasSize.y });

        // applyShader.SetFloats("RakelAnchor", new float[] { Anchor.x, Anchor.y, Anchor.z });
        // applyShader.SetFloats("RakelPosition", new float[] { rakelPosition.x, rakelPosition.y, rakelPosition.z });
        // applyShader.SetFloat("RakelLength", Length);
        // applyShader.SetFloat("RakelWidth", Width);

        // // Vector3 orientationReference = ulRotated - llRotated;
        // // float angle = MathUtil.Angle360(Vector2.up, new Vector2(orientationReference.x, orientationReference.y));
        // // TODO maybe use rounded boundaries for this angle
        // applyShader.SetFloat("RakelRotation", rakelRotation);

        // // Tilted rakel boundary description
        // applyShader.SetFloats("RakelOriginBoundaries", new float[] { rakelSnapshot.OriginBoundaries.x, rakelSnapshot.OriginBoundaries.y });



        // applyShader.SetTexture(0, "Texture", canvasTexture);
        // applyShader.Dispatch(0, sr.ThreadGroups.x, sr.ThreadGroups.y, 1);




        // if (DEBUG){
        //     debugBuffer.GetData(debugValues);
        //     LogUtil.Log(debugValues, sr.CalculationSize.y, "debug");
        //     debugBuffer.Dispose();  
        // }
    }

    public void Dispose()
    {
        RakelApplicationReservoir.Dispose();
    }
}
