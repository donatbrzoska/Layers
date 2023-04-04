using UnityEngine;

public abstract class VolumeFiller
{
    protected static int PAINT_UNIT = 100_000;

    protected int BaseVolume;

    protected VolumeFiller(int baseVolume)
    {
        BaseVolume = baseVolume * PAINT_UNIT;
    }

    public abstract void Fill(Paint[] target, Vector2Int targetSize);
}
