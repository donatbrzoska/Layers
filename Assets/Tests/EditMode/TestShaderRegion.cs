using NUnit.Framework;
using UnityEngine;

public class TestShaderRegion
{
    [Test]
    public void Point()
    {
        Vector2Int a = new Vector2Int(0, 0);
        Vector2Int b = a;
        Vector2Int c = a;
        Vector2Int d = a;
        Vector2Int groupSize = new Vector2Int(8, 1);

        ShaderRegion sr = new ShaderRegion(a, b, c, d, groupSize);

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
        Vector2Int groupSize = new Vector2Int(8, 1);

        ShaderRegion sr = new ShaderRegion(a, b, c, d, groupSize, 1);

        Assert.AreEqual(
            1,
            sr.ThreadGroups.x
        );

        Assert.AreEqual(
            3,
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
    public void Rectangle_SmallerThanGroupSize()
    {
        Vector2Int a = new Vector2Int(0, 2);
        Vector2Int b = new Vector2Int(6, 2);
        Vector2Int c = new Vector2Int(0, 0);
        Vector2Int d = new Vector2Int(6, 0);
        Vector2Int groupSize = new Vector2Int(8, 4);

        ShaderRegion sr = new ShaderRegion(a, b, c, d, groupSize);

        Assert.AreEqual(
            1,
            sr.ThreadGroups.x
        );

        Assert.AreEqual(
            1,
            sr.ThreadGroups.y
        );

        Assert.AreEqual(
            7,
            sr.CalculationSize.x
        );

        Assert.AreEqual(
            3,
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
    public void Rectangle_ExactlyGroupSize()
    {
        Vector2Int a = new Vector2Int(0, 3);
        Vector2Int b = new Vector2Int(7, 3);
        Vector2Int c = new Vector2Int(0, 0);
        Vector2Int d = new Vector2Int(7, 0);
        Vector2Int groupSize = new Vector2Int(8, 4);

        ShaderRegion sr = new ShaderRegion(a, b, c, d, groupSize);

        Assert.AreEqual(
            1,
            sr.ThreadGroups.x
        );

        Assert.AreEqual(
            1,
            sr.ThreadGroups.y
        );

        Assert.AreEqual(
            8,
            sr.CalculationSize.x
        );

        Assert.AreEqual(
            4,
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
    public void Rectangle_BiggerThanGroupSize()
    {
        Vector2Int a = new Vector2Int(0, 4);
        Vector2Int b = new Vector2Int(16, 4);
        Vector2Int c = new Vector2Int(0, 0);
        Vector2Int d = new Vector2Int(16, 0);
        Vector2Int groupSize = new Vector2Int(8, 4);

        ShaderRegion sr = new ShaderRegion(a, b, c, d, groupSize);

        Assert.AreEqual(
            3,
            sr.ThreadGroups.x
        );

        Assert.AreEqual(
            2,
            sr.ThreadGroups.y
        );

        Assert.AreEqual(
            17,
            sr.CalculationSize.x
        );

        Assert.AreEqual(
            5,
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
