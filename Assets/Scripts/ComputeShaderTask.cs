using UnityEngine;
using System.Collections.Generic;

public abstract class CSAttribute
{
    protected string Key;

    public CSAttribute(string key)
    {
        Key = key;
    }

    public abstract void Apply(ComputeShader computeShader);
}

public class CSComputeBuffer : CSAttribute
{
    private ComputeBuffer ComputeBuffer;

    public CSComputeBuffer(string key, ComputeBuffer computeBuffer) : base(key)
    {
        ComputeBuffer = computeBuffer;
    }

    public override void Apply(ComputeShader computeShader)
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

    public override void Apply(ComputeShader computeShader)
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

    public override void Apply(ComputeShader computeShader)
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

    public override void Apply(ComputeShader computeShader)
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

    public override void Apply(ComputeShader computeShader)
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

    public override void Apply(ComputeShader computeShader)
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

    public override void Apply(ComputeShader computeShader)
    {
        computeShader.SetFloats(Key, Values.x, Values.y, Values.z);
    }
}

public struct ComputeShaderTask {
    public ComputeShader ComputeShader;
    public List<CSAttribute> Attributes;
    public Vector2Int ThreadGroups;
    public List<ComputeBuffer> BuffersToDispose;

    public ComputeShaderTask(ComputeShader computeShader, List<CSAttribute> attributes, Vector2Int threadGroups, List<ComputeBuffer> buffersToDispose)
    {
        ComputeShader = computeShader;
        Attributes = attributes;
        ThreadGroups = threadGroups;
        BuffersToDispose = buffersToDispose;
    }
}