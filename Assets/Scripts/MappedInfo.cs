using UnityEngine;

public struct MappedInfo
{
    public const int SizeInBytes = 1 * 3 * sizeof(float) + 1 * 2 * sizeof(float) + 1 * sizeof(float);

    public Vector3 TransformedPixel;
    public Vector2 ReservoirPixel;
    public float Distance;
}