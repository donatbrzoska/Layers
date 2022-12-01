using System;
using NUnit.Framework;
using UnityEngine;

public class AssertUtil
{
    public static void AssertColorsAreEqual(Color[,] expected, Color[,] actual)
    {
        Assert.AreEqual(expected.GetLength(0), actual.GetLength(0));
        Assert.AreEqual(expected.GetLength(1), actual.GetLength(1));

        for (int i = 0; i < expected.GetLength(0); i++)
        {
            for (int j = 0; j < expected.GetLength(1); j++)
            {
                if (!ColorsAreEqual(expected[i, j], actual[i, j]))
                {
                    // TODO this is hacky but works, just NOTE that the given error message
                    //      might be misleading and you have to print your arrays for a better view
                    Assert.AreEqual(expected, actual);
                }
            }
        }
    }

    public static void AssertColorsAreEqual(Color[] expected, Color[] actual)
    {
        Assert.AreEqual(expected.GetLength(0), actual.GetLength(0));

        for (int i = 0; i < expected.GetLength(0); i++)
        {
            if (!ColorsAreEqual(expected[i], actual[i]))
            {
                // TODO this is hacky but works, just NOTE that the given error message
                //      might be misleading and you have to print your arrays for a better view
                Assert.AreEqual(expected, actual);
            }
        }
    }

    public static void AssertColorsAreEqual(Color expected, Color actual)
    {
        if (!ColorsAreEqual(expected, actual))
        {
            Assert.AreEqual(expected, actual);
        }
    }

    private static bool ColorsAreEqual(Color expected, Color actual)
    {
        bool equal = FloatsEqual(expected.a, actual.a)
            && FloatsEqual(expected.r, actual.r)
            && FloatsEqual(expected.g, actual.g)
            && FloatsEqual(expected.b, actual.b);
        return equal;
    }

    public static void Vector2sEqual(Vector2 a, Vector2 b)
    {
        Assert.IsTrue(FloatsEqual(a.x, b.x) && FloatsEqual(a.y, b.y));
    }

    private static bool FloatsEqual(float a, float b, float precision = 0.001f)
    {
        return Mathf.Abs(a - b) < precision;
    }

    private AssertUtil() { }
}
