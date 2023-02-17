using UnityEngine;

public class ShaderRegionFactory
{
    private Vector2Int GroupSize;

    public ShaderRegionFactory(Vector2Int groupSize)
    {
        GroupSize = groupSize;
    }

    public ShaderRegion Create(Vector2Int a, Vector2Int b, Vector2Int c, Vector2Int d, int padding)
    {
        return new ShaderRegion(a, b, c, d, GroupSize, padding);
    }

    public ShaderRegion Create(Vector2Int a, Vector2Int b, Vector2Int c, Vector2Int d)
    {
        return Create(a, b, c, d, 0);
    }
}
