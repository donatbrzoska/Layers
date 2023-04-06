using UnityEngine;

public class ShaderRegionFactory
{
    private Vector2Int ThreadGroupSize;

    public ShaderRegionFactory(Vector2Int threadGroupSize)
    {
        ThreadGroupSize = threadGroupSize;
    }

    public ShaderRegion Create(Vector2Int a, Vector2Int b, Vector2Int c, Vector2Int d, int padding)
    {
        return new ShaderRegion(a, b, c, d, ThreadGroupSize, padding);
    }

    public ShaderRegion Create(Vector2Int a, Vector2Int b, Vector2Int c, Vector2Int d)
    {
        return Create(a, b, c, d, 0);
    }
}
