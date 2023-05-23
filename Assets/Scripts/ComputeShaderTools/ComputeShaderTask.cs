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
    public static Vector2Int ThreadGroupSize = new Vector2Int(32, 1);

    private const int DEBUG_LIST_SIZE_PER_THREAD_MAX = 8;

    public string Name;
    private ShaderCalculation ShaderCalculation;
    public List<CSAttribute> Attributes;
    public List<ComputeBuffer> BuffersToDispose;
    public bool DebugEnabled;

    public ComputeShaderTask(
        string name,
        ShaderCalculation shaderCalculation,
        List<CSAttribute> attributes,
        List<ComputeBuffer> buffersToDispose,
        bool debugEnabled)
    {
        Name = name;
        ShaderCalculation = shaderCalculation;
        Attributes = attributes;
        BuffersToDispose = buffersToDispose;
        DebugEnabled = debugEnabled;
    }

    public void Run()
    {
        ComputeShader computeShader = (ComputeShader)Resources.Load(Name);
        Attributes.Add(new CSInt2("CalculationPosition", ShaderCalculation.Position));
        Attributes.Add(new CSInt2("CalculationSize", ShaderCalculation.Size));


        ComputeBuffer debugBuffer = new ComputeBuffer(1, sizeof(float)); // just for the compiler
        Color[] debugValues = new Color[1]; // just for the compiler
        ComputeBuffer debugListInfoBuffer = new ComputeBuffer(1, DebugListInfo.SizeInBytes);
        DebugListInfo[] debugListInfoValue = new DebugListInfo[] { new DebugListInfo(0, DebugListType.None)};
        if (DebugEnabled)
        {
            debugBuffer.Dispose();
            debugBuffer = new ComputeBuffer(ShaderCalculation.PixelCount, DEBUG_LIST_SIZE_PER_THREAD_MAX * 4 * sizeof(float));
            debugValues = new Color[DEBUG_LIST_SIZE_PER_THREAD_MAX * ShaderCalculation.PixelCount];
            debugBuffer.SetData(debugValues);
            Attributes.Add(new CSComputeBuffer("Debug", debugBuffer));
            Attributes.Add(new CSComputeBuffer("DebugInfo", debugListInfoBuffer));
        }


        //Debug.Log("Processing " + Name);
        foreach (CSAttribute ca in Attributes)
        {
            //Debug.Log("Processing " + ca);
            ca.ApplyTo(computeShader);
        }

        Vector2Int threadGroups = CalculateThreadGroups(ShaderCalculation.Size, ThreadGroupSize);
        computeShader.Dispatch(0, threadGroups.x, threadGroups.y, 1);


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
                    new Vector3Int(ShaderCalculation.Size.x, ShaderCalculation.Size.y, DEBUG_LIST_SIZE_PER_THREAD_MAX),
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

    Vector2Int CalculateThreadGroups(Vector2Int RegionSize, Vector2Int ThreadGroupSize)
    {
        return new Vector2Int(
            Mathf.CeilToInt((float)RegionSize.x / ThreadGroupSize.x),
            Mathf.CeilToInt((float)RegionSize.y / ThreadGroupSize.y));
    }
}