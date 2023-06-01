using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestDistance
{
    private const int KERNEL_ID_distance_point_plane = 0;

    List<CSAttribute> Attributes;

    [SetUp]
    public void Setup()
    {
        Attributes = new List<CSAttribute>();
    }

    private ComputeShaderTask Execute(int kernelID)
    {
        ComputeShaderTask cst = new ComputeShaderTask(
            "TestDistance",
            new ShaderCalculation(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            Attributes,
            true,
            kernelID);

        cst.Run();

        return cst;
    }

    [Test]
    public void distance_point_plane_ZeroDistance()
    {
        // Arrange
        Attributes.Add(new CSFloat3("PointPosition", new Vector3(0, 0, -0.1f)));
        Attributes.Add(new CSFloat3("PlaneSuppVec", new Vector3(0, 0, -0.1f)));
        Attributes.Add(new CSFloat3("PlaneNormal", new Vector3(0, 0, -1)));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_distance_point_plane);


        // Assert
        Vector4 e = cst.DebugValues[0];
        float result = e.x;

        Assert.AreEqual(0, result);
    }

    [Test]
    public void distance_point_plane_full()
    {
        // Arrange
        Attributes.Add(new CSFloat3("PointPosition", new Vector3(-1, -1, -1)));
        Attributes.Add(new CSFloat3("PlaneSuppVec", new Vector3(2, 2, 2)));
        Attributes.Add(new CSFloat3("PlaneNormal", new Vector3(1, 1, 1)));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_distance_point_plane);


        // Assert
        Vector4 e = cst.DebugValues[0];
        float result = e.x;

        Assert.AreEqual(new Vector3(3, 3, 3).magnitude, result);
    }

    [Test]
    public void distance_point_plane_xy_NoNegativeDistance_1()
    {
        // Arrange
        Attributes.Add(new CSFloat3("PointPosition", new Vector3(0, 0, -0.1f)));
        Attributes.Add(new CSFloat3("PlaneSuppVec", new Vector3(0, 0, 0)));
        Attributes.Add(new CSFloat3("PlaneNormal", new Vector3(0, 0, -1)));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_distance_point_plane);

        // Assert
        Vector4 e = cst.DebugValues[0];
        float result = e.x;

        AssertUtil.AssertFloatsEqual(0.1f, result);
    }

    [Test]
    public void distance_point_plane_xy_NoNegativeDistance_2()
    {
        // Arrange
        Attributes.Add(new CSFloat3("PointPosition", new Vector3(0, 0, 0.1f)));
        Attributes.Add(new CSFloat3("PlaneSuppVec", new Vector3(0, 0, 0)));
        Attributes.Add(new CSFloat3("PlaneNormal", new Vector3(0, 0, -1)));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_distance_point_plane);

        // Assert
        Vector4 e = cst.DebugValues[0];
        float result = e.x;

        AssertUtil.AssertFloatsEqual(0.1f, result);
    }

    [Test]
    public void distance_point_plane_xz()
    {
        // Arrange
        Attributes.Add(new CSFloat3("PointPosition", new Vector3(-0.2f, 0, 0)));
        Attributes.Add(new CSFloat3("PlaneSuppVec", new Vector3(0, 0, 0)));
        Attributes.Add(new CSFloat3("PlaneNormal", new Vector3(1, 0, 0)));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_distance_point_plane);

        // Assert
        Vector4 e = cst.DebugValues[0];
        float result = e.x;

        AssertUtil.AssertFloatsEqual(0.2f, result);
    }
}