using UnityEngine;

public unsafe struct MappedInfo
{
    public const int SizeInBytes = 1 * 3 * sizeof(float) + 1 * 2 * sizeof(float) + 2 * sizeof(float) + 9 * 9 * sizeof(float);

    public Vector3 TransformedPixel;
    public Vector2 ReservoirPixel;
    public float Distance;
    public fixed float Overlap[9 * 9];
    public float TargetVolumeToTransfer;
}