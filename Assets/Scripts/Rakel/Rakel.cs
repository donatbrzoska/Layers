using UnityEngine;

public class Rakel : IRakel
{
    private const bool DEBUG = false;
    public Vector3 Anchor { get; private set; }
    public float Length { get; private set; } // world space
    public float Width { get; private set; } // world space
    private int Resolution; // pixels per 1 world space
    private WorldSpaceCanvas WorldSpaceCanvas;

    public Rakel(float length, float width, int resolution, WorldSpaceCanvas wsc)
    {
        Length = length;
        Width = width;
        Resolution = resolution;
        WorldSpaceCanvas = wsc;
        Anchor = new Vector3(Width, Length / 2, 0);
    }

    // Position is located halfway through the rakel, at the handle
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
            debugBuffer = new ComputeBuffer(sr.CalculationSizeX * sr.CalculationSizeY * 3, sizeof(float));
            debugValues = new Vector3[sr.CalculationSizeX * sr.CalculationSizeY];
            debugBuffer.SetData(debugValues);
            applyShader.SetBuffer(0, "Debug", debugBuffer);
        }


        // Filter #1: Is the current thread even relevant or just spawned because size must be multiple of THREAD_GROUP_SIZE
        applyShader.SetInts("CalculationSize", new int[] { sr.CalculationSizeX, sr.CalculationSizeY }); ;


        // Filter #2: Is the pixel belonging to the current thread underneath the rakel?
        
        // Values for pixel to world space back conversion
        applyShader.SetInts("CalculationPosition", new int[] { sr.CalculationPositionX, sr.CalculationPositionY }); // ... Lowest left pixel on canvas that is modified though this shader computation
        
        applyShader.SetInts("TextureSize", new int[] { canvasTexture.width, canvasTexture.height });
        applyShader.SetFloats("CanvasPosition", new float[] { canvasPosition.x, canvasPosition.y, canvasPosition.z });
        applyShader.SetFloats("CanvasSize", new float[] { canvasSize.x, canvasSize.y });

        applyShader.SetFloats("RakelAnchor", new float[] { Anchor.x, Anchor.y, Anchor.z });
        applyShader.SetFloats("RakelPosition", new float[] { rakelPosition.x, rakelPosition.y, rakelPosition.z });
        applyShader.SetFloat("RakelLength", Length);
        applyShader.SetFloat("RakelWidth", Width);

        // Vector3 orientationReference = ulRotated - llRotated;
        // float angle = MathUtil.Angle360(Vector2.up, new Vector2(orientationReference.x, orientationReference.y));
        // TODO maybe use rounded boundaries for this angle
        applyShader.SetFloat("RakelRotation", rakelRotation);

        // Tilted rakel boundary description
        applyShader.SetFloats("RakelOriginBoundaries", new float[] { rakelSnapshot.OriginBoundaries.x, rakelSnapshot.OriginBoundaries.y });



        applyShader.SetTexture(0, "Texture", canvasTexture);
        applyShader.Dispatch(0, sr.ThreadGroupsX, sr.ThreadGroupsY, 1);




        if (DEBUG){
            debugBuffer.GetData(debugValues);
            LogUtil.Log(debugValues, sr.CalculationSizeY, "debug");
            debugBuffer.Dispose();  
        }
    }
}
