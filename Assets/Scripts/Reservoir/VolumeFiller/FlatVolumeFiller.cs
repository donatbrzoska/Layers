﻿using UnityEngine;

public class FlatVolumeFiller : VolumeFiller
{
    public FlatVolumeFiller(float baseVolume) : base(baseVolume) { }

    public override void Fill(ColumnInfo[] targetInfo, Paint[] target, Vector3Int targetSize, float targetCellVolume)
    {
        for (int i = 0; i < targetSize.y; i++)
        {
            for (int j = 0; j < targetSize.x; j++)
            {
                SetVolume(targetInfo, target, targetSize, targetCellVolume, j, i, BaseVolume);
            }
        }
    }
}
