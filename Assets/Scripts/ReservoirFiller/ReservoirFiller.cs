using UnityEngine;

public abstract class ReservoirFiller
{
    protected int XY(int x, int y, int width) {
        return y * width + x;
    }

    public abstract void Fill(Paint paint, Color[] colorsTarget, int[] volumesTarget, Vector2Int targetSize);
}