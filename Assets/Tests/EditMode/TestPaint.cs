using System;
using NUnit.Framework;
using UnityEngine;

public class TestPaint
{
    [Test]
    public void Equals_VolumeOnly_IsNotEqual()
    {
        Paint p1 = new Paint(new Vector3(0.2f, 0.2f, 0.2f), 1);
        Paint p2 = new Paint(new Vector3(0.2f, 0.2f, 0.4f), 1);

        Assert.AreNotEqual(p1, p2);
    }

    [Test]
    public void Equals_ColorOnly_IsNotEqual()
    {
        Paint p1 = new Paint(new Vector3(0.2f, 0.2f, 0.2f), 1);
        Paint p2 = new Paint(new Vector3(0.2f, 0.2f, 0.2f), 2);

        Assert.AreNotEqual(p1, p2);
    }

    [Test]
    public void Equals()
    {
        Paint p1 = new Paint(new Vector3(0.2f, 0.3f, 0.4f), 1);
        Paint p2 = new Paint(new Vector3(0.2f, 0.3f, 0.4f), 1);

        Assert.AreEqual(p1, p2);
    }

    [Test]
    public void IsEmpty()
    {
        Paint p = new Paint(Colors.NO_PAINT_COLOR, 0);

        Assert.AreEqual(p, Paint.EMPTY_PAINT);
    }

    [Test]
    public void Plus_BothEmpty()
    {
        Paint res = Paint.EMPTY_PAINT + Paint.EMPTY_PAINT;

        Assert.AreEqual(Paint.EMPTY_PAINT, res);
    }

    [Test]
    public void Plus_FirstFilled_SecondEmpty()
    {
        Paint p1 = new Paint(new Vector3(0.1f, 0.2f, 0.3f), 1);
        Paint p2 = Paint.EMPTY_PAINT;

        Paint res = p1 + p2;

        Assert.AreEqual(p1, res);
    }

    [Test]
    public void Plus_FirstEmpty_SecondFilled()
    {
        Paint p1 = Paint.EMPTY_PAINT;
        Paint p2 = new Paint(new Vector3(0.1f, 0.2f, 0.3f), 1);

        Paint res = p1 + p2;

        Assert.AreEqual(p2, res);
    }

    [Test]
    public void Plus_BothFilled()
    {
        Paint p1 = new Paint(new Vector3(0.1f, 0.2f, 0.3f), 1);
        Paint p2 = new Paint(new Vector3(0.4f, 0.5f, 0.6f), 2);

        Paint res = p1 + p2;

        Assert.AreEqual(new Paint(new Vector3(0.3f, 0.4f, 0.5f), 3), res);
    }

    [Test]
    public void ToString_()
    {
        Paint p = new Paint(new Vector3(0.4f, 0.5f, 0.6f), 2);

        Assert.AreEqual("Paint(r=0.4, g=0.5, b=0.6, vol=2)", p.ToString());
    }

    [Test]
    public void CopyConstructor()
    {
        Paint p1 = new Paint(new Vector3(0.1f, 0.2f, 0.3f), 2);

        Paint p2 = new Paint(p1);
        p1.Volume -= 1;

        Assert.AreEqual(2, p2.Volume);
    }
}
