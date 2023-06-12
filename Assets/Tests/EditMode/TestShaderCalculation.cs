using NUnit.Framework;
using UnityEngine;

public class TestShaderCalculation
{
    [Test]
    public void Point()
    {
        Vector2Int a = new Vector2Int(0, 0);
        Vector2Int b = a;
        Vector2Int c = a;
        Vector2Int d = a;

        ShaderRegion sr = new ShaderRegion(a, b, c, d);

        Assert.AreEqual(
            new Vector2Int(1, 1),
            sr.Size
        );

        Assert.AreEqual(
            new Vector2Int(0, 0),
            sr.Position
        );
    }

    [Test]
    public void PointPadded()
    {
        Vector2Int a = new Vector2Int(0, 0);
        Vector2Int b = a;
        Vector2Int c = a;
        Vector2Int d = a;

        ShaderRegion sr = new ShaderRegion(a, b, c, d, 1);

        Assert.AreEqual(
            new Vector2Int(3, 3),
            sr.Size
        );

        Assert.AreEqual(
            new Vector2Int(-1, -1),
            sr.Position
        );
    }

    [Test]
    public void Rectangle_SmallerThanGroupSize()
    {
        Vector2Int a = new Vector2Int(0, 2);
        Vector2Int b = new Vector2Int(6, 2);
        Vector2Int c = new Vector2Int(0, 0);
        Vector2Int d = new Vector2Int(6, 0);

        ShaderRegion sr = new ShaderRegion(a, b, c, d);

        Assert.AreEqual(
            new Vector2Int(7, 3),
            sr.Size
        );

        Assert.AreEqual(
            new Vector2Int(0, 0),
            sr.Position
        );
    }

    [Test]
    public void Rectangle_ExactlyGroupSize()
    {
        Vector2Int a = new Vector2Int(0, 3);
        Vector2Int b = new Vector2Int(7, 3);
        Vector2Int c = new Vector2Int(0, 0);
        Vector2Int d = new Vector2Int(7, 0);

        ShaderRegion sr = new ShaderRegion(a, b, c, d);

        Assert.AreEqual(
            new Vector2Int(8, 4),
            sr.Size
        );

        Assert.AreEqual(
            new Vector2Int(0, 0),
            sr.Position
        );
    }

    [Test]
    public void Rectangle_BiggerThanGroupSize()
    {
        Vector2Int a = new Vector2Int(0, 4);
        Vector2Int b = new Vector2Int(16, 4);
        Vector2Int c = new Vector2Int(0, 0);
        Vector2Int d = new Vector2Int(16, 0);

        ShaderRegion sr = new ShaderRegion(a, b, c, d);

        Assert.AreEqual(
            new Vector2Int(17, 5),
            sr.Size
        );

        Assert.AreEqual(
            new Vector2Int(0, 0),
            sr.Position
        );
    }

    [Test]
    public void ReduceShaderRegion()
    {
        Vector2Int reduceRegionPosition = new Vector2Int(1, 2);
        Vector2Int reduceRegionSize = new Vector2Int(9, 6);
        Vector2Int reduceBlockSize = new Vector2Int(2, 2);

        ShaderRegion rsr = new ShaderRegion(reduceRegionPosition, reduceRegionSize, reduceBlockSize);

        Assert.AreEqual(
            new Vector2Int(5, 3),
            rsr.Size
        );

        Assert.AreEqual(
            reduceRegionPosition,
            rsr.Position
        );
    }
}
