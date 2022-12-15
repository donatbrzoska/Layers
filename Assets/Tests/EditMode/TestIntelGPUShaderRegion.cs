using NUnit.Framework;
using UnityEngine;

public class TestIntelGPUShaderRegion
{
    [Test]
    public void Point()
    {
        Vector2Int a = new Vector2Int(0, 0);
        Vector2Int b = a;
        Vector2Int c = a;
        Vector2Int d = a;
        IntelGPUShaderRegion sr = new IntelGPUShaderRegion(a, b, c, d);

        Assert.AreEqual(
            1,
            sr.ThreadGroups.x
        );

        Assert.AreEqual(
            1,
            sr.ThreadGroups.y
        );

        Assert.AreEqual(
            1,
            sr.CalculationSize.x
        );

        Assert.AreEqual(
            1,
            sr.CalculationSize.y
        );

        Assert.AreEqual(
            0,
            sr.CalculationPosition.x
        );

        Assert.AreEqual(
            0,
            sr.CalculationPosition.y
        );
    }

    [Test]
    public void PointPadded()
    {
        Vector2Int a = new Vector2Int(0, 0);
        Vector2Int b = a;
        Vector2Int c = a;
        Vector2Int d = a;
        IntelGPUShaderRegion sr = new IntelGPUShaderRegion(a, b, c, d, 1);

        Assert.AreEqual(
            3,
            sr.ThreadGroups.x
        );

        Assert.AreEqual(
            1,
            sr.ThreadGroups.y
        );

        Assert.AreEqual(
            3,
            sr.CalculationSize.x
        );

        Assert.AreEqual(
            3,
            sr.CalculationSize.y
        );

        Assert.AreEqual(
            -1,
            sr.CalculationPosition.x
        );

        Assert.AreEqual(
            -1,
            sr.CalculationPosition.y
        );
    }

    [Test]
    public void LineHorizontal_SmallerThanGroupSize()
    {
        Vector2Int a = new Vector2Int(0, 0);
        Vector2Int b = a;
        Vector2Int c = new Vector2Int(1, 0);
        Vector2Int d = c;

        IntelGPUShaderRegion sr = new IntelGPUShaderRegion(a, b, c, d);

        Assert.AreEqual(
            2,
            sr.ThreadGroups.x
        );

        Assert.AreEqual(
            1,
            sr.ThreadGroups.y
        );
    }

    [Test]
    public void LineHorizontal_BiggerThanGroupSize()
    {
        Vector2Int a = new Vector2Int(0, 0);
        Vector2Int b = a;
        Vector2Int c = new Vector2Int(9, 0);
        Vector2Int d = c;

        IntelGPUShaderRegion sr = new IntelGPUShaderRegion(a, b, c, d);

        Assert.AreEqual(
            10,
            sr.ThreadGroups.x
        );

        Assert.AreEqual(
            1,
            sr.ThreadGroups.y
        );
    }

    [Test]
    public void LineVertical_SmallerThanGroupSize()
    {
        Vector2Int a = new Vector2Int(0, 1);
        Vector2Int b = a;
        Vector2Int c = new Vector2Int(0, 0);
        Vector2Int d = c;

        IntelGPUShaderRegion sr = new IntelGPUShaderRegion(a, b, c, d);

        Assert.AreEqual(
            1,
            sr.ThreadGroups.x
        );

        Assert.AreEqual(
            1,
            sr.ThreadGroups.y
        );
    }

    [Test]
    public void LineVertical_BiggerThanGroupSize()
    {
        Vector2Int a = new Vector2Int(0, 8);
        Vector2Int b = a;
        Vector2Int c = new Vector2Int(0, 0);
        Vector2Int d = c;

        IntelGPUShaderRegion sr = new IntelGPUShaderRegion(a, b, c, d);

        Assert.AreEqual(
            1,
            sr.ThreadGroups.x
        );

        Assert.AreEqual(
            2,
            sr.ThreadGroups.y
        );
    }

    [Test]
    public void Rectangle()
    {
        Vector2Int a = new Vector2Int(0, 16);
        Vector2Int b = new Vector2Int(8, 16);
        Vector2Int c = new Vector2Int(0, 0);
        Vector2Int d = new Vector2Int(8, 0);

        IntelGPUShaderRegion sr = new IntelGPUShaderRegion(a, b, c, d);

        Assert.AreEqual(
            9,
            sr.ThreadGroups.x
        );

        Assert.AreEqual(
            3,
            sr.ThreadGroups.y
        );

        Assert.AreEqual(
            9,
            sr.CalculationSize.x
        );

        Assert.AreEqual(
            17,
            sr.CalculationSize.y
        );

        Assert.AreEqual(
            0,
            sr.CalculationPosition.x
        );

        Assert.AreEqual(
            0,
            sr.CalculationPosition.y
        );
    }
}
