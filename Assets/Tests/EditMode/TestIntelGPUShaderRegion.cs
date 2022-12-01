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
            sr.ThreadGroupsX
        );

        Assert.AreEqual(
            1,
            sr.ThreadGroupsY
        );

        Assert.AreEqual(
            1,
            sr.CalculationSizeX
        );

        Assert.AreEqual(
            1,
            sr.CalculationSizeY
        );

        Assert.AreEqual(
            0,
            sr.CalculationPositionX
        );

        Assert.AreEqual(
            0,
            sr.CalculationPositionY
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
            sr.ThreadGroupsX
        );

        Assert.AreEqual(
            1,
            sr.ThreadGroupsY
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
            sr.ThreadGroupsX
        );

        Assert.AreEqual(
            1,
            sr.ThreadGroupsY
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
            sr.ThreadGroupsX
        );

        Assert.AreEqual(
            1,
            sr.ThreadGroupsY
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
            sr.ThreadGroupsX
        );

        Assert.AreEqual(
            2,
            sr.ThreadGroupsY
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
            sr.ThreadGroupsX
        );

        Assert.AreEqual(
            3,
            sr.ThreadGroupsY
        );

        Assert.AreEqual(
            9,
            sr.CalculationSizeX
        );

        Assert.AreEqual(
            17,
            sr.CalculationSizeY
        );

        Assert.AreEqual(
            0,
            sr.CalculationPositionX
        );

        Assert.AreEqual(
            0,
            sr.CalculationPositionY
        );
    }
}
