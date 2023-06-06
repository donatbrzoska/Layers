using System;
using UnityEngine;

public class PerlinVolumeFiller : VolumeFiller
{
    public PerlinVolumeFiller(float baseVolume) : base(baseVolume) { }

    public override void Fill(Paint[] target, Vector2Int targetSize)
    {
        PerlinNoise perlinNoise = new PerlinNoise(targetSize, new Vector2(5, 5));

        // determine added volume
        float max_added_volume = BaseVolume;
        float[,] added_volumes = new float[targetSize.y, targetSize.x];
        float added_volumes_min = int.MaxValue;

        // volume is the lowest possible value, perlin noise is added on top of that
        for (int i = 0; i < targetSize.y; i++)
        {
            for (int j = 0; j < targetSize.x; j++)
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
            for (int j = 0; j < targetSize.x; j++)
            {
                target[IndexUtil.XY(j, i, targetSize.x)].Volume = BaseVolume + (added_volumes[i, j] - added_volumes_min);
            }
        }
    }
}