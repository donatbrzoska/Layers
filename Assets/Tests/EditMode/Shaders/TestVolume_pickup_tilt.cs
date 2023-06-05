﻿using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestVolume_pickup_tilt
{
    private const int KERNEL_ID_pickup_volume_tilt = 1;

    List<CSAttribute> Attributes;

    [SetUp]
    public void Setup()
    {
        Attributes = new() { };
        Attributes.Add(new CSFloat("Tilt_MAX", Rakel.MAX_SUPPORTED_TILT));
        Attributes.Add(new CSFloat("PickupVolume_MIN", 0));
        Attributes.Add(new CSFloat("PickupVolume_MAX", 1));
    }

    private ComputeShaderTask Execute(int kernelID)
    {
        ComputeShaderTask cst = new ComputeShaderTask(
            "TestVolume_pickup",
            new ShaderCalculation(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            Attributes,
            true,
            kernelID);

        cst.Run();

        return cst;
    }

    [Test]
    public void pickup_volume_tilt_Lower_OOB()
    {
        // Arrange
        Attributes.Add(new CSFloat("Tilt", -1f));


        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_pickup_volume_tilt);


        // Assert
        Color c = cst.DebugValues[0];
        float result = c.r;

        Assert.IsTrue(result > 0);
    }

    [Test]
    public void pickup_volume_tilt_Lower()
    {
        // Arrange
        Attributes.Add(new CSFloat("Tilt", 0));


        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_pickup_volume_tilt);


        // Assert
        Color c = cst.DebugValues[0];
        float result = c.r;

        AssertUtil.AssertFloatsEqual(0.0497870743f, result);
    }

    [Test]
    public void pickup_volume_tilt_Upper()
    {
        // Arrange
        Attributes.Add(new CSFloat("Tilt", Rakel.MAX_SUPPORTED_TILT));


        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_pickup_volume_tilt);


        // Assert
        Color c = cst.DebugValues[0];
        float result = c.r;

        AssertUtil.AssertFloatsEqual(1, result);
    }


    [Test]
    public void pickup_volume_tilt_Upper_OOB()
    {
        // Arrange
        Attributes.Add(new CSFloat("Tilt", Rakel.MAX_SUPPORTED_TILT + 1));


        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_pickup_volume_tilt);


        // Assert
        Color c = cst.DebugValues[0];
        float result = c.r;

        Assert.AreEqual(1, result);
    }
}
