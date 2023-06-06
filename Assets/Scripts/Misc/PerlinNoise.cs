using System;
using UnityEngine;

public class PerlinNoise
{
    private Vector2Int TargetTextureSize;
    private Vector2 Scale;

    private float OffsetX;
    private float OffsetY;

    // scale: bigger values => larger areas of the perlin noise terrain in the same amount of space => more frequent noise
    public PerlinNoise(Vector2Int targetTextureSize, Vector2 scale)
    {
        TargetTextureSize = targetTextureSize;
        Scale = scale;

        OffsetX = UnityEngine.Random.Range(0f, 1000f);
        OffsetY = UnityEngine.Random.Range(0f, 1000f);
    }

    public float ValueAt(float x, float y)
    {
        int maxSide = Mathf.Max(TargetTextureSize.x, TargetTextureSize.y);

        x = OffsetX + (float)x / maxSide * Scale.x;
        y = OffsetY + (float)y / maxSide * Scale.y;

        return Mathf.Clamp01(Mathf.PerlinNoise(x, y));
    }
}
