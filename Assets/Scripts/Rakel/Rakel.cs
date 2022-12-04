using UnityEngine;

public class Rakel : IRakel
{
    private const bool DEBUG = false;
    public Vector3 Anchor { get; private set; }
    public float Length { get; private set; } // world space
    public float Width { get; private set; } // world space
    private int Resolution; // pixels per 1 world space
    private WorldSpaceCanvas WorldSpaceCanvas;

    Vector2Int RakelReservoirSize;
    Paint[] RakelApplicationReservoirData;
    ComputeBuffer RakelApplicationReservoir;
    Paint[] RakelEmittedPaintData;
    ComputeBuffer RakelEmittedPaint;
    ComputeBuffer RakelPickupReservoir;
    // ComputeBuffer CanvasReservoir;

    public Rakel(float length, float width, int resolution, WorldSpaceCanvas wsc)
    {
        Length = length;
        Width = width;
        Resolution = resolution;
        WorldSpaceCanvas = wsc;
        Anchor = new Vector3(Width, Length / 2, 0);

        RakelReservoirSize.y = (int)(length * resolution);
        RakelReservoirSize.x = (int)(width * resolution);
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
        Vector3 canvasPosition,
        Vector2 canvasSize,
        RenderTexture canvasTexture)
    {
        RakelSnapshot rakelSnapshot = new RakelSnapshot(Length, Width, Anchor, rakelPosition, rakelRotation, rakelTilt);

        IntelGPUShaderRegion sr = new IntelGPUShaderRegion(
            WorldSpaceCanvas.MapToPixelInRange(rakelSnapshot.UpperLeft),
            WorldSpaceCanvas.MapToPixelInRange(rakelSnapshot.UpperRight),
            WorldSpaceCanvas.MapToPixelInRange(rakelSnapshot.LowerLeft),
            WorldSpaceCanvas.MapToPixelInRange(rakelSnapshot.LowerRight)
        );

        ComputeShader applyShader = (ComputeShader)Resources.Load("ApplyShader");

        ComputeBuffer debugBuffer;
        Vector3[] debugValues;
        if (DEBUG){
            debugBuffer = new ComputeBuffer(sr.CalculationSize.x * sr.CalculationSize.y * 3, sizeof(float));
            debugValues = new Vector3[sr.CalculationSize.x * sr.CalculationSize.y];
            debugBuffer.SetData(debugValues);
            applyShader.SetBuffer(0, "Debug", debugBuffer);
        }


        // Filter #1: Is the current thread even relevant or just spawned because size must be multiple of THREAD_GROUP_SIZE
        applyShader.SetInts("CalculationSize", new int[] { sr.CalculationSize.x, sr.CalculationSize.y });
        
        
        // Filter #2: Is the pixel belonging to the current thread underneath the rakel?
        CanvasPixelMappingInfo[] canvasPixelMappingInfoData = new CanvasPixelMappingInfo[] {
            new CanvasPixelMappingInfo(
                sr.CalculationPosition,
                new Vector2Int(canvasTexture.width, canvasTexture.height),
                canvasPosition,
                canvasSize,
                Anchor,
                rakelPosition,
                Length,
                Width,
                rakelRotation,
                rakelSnapshot.OriginBoundaries
            )
        };
        ComputeBuffer canvasPixelMappingInfo = new ComputeBuffer(1, 4 * 20 + 4 * 4);
        canvasPixelMappingInfo.SetData(canvasPixelMappingInfoData);
        applyShader.SetBuffer(0, "CanvasPixelMappingInfoBuffer", canvasPixelMappingInfo);


        applyShader.SetTexture(0, "Texture", canvasTexture);
        applyShader.Dispatch(0, sr.ThreadGroups.x, sr.ThreadGroups.y, 1);




        if (DEBUG){
            debugBuffer.GetData(debugValues);
            LogUtil.Log(debugValues, sr.CalculationSize.y, "debug");
            debugBuffer.Dispose();  
        }

        canvasPixelMappingInfo.Dispose();
    }
}
