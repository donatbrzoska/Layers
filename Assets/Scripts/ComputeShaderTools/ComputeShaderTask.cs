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

public class CSInt2 : CSAttribute
{
    private Vector2Int Values;

    public CSInt2(string key, Vector2Int values) : base(key)
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

public class CSFloat2 : CSAttribute
{
    private Vector2 Values;

    public CSFloat2(string key, Vector2 values) : base(key)
    {
        Values = values;
    }

    public override void ApplyTo(ComputeShader computeShader)
    {
        computeShader.SetFloats(Key, Values.x, Values.y);
    }
}

public class CSFloat3 : CSAttribute
{
    private Vector3 Values;

    public CSFloat3(string key, Vector3 values) : base(key)
    {
        Values = values;
    }

    public override void ApplyTo(ComputeShader computeShader)
    {
        computeShader.SetFloats(Key, Values.x, Values.y, Values.z);
    }
}

public class CSFloat4 : CSAttribute
{
    private Vector4 Values;

    public CSFloat4(string key, Vector4 values) : base(key)
    {
        Values = values;
    }

    public override void ApplyTo(ComputeShader computeShader)
    {
        computeShader.SetFloats(Key, Values.x, Values.y, Values.z, Values.w);
    }
}

public struct DebugListInfo
{
    public int Size;
    public DebugListType Type;

    public static int SizeInBytes = sizeof(int) + sizeof(int);

    public DebugListInfo(int size, DebugListType type)
    {
        Size = size;
        Type = type;
    }
}

public enum DebugListType
{
    None,
    Float,
    Float2,
    Float3,
    Float4
}

public class ComputeShaderTask
{
    const int DEBUG_LIST_SIZE_PER_THREAD_MAX = 8;

    public string Name;
    private ShaderRegion ShaderRegion;
    public List<CSAttribute> Attributes;
    public List<ComputeBuffer> BuffersToDispose;
    public bool DebugEnabled;

    public ComputeShaderTask(
        string name,
        ShaderRegion shaderRegion,
        List<CSAttribute> attributes,
        List<ComputeBuffer> buffersToDispose,
        bool debugEnabled)
    {
        Name = name;
        ShaderRegion = shaderRegion;
        Attributes = attributes;
        BuffersToDispose = buffersToDispose;
        DebugEnabled = debugEnabled;
    }

    public void Run()
    {
        ComputeShader computeShader = (ComputeShader)Resources.Load(Name);
        Attributes.Add(new CSInt2("CalculationSize", ShaderRegion.CalculationSize));

        //Debug.Log("Processing " + Name);
        foreach (CSAttribute ca in Attributes)
        {
            //Debug.Log("Processing " + ca);
            ca.ApplyTo(computeShader);
        }


        ComputeBuffer debugBuffer = new ComputeBuffer(1, sizeof(float)); // just for the compiler
        Color[] debugValues = new Color[1]; // just for the compiler
        ComputeBuffer debugListInfoBuffer = new ComputeBuffer(1, DebugListInfo.SizeInBytes);
        DebugListInfo[] debugListInfoValue = new DebugListInfo[] { new DebugListInfo(0, DebugListType.None)};
        if (DebugEnabled)
        {
            debugBuffer.Dispose();
            debugBuffer = new ComputeBuffer(ShaderRegion.PixelCount, DEBUG_LIST_SIZE_PER_THREAD_MAX * 4 * sizeof(float));
            debugValues = new Color[DEBUG_LIST_SIZE_PER_THREAD_MAX * ShaderRegion.PixelCount];
            debugBuffer.SetData(debugValues);
            computeShader.SetBuffer(0, "Debug", debugBuffer);
            computeShader.SetBuffer(0, "DebugInfo", debugListInfoBuffer);
        }


        computeShader.Dispatch(0, ShaderRegion.ThreadGroups.x, ShaderRegion.ThreadGroups.y, 1);


        if (DebugEnabled)
        {
            debugBuffer.GetData(debugValues);
            debugListInfoBuffer.GetData(debugListInfoValue);
            int debugListSize = debugListInfoValue[0].Size;
            DebugListType debugElementType = debugListInfoValue[0].Type;

            if (debugListSize > 0)
            {
                LogUtil.Log(
                    debugValues,
                    new Vector3Int(ShaderRegion.CalculationSize.x, ShaderRegion.CalculationSize.y, DEBUG_LIST_SIZE_PER_THREAD_MAX),
                    debugListSize,
                    debugElementType,
                    Name
                );

                //int sum = 0;
                //for (int i = 0; i < debugValues.GetLength(0); i++)
                //{
                //    sum += (int)debugValues[i].r;
                //}
                //Debug.Log("Sum is " + sum);
            }
        }
        debugBuffer.Dispose();
        debugListInfoBuffer.Dispose();


        foreach (ComputeBuffer c in BuffersToDispose)
        {
            c.Dispose();
        }
    }
}