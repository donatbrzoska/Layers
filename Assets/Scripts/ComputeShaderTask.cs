using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public abstract class CSAttribute
{
    protected string Key;

    public CSAttribute(string key)
    {
        Key = key;
    }

    public abstract void ApplyTo(ComputeShader computeShader);
}

public class CSComputeBuffer : CSAttribute
{
    private ComputeBuffer ComputeBuffer;

    public CSComputeBuffer(string key, ComputeBuffer computeBuffer) : base(key)
    {
        ComputeBuffer = computeBuffer;
    }

    public override void ApplyTo(ComputeShader computeShader)
    {
        computeShader.SetBuffer(0, Key, ComputeBuffer);
    }
}

public class CSTexture : CSAttribute
{
    private RenderTexture RenderTexture;

    public CSTexture(string key, RenderTexture renderTexture) : base(key)
    {
        RenderTexture = renderTexture;
    }

    public override void ApplyTo(ComputeShader computeShader)
    {
        computeShader.SetTexture(0, Key, RenderTexture);
    }
}

public class CSInt : CSAttribute
{
    private int Value;

    public CSInt(string key, int value) : base(key)
    {
        Value = value;
    }

    public override void ApplyTo(ComputeShader computeShader)
    {
        computeShader.SetInt(Key, Value);
    }
}

public class CSInts2 : CSAttribute
{
    private Vector2Int Values;

    public CSInts2(string key, Vector2Int values) : base(key)
    {
        Values = values;
    }

    public override void ApplyTo(ComputeShader computeShader)
    {
        computeShader.SetInts(Key, Values.x, Values.y);
    }
}

public class CSFloat : CSAttribute
{
    private float Value;

    public CSFloat(string key, float value) : base(key)
    {
        Value = value;
    }

    public override void ApplyTo(ComputeShader computeShader)
    {
        computeShader.SetFloat(Key, Value);
    }
}

public class CSFloats2 : CSAttribute
{
    private Vector2 Values;

    public CSFloats2(string key, Vector2 values) : base(key)
    {
        Values = values;
    }

    public override void ApplyTo(ComputeShader computeShader)
    {
        computeShader.SetFloats(Key, Values.x, Values.y);
    }
}

public class CSFloats3 : CSAttribute
{
    private Vector3 Values;

    public CSFloats3(string key, Vector3 values) : base(key)
    {
        Values = values;
    }

    public override void ApplyTo(ComputeShader computeShader)
    {
        computeShader.SetFloats(Key, Values.x, Values.y, Values.z);
    }
}

public class CSFloats4 : CSAttribute
{
    private Vector4 Values;

    public CSFloats4(string key, Vector4 values) : base(key)
    {
        Values = values;
    }

    public override void ApplyTo(ComputeShader computeShader)
    {
        computeShader.SetFloats(Key, Values.x, Values.y, Values.z, Values.w);
    }
}

public struct ComputeShaderTask {
    public string Name;
    public ComputeShader ComputeShader;
    public List<CSAttribute> Attributes;
    public Vector2Int ThreadGroups;
    public ComputeBuffer FinishedMarkerBuffer;
    public List<ComputeBuffer> BuffersToDispose;
    public List<int> DebugBufferSize;

    public ComputeShaderTask(
        string name,
        ComputeShader computeShader,
        List<CSAttribute> attributes,
        Vector2Int threadGroups,
        ComputeBuffer finishedMarkerBuffer,
        List<ComputeBuffer> buffersToDispose,
        List<int> debugBufferSize)
    {
        Name = name;
        ComputeShader = computeShader;
        Attributes = attributes;
        ThreadGroups = threadGroups;
        FinishedMarkerBuffer = finishedMarkerBuffer;
        BuffersToDispose = buffersToDispose;
        DebugBufferSize = debugBufferSize;
    }

    public void Run()
    {
        foreach (CSAttribute ca in Attributes)
        {
            ca.ApplyTo(ComputeShader);
        }


        ComputeBuffer debugBuffer = new ComputeBuffer(1, sizeof(float)); // just for C#
        //float[] debugValues = new float[1]; // just for C#
        //int[] debugValues = new int[1]; // just for C#
        Vector2[] debugValues = new Vector2[1]; // just for C#
        //Vector2Int[] debugValues = new Vector2Int[1]; // just for C#
        //Vector3[] debugValues = new Vector3[1]; // just for C#
        //Color[] debugValues = new Color[1]; // just for C#
        if (DebugBufferSize != null)
        {
            debugBuffer.Dispose();
            debugBuffer = new ComputeBuffer(DebugBufferSize[0] * DebugBufferSize[1] * 4, sizeof(float));
            //debugValues = new float[DebugBufferSize[0] * DebugBufferSize[1]];
            //debugValues = new int[DebugBufferSize[0] * DebugBufferSize[1]];
            debugValues = new Vector2[DebugBufferSize[0] * DebugBufferSize[1]];
            //debugValues = new Vector2Int[DebugBufferSize[0] * DebugBufferSize[1]];
            //debugValues = new Vector3[DebugBufferSize[0] * DebugBufferSize[1]];
            //debugValues = new Color[DebugBufferSize[0] * DebugBufferSize[1]];
            debugBuffer.SetData(debugValues);
            ComputeShader.SetBuffer(0, "Debug", debugBuffer);
        }


        if (FinishedMarkerBuffer != null)
        {
            ComputeShader.SetBuffer(0, "Finished", FinishedMarkerBuffer);
        }

        // The problem with AsyncGPUReadback is that .done is probably set in the next frame,
        // .. so we cannot use this to run multiple dispatches during one frame
        //CurrentReadbackRequest = AsyncGPUReadback.Request(cst.FinishedMarkerBuffer);

        ComputeShader.Dispatch(0, ThreadGroups.x, ThreadGroups.y, 1);

        GL.Flush();

        // Alternative but slow: GetData() blocks until the task is finished
        if (FinishedMarkerBuffer != null)
        {
            FinishedMarkerBuffer.GetData(new int[1]);
            FinishedMarkerBuffer.Dispose();
        }

        //while (!CurrentReadbackRequest.done)
        //{
        //    Thread.Sleep(1);
        //}


        if (DebugBufferSize != null)
        {
            debugBuffer.GetData(debugValues);
            LogUtil.Log(debugValues, DebugBufferSize[1], "debug");

            //int sum = 0;
            //for (int i = 0; i < debugValues.GetLength(0); i++)
            //{
            //    sum += (int)debugValues[i].x;
            //}
            //Debug.Log("Sum is " + sum);
        }
        debugBuffer.Dispose();


        foreach (ComputeBuffer c in BuffersToDispose)
        {
            c.Dispose();
        }
    }
}