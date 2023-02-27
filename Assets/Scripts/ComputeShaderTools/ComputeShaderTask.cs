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

public class ComputeShaderTask {
    public string Name;
    private ShaderRegion ShaderRegion;
    public ComputeShader ComputeShader;
    public List<CSAttribute> Attributes;
    public Vector2Int ThreadGroups;
    public ComputeBuffer FinishedMarkerBuffer;
    public List<ComputeBuffer> BuffersToDispose;
    public bool DebugEnabled;

    public ComputeShaderTask(
        string name,
        ShaderRegion shaderRegion,
        List<CSAttribute> attributes,
        ComputeBuffer finishedMarkerBuffer,
        List<ComputeBuffer> buffersToDispose,
        bool debugEnabled)
    {
        Name = name;
        ShaderRegion = shaderRegion;
        Attributes = attributes;
        FinishedMarkerBuffer = finishedMarkerBuffer;
        BuffersToDispose = buffersToDispose;
        DebugEnabled = debugEnabled;

        ComputeShader = (ComputeShader)Resources.Load(name);
        ThreadGroups = shaderRegion.ThreadGroups;
        Attributes.Add(new CSInts2("CalculationSize", shaderRegion.CalculationSize));
    }

    public void Run()
    {
        //Debug.Log("Processing " + Name);
        foreach (CSAttribute ca in Attributes)
        {
            //Debug.Log("Processing " + ca);
            ca.ApplyTo(ComputeShader);
        }


        ComputeBuffer debugBuffer = new ComputeBuffer(1, sizeof(float)); // just for C#
        Color[] debugValues = new Color[1]; // just for C#
        ComputeBuffer debugTypeBuffer = new ComputeBuffer(1, sizeof(int));
        int[] debugTypeValue = new int[] { 0 };
        if (DebugEnabled)
        {
            debugBuffer.Dispose();
            debugBuffer = new ComputeBuffer(ShaderRegion.PixelCount, 4 * sizeof(float));
            debugValues = new Color[ShaderRegion.PixelCount];
            debugBuffer.SetData(debugValues);
            ComputeShader.SetBuffer(0, "Debug", debugBuffer);
            ComputeShader.SetBuffer(0, "DebugType", debugTypeBuffer);
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


        if (DebugEnabled)
        {
            debugBuffer.GetData(debugValues);
            debugTypeBuffer.GetData(debugTypeValue);
            DebugType debugType = (DebugType) debugTypeValue[0];

            if (debugType != DebugType.None)
            {
                LogUtil.Log(debugValues, ShaderRegion.CalculationSize.y, Name, debugType);

                int sum = 0;
                for (int i = 0; i < debugValues.GetLength(0); i++)
                {
                    sum += (int)debugValues[i].r;
                }
                Debug.Log("Sum is " + sum);
            }
        }
        debugBuffer.Dispose();
        debugTypeBuffer.Dispose();


        foreach (ComputeBuffer c in BuffersToDispose)
        {
            c.Dispose();
        }
    }
}