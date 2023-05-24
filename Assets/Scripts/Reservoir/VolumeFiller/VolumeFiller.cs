using UnityEngine;

public abstract class VolumeFiller
{
    protected float BaseVolume;

    protected VolumeFiller(float baseVolume)
    {
        BaseVolume = baseVolume * Paint.UNIT;
    }

    public abstract void Fill(Paint[] target, Vector2Int targetSize);
}
