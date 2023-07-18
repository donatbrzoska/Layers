using System;
using UnityEngine;

public class PerlinVolumeFiller : VolumeFiller
{
    public PerlinVolumeFiller(float widthRatio, float baseVolume) : base(widthRatio, baseVolume) { }

    public override void Fill(ColumnInfo[] targetInfo, Paint[] target, Vector3Int targetSize, float targetCellVolume)
    {
        PerlinNoise perlinNoise = new PerlinNoise(new Vector2Int(targetSize.x, targetSize.y), new Vector2(5, 5));

        // determine added volume
        float max_added_volume = BaseVolume;
        float[,] added_volumes = new float[targetSize.y, targetSize.x];
        float added_volumes_min = int.MaxValue;

        // volume is the lowest possible value, perlin noise is added on top of that
        for (int i = 0; i < targetSize.y; i++)
        {
            for (int j = 0; j < WidthRatio * targetSize.x; j++)
            {
                float noise = perlinNoise.ValueAt(j, i);
                float added_volume = noise * max_added_volume;

                added_volumes[i, j] = added_volume;

                if (added_volume < added_volumes_min)
                    added_volumes_min = added_volume;
            }
        }

        // set volume to reservoir
        for (int i = 0; i < targetSize.y; i++)
        {
            for (int j = 0; j < WidthRatio * targetSize.x; j++)
            {
                float v = BaseVolume + (added_volumes[i, j] - added_volumes_min);
                SetVolume(targetInfo, target, targetSize, targetCellVolume, j, i, v);
            }
        }
    }
}