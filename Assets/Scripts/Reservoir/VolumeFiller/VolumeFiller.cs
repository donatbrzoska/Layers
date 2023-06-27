﻿using UnityEngine;

public abstract class VolumeFiller
{
    protected float BaseVolume;

    protected VolumeFiller(float baseVolume)
    {
        BaseVolume = baseVolume * Paint.UNIT;
    }

    protected void SetVolume(ColumnInfo[] targetInfo, Paint[] target, Vector3Int targetSize, int x, int y, float volume)
    {
        float left = volume;
        int z = 0;

        while (left > 0 && z < targetSize.z)
        {
            float cellVolume = Mathf.Min(Paint.UNIT, left);
            target[IndexUtil.XYZ(x, y, z, targetSize.x, targetSize.y)].Volume = cellVolume;

            left -= cellVolume;
            z++;
        }
        targetInfo[IndexUtil.XY(x, y, targetSize.x)].Size = z;
        targetInfo[IndexUtil.XY(x, y, targetSize.x)].WriteIndex = z;
        targetInfo[IndexUtil.XY(x, y, targetSize.x)].Volume = volume - left;
    }

    public abstract void Fill(ColumnInfo[] targetInfo, Paint[] target, Vector3Int targetSize);
}
