﻿using NUnit.Framework;
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

        ShaderCalculation sc = new ShaderCalculation(a, b, c, d);

        Assert.AreEqual(
            new Vector2Int(1, 1),
            sc.Size
        );

        Assert.AreEqual(
            new Vector2Int(0, 0),
            sc.Position
        );
    }

    [Test]
    public void PointPadded()
    {
        Vector2Int a = new Vector2Int(0, 0);
        Vector2Int b = a;
        Vector2Int c = a;
        Vector2Int d = a;

        ShaderCalculation sc = new ShaderCalculation(a, b, c, d, 1);

        Assert.AreEqual(
            new Vector2Int(3, 3),
            sc.Size
        );

        Assert.AreEqual(
            new Vector2Int(-1, -1),
            sc.Position
        );
    }

    [Test]
    public void Rectangle_SmallerThanGroupSize()
    {
        Vector2Int a = new Vector2Int(0, 2);
        Vector2Int b = new Vector2Int(6, 2);
        Vector2Int c = new Vector2Int(0, 0);
        Vector2Int d = new Vector2Int(6, 0);

        ShaderCalculation sc = new ShaderCalculation(a, b, c, d);

        Assert.AreEqual(
            new Vector2Int(7, 3),
            sc.Size
        );

        Assert.AreEqual(
            new Vector2Int(0, 0),
            sc.Position
        );
    }

    [Test]
    public void Rectangle_ExactlyGroupSize()
    {
        Vector2Int a = new Vector2Int(0, 3);
        Vector2Int b = new Vector2Int(7, 3);
        Vector2Int c = new Vector2Int(0, 0);
        Vector2Int d = new Vector2Int(7, 0);

        ShaderCalculation sc = new ShaderCalculation(a, b, c, d);

        Assert.AreEqual(
            new Vector2Int(8, 4),
            sc.Size
        );

        Assert.AreEqual(
            new Vector2Int(0, 0),
            sc.Position
        );
    }

    [Test]
    public void Rectangle_BiggerThanGroupSize()
    {
        Vector2Int a = new Vector2Int(0, 4);
        Vector2Int b = new Vector2Int(16, 4);
        Vector2Int c = new Vector2Int(0, 0);
        Vector2Int d = new Vector2Int(16, 0);

        ShaderCalculation sc = new ShaderCalculation(a, b, c, d);

        Assert.AreEqual(
            new Vector2Int(17, 5),
            sc.Size
        );

        Assert.AreEqual(
            new Vector2Int(0, 0),
            sc.Position
        );
    }
}