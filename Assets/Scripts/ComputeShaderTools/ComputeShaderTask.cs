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

    public abstract void ApplyTo(ComputeShader computeShader, int kernelID);
}

public class CSComputeBuffer : CSAttribute
{
    private ComputeBuffer ComputeBuffer;

    public CSComputeBuffer(string key, ComputeBuffer computeBuffer) : base(key)
    {
        ComputeBuffer = computeBuffer;
    }

    public override void ApplyTo(ComputeShader computeShader, int kernelID)
    {
        computeShader.SetBuffer(kernelID, Key, ComputeBuffer);
    }
}

public class CSTexture : CSAttribute
{
    private RenderTexture RenderTexture;

    public CSTexture(string key, RenderTexture renderTexture) : base(key)
    {
        RenderTexture = renderTexture;
    }

    public override void ApplyTo(ComputeShader computeShader, int kernelID)
    {
        computeShader.SetTexture(kernelID, Key, RenderTexture);
    }
}

public class CSInt : CSAttribute
{
    private int Value;

    public CSInt(string key, int value) : base(key)
    {
        Value = value;
    }

    public override void ApplyTo(ComputeShader computeShader, int kernelID)
    {
        computeShader.SetInt(Key, Value);
    }
}

public class CSInt3 : CSAttribute
{
    private Vector3Int Values;

    public CSInt3(string key, Vector3Int values) : base(key)
    {
        Values = values;
    }

    public override void ApplyTo(ComputeShader computeShader, int kernelID)
    {
        computeShader.SetInts(Key, Values.x, Values.y, Values.z);
    }
}

public class CSInt2 : CSAttribute
{
    private Vector2Int Values;

    public CSInt2(string key, Vector2Int values) : base(key)
    {
        Values = values;
    }

    public override void ApplyTo(ComputeShader computeShader, int kernelID)
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

    public override void ApplyTo(ComputeShader computeShader, int kernelID)
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

    public override void ApplyTo(ComputeShader computeShader, int kernelID)
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

    public override void ApplyTo(ComputeShader computeShader, int kernelID)
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

    public override void ApplyTo(ComputeShader computeShader, int kernelID)
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

    private const int DEBUG_LIST_SIZE_PER_THREAD_MAX = 16;

    public string Name;
    private ShaderRegion ShaderRegion;
    Vector2Int SubgridGroupSize;
    public List<CSAttribute> Attributes;
    public bool DebugEnabled;
    public int KernelID;

    public Vector4[] DebugValues;

    public ComputeShaderTask(
        string name,
        ShaderRegion shaderRegion,
        Vector2Int subgridGroupSize,
        List<CSAttribute> attributes,
        bool debugEnabled,
        int kernelID = 0)
    {
        Name = name;
        ShaderRegion = shaderRegion;
        SubgridGroupSize = subgridGroupSize;

        Attributes = attributes;
        DebugEnabled = debugEnabled;
        KernelID = kernelID;
    }

    // disable subgrid logic constructor
    public ComputeShaderTask(string name, ShaderRegion shaderRegion, List<CSAttribute> attributes, bool debugEnabled, int kernelID = 0) : this(name, shaderRegion, new Vector2Int(1, 1), attributes, debugEnabled, kernelID) { }

    public void Run()
    {
        ComputeShader computeShader = (ComputeShader)Resources.Load(Name);
        Attributes.Add(new CSInt2("CalculationPosition", ShaderRegion.Position));
        Attributes.Add(new CSInt2("CalculationSize", ShaderRegion.Size));


        ComputeBuffer debugBuffer = new ComputeBuffer(1, sizeof(float)); // just for the compiler
        DebugValues = new Vector4[1]; // just for the compiler
        ComputeBuffer debugListInfoBuffer = new ComputeBuffer(1, DebugListInfo.SizeInBytes);
        DebugListInfo[] debugListInfoValue = new DebugListInfo[] { new DebugListInfo(0, DebugListType.None)};
        if (DebugEnabled)
        {
            debugBuffer.Dispose();
            debugBuffer = new ComputeBuffer(ShaderRegion.PixelCount, DEBUG_LIST_SIZE_PER_THREAD_MAX * 4 * sizeof(float));
            DebugValues = new Vector4[DEBUG_LIST_SIZE_PER_THREAD_MAX * ShaderRegion.PixelCount];
            debugBuffer.SetData(DebugValues);
            Attributes.Add(new CSComputeBuffer("Debug", debugBuffer));
            Attributes.Add(new CSComputeBuffer("DebugInfo", debugListInfoBuffer));
        }


        //Debug.Log("Processing " + Name);
        foreach (CSAttribute ca in Attributes)
        {
            //Debug.Log("Processing " + ca);
            ca.ApplyTo(computeShader, KernelID);
        }


        Vector2Int subgridGroups = CalculateGroups(ShaderRegion.Size, SubgridGroupSize);
        Vector2Int threadGroups = CalculateGroups(subgridGroups, ThreadGroupSize);
        for (int y = 0; y < SubgridGroupSize.y; y++)
        {
            for (int x = 0; x < SubgridGroupSize.x; x++)
            {
                computeShader.SetInts("SubgridCurrentThreadID", x, y);
                computeShader.SetInts("SubgridGroupSize", SubgridGroupSize.x, SubgridGroupSize.y);

                computeShader.Dispatch(KernelID, threadGroups.x, threadGroups.y, 1);
            }
        }


        if (DebugEnabled)
        {
            debugBuffer.GetData(DebugValues);
            debugListInfoBuffer.GetData(debugListInfoValue);
            int debugListSize = debugListInfoValue[0].Size;
            DebugListType debugElementType = debugListInfoValue[0].Type;

            if (debugListSize > 0)
            {
                LogUtil.Log(
                    DebugValues,
                    new Vector3Int(ShaderRegion.Size.x, ShaderRegion.Size.y, DEBUG_LIST_SIZE_PER_THREAD_MAX),
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
    }

    Vector2Int CalculateGroups(Vector2Int RegionSize, Vector2Int GroupSize)
    {
        return new Vector2Int(
            Mathf.CeilToInt((float)RegionSize.x / GroupSize.x),
            Mathf.CeilToInt((float)RegionSize.y / GroupSize.y));
    }
}