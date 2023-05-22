using UnityEngine;

public abstract class VolumeFiller
{
    protected int BaseVolume;

    protected VolumeFiller(int baseVolume)
    {
        BaseVolume = baseVolume * Paint.UNIT;
    }

    public abstract void Fill(Paint[] target, Vector2Int targetSize);
}
