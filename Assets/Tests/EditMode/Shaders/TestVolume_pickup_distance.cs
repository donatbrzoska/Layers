using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestVolume_pickup_distance
{
    private const int KERNEL_ID_pickup_volume_distance = 0;

    private const float PickupDistance_MAX = 0.1f;

    List<CSAttribute> Attributes;

    [SetUp]
    public void Setup()
    {
        Attributes = new() { };
        Attributes.Add(new CSFloat("PickupDistance_MAX", PickupDistance_MAX));
        Attributes.Add(new CSFloat("PickupVolume_MIN", 0));
        Attributes.Add(new CSFloat("PickupVolume_MAX", 1));
    }

    private ComputeShaderTask Execute(int kernelID)
    {
        ComputeShaderTask cst = new ComputeShaderTask(
            "Tests/TestVolume_pickup",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            Attributes,
            true,
            kernelID);

        cst.Run();

        return cst;
    }

    [Test]
    public void pickup_volume_distance_Lower_OOB()
    {
        // Arrange
        Attributes.Add(new CSFloat("Distance", -1));


        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_pickup_volume_distance);


        // Assert
        Color c = cst.DebugValues[0];
        float result = c.r;

        Assert.AreEqual(1, result);
    }

    [Test]
    public void pickup_volume_distance_Lower()
    {
        // Arrange
        Attributes.Add(new CSFloat("Distance", 0));


        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_pickup_volume_distance);


        // Assert
        Color c = cst.DebugValues[0];
        float result = c.r;

        Assert.AreEqual(1, result);
    }

    [Test]
    public void pickup_volume_distance_Border()
    {
        // Arrange
        Attributes.Add(new CSFloat("Distance", PickupDistance_MAX));


        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_pickup_volume_distance);


        // Assert
        Color c = cst.DebugValues[0];
        float result = c.r;

        Assert.AreEqual(0, result);
    }

    [Test]
    public void pickup_volume_distance_Upper()
    {
        // Arrange
        Attributes.Add(new CSFloat("Distance", 1));


        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_pickup_volume_distance);


        // Assert
        Color c = cst.DebugValues[0];
        float result = c.r;

        Assert.AreEqual(0, result);
    }

    [Test]
    public void pickup_volume_distance_Upper_OOB()
    {
        // Arrange
        Attributes.Add(new CSFloat("Distance", 1 + 0.1f));


        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_pickup_volume_distance);


        // Assert
        Color c = cst.DebugValues[0];
        float result = c.r;

        Assert.AreEqual(0, result);
    }
}
