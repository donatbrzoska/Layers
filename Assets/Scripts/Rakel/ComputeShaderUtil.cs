using System.Collections.Generic;
using UnityEngine;

public class ComputeShaderUtil
{
    public static ComputeShader LoadComputeShader(string computeShaderName)
    {
        return (ComputeShader)Resources.Load(computeShaderName);
    }

    public static List<CSAttribute> GenerateReservoirRegionShaderAttributes(IntelGPUShaderRegion sr)
    {
        return new List<CSAttribute>() {
            new CSInts2("CalculationSize", sr.CalculationSize)
        };
    }

    public static List<CSAttribute> GenerateCanvasRegionShaderAttributes(
        RakelSnapshot rakelSnapshot,
        IntelGPUShaderRegion sr,
        WorldSpaceCanvas wsc,
        Rakel rakel)
    {
        return new List<CSAttribute>() {
            new CSInts2("CalculationSize", sr.CalculationSize),
            new CSInts2("CalculationPosition", sr.CalculationPosition),
            new CSInts2("TextureSize", wsc.TextureSize),
            new CSFloats3("CanvasPosition", wsc.Position),
            new CSFloats2("CanvasSize", wsc.Size),
            new CSFloats3("RakelAnchor", rakel.Anchor),
            new CSFloats3("RakelPosition", rakelSnapshot.Position),
            new CSFloat("RakelLength", rakel.Length),
            new CSFloat("RakelWidth", rakel.Width),
            new CSFloat("RakelRotation", rakelSnapshot.Rotation),
            new CSFloats2("RakelOriginBoundaries", rakelSnapshot.OriginBoundaries)
        };
    }

    public static List<CSAttribute> GenerateCopyBufferToTextureShaderAttributes(IntelGPUShaderRegion sr)
    {
        return new List<CSAttribute>() {
            new CSInts2("CalculationSize", sr.CalculationSize),
            new CSInts2("CalculationPosition", sr.CalculationPosition)
        };
    }

    public static ComputeShader GenerateCanvasRegionShader(
        string computeShaderName,
        RakelSnapshot rakelSnapshot,
        IntelGPUShaderRegion sr,
        WorldSpaceCanvas wsc,
        Rakel rakel)
    {
        ComputeShader computeShader = (ComputeShader)Resources.Load(computeShaderName);


        // Filter #1: Is the current thread even relevant or just spawned because size must be multiple of THREAD_GROUP_SIZE
        computeShader.SetInts("CalculationSize", new int[] { sr.CalculationSize.x, sr.CalculationSize.y });
        

        // Filter #2: Is the pixel belonging to the current thread underneath the rakel?
        
        // Values for pixel to world space back conversion
        computeShader.SetInts("CalculationPosition", new int[] { sr.CalculationPosition.x, sr.CalculationPosition.y }); // ... Lowest left pixel on canvas that is modified though this shader computation
        
        computeShader.SetInts("TextureSize", new int[] { wsc.TextureSize.x, wsc.TextureSize.y });
        computeShader.SetFloats("CanvasPosition", new float[] { wsc.Position.x, wsc.Position.y, wsc.Position.z });
        computeShader.SetFloats("CanvasSize", new float[] { wsc.Size.x, wsc.Size.y });

        computeShader.SetFloats("RakelAnchor", new float[] { rakel.Anchor.x, rakel.Anchor.y, rakel.Anchor.z });
        computeShader.SetFloats("RakelPosition", new float[] { rakelSnapshot.Position.x, rakelSnapshot.Position.y, rakelSnapshot.Position.z });
        computeShader.SetFloat("RakelLength", rakel.Length);
        computeShader.SetFloat("RakelWidth", rakel.Width);

        // TODO maybe use rounded boundaries for this angle
        computeShader.SetFloat("RakelRotation", rakelSnapshot.Rotation);

        // Tilted rakel boundary description
        computeShader.SetFloats("RakelOriginBoundaries", new float[] { rakelSnapshot.OriginBoundaries.x, rakelSnapshot.OriginBoundaries.y });


        return computeShader;
    }

    public static ComputeShader GenerateReservoirRegionShader(
        string computeShaderName,
        IntelGPUShaderRegion sr)
    {
        ComputeShader computeShader = (ComputeShader)Resources.Load(computeShaderName); 
    
        computeShader.SetInts("CalculationSize", new int[] { sr.CalculationSize.x, sr.CalculationSize.y });

        return computeShader;
    }

    public static ComputeShader GenerateCopyBufferToTextureShader(
        string computeShaderName,
        IntelGPUShaderRegion sr)
    {
        ComputeShader computeShader = (ComputeShader)Resources.Load(computeShaderName); 
    
        computeShader.SetInts("CalculationSize", sr.CalculationSize.x, sr.CalculationSize.y);
        computeShader.SetInts("CalculationPosition", sr.CalculationPosition.x, sr.CalculationPosition.y); // ... Lowest left pixel on canvas that is modified though this shader computation

        return computeShader;
    }

    // public void AddBuffer(ComputeShader computeShader, string bufferID, ComputeBuffer buffer)
    // {
    //     computeShader.SetBuffer(0, bufferID, buffer);
    // }
}