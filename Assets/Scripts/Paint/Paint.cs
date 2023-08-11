using System;
using UnityEngine;

public struct Paint : IEquatable<Paint>
{
    public static float UNIT = 1;

    public static float VOLUME_THICKNESS = 0.01f; // 1 volume = 1mm = 0.01dm = 0.01 unity worldspace

    public static Paint EMPTY_PAINT { get; private set; } = new Paint(Colors.NO_PAINT_COLOR, 0);

    public static int SizeInBytes = 3 * sizeof(float) + sizeof(float);

    public static Paint operator +(Paint a, Paint b)
    {
        if (a.IsEmpty() && b.IsEmpty())
        {
            return EMPTY_PAINT;
        }

        float summed_volume = a.Volume + b.Volume;
        float a_part = a.Volume / summed_volume;
        float b_part = b.Volume / summed_volume;

        return new Paint(
            new Vector3(
                a.Color.x * a_part + b.Color.x * b_part,
                a.Color.y * a_part + b.Color.y * b_part,
                a.Color.z * a_part + b.Color.z * b_part
            ),
            summed_volume);
    }

    public Vector3 Color { get; set; }
    public float Volume { get; set; }

    public Paint(Vector3 color, float volume)
    {
        Color = color;
        Volume = volume;
    }

    public Paint(Paint paint)
    {
        Color = paint.Color;
        Volume = paint.Volume;
    }

    public bool IsEmpty()
    {
        return Volume == 0;
    }

    public bool Equals(Paint other)
    {
        return ColorsEqual(Color, other.Color) && FloatsEqual(Volume, other.Volume);
    }

    public override string ToString()
    {
        return string.Format("Paint(r={0}, g={1}, b={2}, vol={3})", Color.x, Color.y, Color.z, Volume);
    }

    private static bool ColorsEqual(Vector3 expected, Vector3 actual)
    {
        bool equal = FloatsEqual(expected.x, actual.x)
            && FloatsEqual(expected.y, actual.y)
            && FloatsEqual(expected.z, actual.z);
        return equal;
    }

    private static bool FloatsEqual(float a, float b, float precision = 0.001f)
    {
        return Mathf.Abs(a - b) < precision;
    }
}
