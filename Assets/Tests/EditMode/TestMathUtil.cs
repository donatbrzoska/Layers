using System;
using NUnit.Framework;
using UnityEngine;

public class TestMathUtil
{
    [Test]
    public void RotateAroundOrigin_90()
    {
        AssertUtil.Vector2sEqual(
            Vector2.down,
            MathUtil.RotateAroundOrigin(Vector2.right, 90)
        );
    }
    [Test]
    public void RotateAroundOrigin_minus90()
    {
        AssertUtil.Vector2sEqual(
            Vector2.up,
            MathUtil.RotateAroundOrigin(Vector2.right, -90)
        );
    }

    [Test]
    public void Angle360_0()
    {
        Vector2 from = Vector2.right;
        Vector2 to = Vector2.right;

        Assert.AreEqual(0, MathUtil.Angle360(from, to));
    }

    [Test]
    public void Angle360_90()
    {
        Vector2 from = Vector2.right;
        Vector2 to = Vector2.down;

        Assert.AreEqual(90, MathUtil.Angle360(from, to));
    }

    [Test]
    public void Angle360_270()
    {
        Vector2 from = Vector2.right;
        Vector2 to = Vector2.up;

        Assert.AreEqual(270, MathUtil.Angle360(from, to));
    }

    [Test]
    public void RoundToInt_Positive_Lower()
    {
        int rounded = MathUtil.RoundToInt(0.3f);

        Assert.AreEqual(rounded, 0);
    }

    [Test]
    public void RoundToInt_Positive_Edge()
    {
        int rounded = MathUtil.RoundToInt(0.5f);

        Assert.AreEqual(rounded, 1);
    }

    [Test]
    public void RoundToInt_Positive_Upper()
    {
        int rounded = MathUtil.RoundToInt(1.8f);

        Assert.AreEqual(rounded, 2);
    }

    [Test]
    public void RoundToInt_Negative_Lower()
    {
        int rounded = MathUtil.RoundToInt(-2.3f);

        Assert.AreEqual(rounded, -2);
    }

    [Test]
    public void RoundToInt_Negative_Edge()
    {
        int rounded = MathUtil.RoundToInt(-4.5f);

        Assert.AreEqual(rounded, -4);
    }

    [Test]
    public void RoundToInt_Negative_Upper()
    {
        int rounded = MathUtil.RoundToInt(-1.8f);

        Assert.AreEqual(rounded, -2);
    }

    //[Test]
    //public void QuaternionRotationDirection()
    //{
    //    Quaternion q = Quaternion.AngleAxis(90, Vector3.up);

    //    Vector3 rotated = q * new Vector3(1, 0, 0);

    //    Assert.AreEqual(
    //        new Vector3(0, 0, -1), // -> Left hand rule IF z positive goes into the screen
    //        rotated
    //    );
    //}
}
