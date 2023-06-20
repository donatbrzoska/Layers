using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestDistance
{
    private const int KERNEL_ID_distance_from_rakel = 0;
    private const int KERNEL_ID_distance_from_canvas = 1;

    List<CSAttribute> Attributes;

    [SetUp]
    public void Setup()
    {
        Attributes = new List<CSAttribute>();
    }

    private ComputeShaderTask Execute(int kernelID)
    {
        ComputeShaderTask cst = new ComputeShaderTask(
            "Tests/TestDistance",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            Attributes,
            true,
            kernelID);

        cst.Run();

        return cst;
    }

    [Test]
    public void distance_from_rakel_Untilted_ZeroDistance()
    {
        // Arrange
        Attributes.Add(new CSFloat3("PointPos", new Vector3(0, 0, -0.1f)));
        Attributes.Add(new CSFloat3("RakelLLTilted", new Vector3(0, 0, -0.1f)));
        Attributes.Add(new CSFloat3("RakelLRTilted", new Vector3(1, 0, -0.1f)));
        Attributes.Add(new CSFloat3("RakelPosition", new Vector3(1, 1, -0.1f)));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_distance_from_rakel);


        // Assert
        Vector4 e = cst.DebugValues[0];
        float result = e.x;

        Assert.AreEqual(0, result);
    }

    [Test]
    public void distance_from_rakel_Untilted_No_NegativeDistance_1()
    {
        // Arrange
        Attributes.Add(new CSFloat3("PointPos", new Vector3(0, 0, 0.1f)));
        Attributes.Add(new CSFloat3("RakelLLTilted", new Vector3(0, 0, 0)));
        Attributes.Add(new CSFloat3("RakelLRTilted", new Vector3(1, 0, 0)));
        Attributes.Add(new CSFloat3("RakelPosition", new Vector3(1, 1, 0)));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_distance_from_rakel);


        // Assert
        Vector4 e = cst.DebugValues[0];
        float result = e.x;

        Assert.AreEqual(0.1f, result);
    }

    [Test]
    public void distance_from_rakel_Untilted_No_NegativeDistance_2()
    {
        // Arrange
        Attributes.Add(new CSFloat3("PointPos", new Vector3(0, 0, -0.1f)));
        Attributes.Add(new CSFloat3("RakelLLTilted", new Vector3(0, 0, 0)));
        Attributes.Add(new CSFloat3("RakelLRTilted", new Vector3(1, 0, 0)));
        Attributes.Add(new CSFloat3("RakelPosition", new Vector3(1, 1, 0)));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_distance_from_rakel);


        // Assert
        Vector4 e = cst.DebugValues[0];
        float result = e.x;

        Assert.AreEqual(0.1f, result);
    }

    [Test]
    public void distance_from_rakel_Tilted30_ZeroDistance()
    {
        float TILT = 30;
        // PointPos is at 0.25 * dx of tilted Rakel, looking from LL of Rakel
        float POS_RATE = 0.25f;
        // Arrange
        Attributes.Add(new CSFloat3("PointPos", new Vector3(
            1 - Mathf.Cos(Mathf.Deg2Rad * TILT) + POS_RATE * Mathf.Cos(Mathf.Deg2Rad * TILT),
            0,
            POS_RATE * Mathf.Sin(Mathf.Deg2Rad * TILT))));
        Attributes.Add(new CSFloat3("RakelLLTilted", new Vector3(1 - Mathf.Cos(Mathf.Deg2Rad * TILT), 0, 0)));
        Attributes.Add(new CSFloat3("RakelLRTilted", new Vector3(1, 0, Mathf.Sin(Mathf.Deg2Rad * TILT))));
        Attributes.Add(new CSFloat3("RakelPosition", new Vector3(1, 1, Mathf.Sin(Mathf.Deg2Rad * TILT))));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_distance_from_rakel);


        // Assert
        Vector4 e = cst.DebugValues[0];
        float result = e.x;

        AssertUtil.AssertFloatsEqual(0, result);
    }

    [Test]
    public void distance_from_rakel_Tilted30()
    {
        float TILT = 70;
        // PointPos is at 0.66 * dx of tilted Rakel, looking from LL of Rakel
        float POS_RATE = 0.66f;
        // Arrange
        Attributes.Add(new CSFloat3("PointPos", new Vector3(
            1 - Mathf.Cos(Mathf.Deg2Rad * TILT) + POS_RATE * Mathf.Cos(Mathf.Deg2Rad * TILT), 0, 0)));
        Attributes.Add(new CSFloat3("RakelLLTilted", new Vector3(1 - Mathf.Cos(Mathf.Deg2Rad * TILT), 0, 0)));
        Attributes.Add(new CSFloat3("RakelLRTilted", new Vector3(1, 0, Mathf.Sin(Mathf.Deg2Rad * TILT))));
        Attributes.Add(new CSFloat3("RakelPosition", new Vector3(1, 1, Mathf.Sin(Mathf.Deg2Rad * TILT))));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_distance_from_rakel);


        // Assert
        Vector4 e = cst.DebugValues[0];
        float result = e.x;

        AssertUtil.AssertFloatsEqual(
            0.6202f, // POS_RATE * Mathf.Sin(Mathf.Deg2Rad * TILT),
            result);
    }

    [Test]
    public void distance_from_canvas_ZeroDistance()
    {
        // Arrange
        Attributes.Add(new CSFloat3("PointPos", new Vector3(0, 0, -0.1f)));
        Attributes.Add(new CSFloat3("CanvasPosition", new Vector3(0, 0, -0.1f)));
        Attributes.Add(new CSFloat3("CanvasNormal", new Vector3(0, 0, -1)));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_distance_from_canvas);


        // Assert
        Vector4 e = cst.DebugValues[0];
        float result = e.x;

        Assert.AreEqual(0, result);
    }

    [Test]
    public void distance_from_canvas_xy()
    {
        // Arrange
        Attributes.Add(new CSFloat3("PointPos", new Vector3(0, 0, -0.1f)));
        Attributes.Add(new CSFloat3("CanvasPosition", new Vector3(0, 0, 0)));
        Attributes.Add(new CSFloat3("CanvasNormal", new Vector3(0, 0, -1)));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_distance_from_canvas);

        // Assert
        Vector4 e = cst.DebugValues[0];
        float result = e.x;

        AssertUtil.AssertFloatsEqual(0.1f, result);
    }

    [Test]
    public void distance_from_canvas_xz()
    {
        // Arrange
        Attributes.Add(new CSFloat3("PointPos", new Vector3(-0.2f, 0, 0)));
        Attributes.Add(new CSFloat3("CanvasPosition", new Vector3(0, 0, 0)));
        Attributes.Add(new CSFloat3("CanvasNormal", new Vector3(1, 0, 0)));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_distance_from_canvas);

        // Assert
        Vector4 e = cst.DebugValues[0];
        float result = e.x;

        AssertUtil.AssertFloatsEqual(0.2f, result);
    }
}