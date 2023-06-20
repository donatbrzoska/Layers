using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestVolume_emit
{
    private const int KERNEL_ID_emit_volume = 2;

    private const float EmitDistance_MAX = 0.1f;

    List<CSAttribute> Attributes;

    [SetUp]
    public void Setup()
    {
        Attributes = new() { };
        Attributes.Add(new CSFloat("EmitDistance_MAX", EmitDistance_MAX));
        Attributes.Add(new CSFloat("Tilt_MAX", Rakel.MAX_SUPPORTED_TILT));
        Attributes.Add(new CSFloat("EmitVolume_MIN", 0));
        Attributes.Add(new CSFloat("EmitVolume_MAX", 1));
    }

    private ComputeShaderTask Execute(int kernelID)
    {
        ComputeShaderTask cst = new ComputeShaderTask(
            "Tests/TestVolume_emit",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            Attributes,
            true,
            kernelID);

        cst.Run();

        return cst;
    }

    [Test]
    public void emit_volume_MaxEmit()
    {
        // Arrange
        Attributes.Add(new CSFloat("Distance", 0.5f * EmitDistance_MAX));
        Attributes.Add(new CSFloat("Tilt", 0));


        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_emit_volume);


        // Assert
        Color c = cst.DebugValues[0];
        float result = c.r;

        Assert.AreEqual(1, result);
    }

    [Test]
    public void emit_volume_DistancePoint01_Tilt60()
    {
        // Arrange
        Attributes.Add(new CSFloat("Distance", 0.01f));
        Attributes.Add(new CSFloat("Tilt", 60));


        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_emit_volume);


        // Assert
        Color c = cst.DebugValues[0];
        float result = c.r;

        AssertUtil.AssertFloatsEqual(0.01725f, result);
    }
}
