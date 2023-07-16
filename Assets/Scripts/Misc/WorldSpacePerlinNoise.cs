using UnityEngine;

public class WorldSpacePerlinNoise
{
    private Vector2 Frequency;

    private float OffsetX;
    private float OffsetY;

    public WorldSpacePerlinNoise(Vector2 frequency)
    {
        Frequency = frequency;

        OffsetX = Random.Range(0f, 1000f);
        OffsetY = Random.Range(0f, 1000f);
    }

    public float ValueAt(float x, float y)
    {
        x = OffsetX + (float)x * Frequency.x;
        y = OffsetY + (float)y * Frequency.y;

        return Mathf.Clamp01(Mathf.PerlinNoise(x, y));
    }
}