using System;
using NUnit.Framework;
using UnityEngine;

public class TestIntelGPUShaderRegion
{
    [Test]
    public void Point()
    {
        Vector3 a = new Vector3(0, 0, 0);
        Vector3 b = a;
        Vector3 c = a;
        Vector3 d = a;
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(1, 1, 3, new Vector3(0, 0, 0));
        IntelGPUShaderRegion sr = new IntelGPUShaderRegion(wsc, a, b, c, d);

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
            1,
            sr.CalculationPositionX
        );

        Assert.AreEqual(
            1,
            sr.CalculationPositionY
        );
    }

    [Test]
    public void LineHorizontal_SmallerThanGroupSize()
    {
        Vector3 a = new Vector3(0, 0, 0);
        Vector3 b = a;
        Vector3 c = new Vector3(1, 0, 0);
        Vector3 d = c;

        WorldSpaceCanvas wsc = new WorldSpaceCanvas(1, 1, 3, new Vector3(0, 0, 0));
        IntelGPUShaderRegion sr = new IntelGPUShaderRegion(wsc, a, b, c, d);

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
        Vector3 a = new Vector3(0, 0, 0);
        Vector3 b = a;
        Vector3 c = new Vector3(9, 0, 0);
        Vector3 d = c;

        WorldSpaceCanvas wsc = new WorldSpaceCanvas(19, 19, 1, new Vector3(0, 0, 0));
        IntelGPUShaderRegion sr = new IntelGPUShaderRegion(wsc, a, b, c, d);

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
        Vector3 a = new Vector3(0, 1, 0);
        Vector3 b = a;
        Vector3 c = new Vector3(0, 0, 0);
        Vector3 d = c;

        WorldSpaceCanvas wsc = new WorldSpaceCanvas(19, 19, 1, new Vector3(0, 0, 0));
        IntelGPUShaderRegion sr = new IntelGPUShaderRegion(wsc, a, b, c, d);

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
        Vector3 a = new Vector3(0, 8, 0);
        Vector3 b = a;
        Vector3 c = new Vector3(0, 0, 0);
        Vector3 d = c;

        WorldSpaceCanvas wsc = new WorldSpaceCanvas(19, 19, 1, new Vector3(0, 0, 0));
        IntelGPUShaderRegion sr = new IntelGPUShaderRegion(wsc, a, b, c, d);

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
        Vector3 a = new Vector3(0, 16, 0);
        Vector3 b = new Vector3(8, 16, 0);
        Vector3 c = new Vector3(0, 0, 0);
        Vector3 d = new Vector3(8, 0, 0);

        WorldSpaceCanvas wsc = new WorldSpaceCanvas(19, 19, 1, new Vector3(9, 9, 0));
        IntelGPUShaderRegion sr = new IntelGPUShaderRegion(wsc, a, b, c, d);

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
