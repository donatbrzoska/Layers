using System;
using UnityEngine;

public class PerlinVolumeFiller : VolumeFiller
{
    public PerlinVolumeFiller(float baseVolume) : base(baseVolume) { }

    public override void Fill(Paint[] target, Vector2Int targetSize)
    {
        // determine added volume
        float max_added_volume = BaseVolume;

        float scale = 5f; // bigger values == larger areas of the perlin noise terrain == more frequent noise
        float offset_x = UnityEngine.Random.Range(0f, 1000f);
        float offset_y = UnityEngine.Random.Range(0f, 1000f);

        float[,] added_volumes = new float[targetSize.y, targetSize.x];
        float added_volumes_min = int.MaxValue;

        // volume is the lowest possible value, perlin noise is added on top of that
        for (int i = 0; i < targetSize.y; i++)
        {
            for (int j = 0; j < targetSize.x; j++)
            {
                int maxSide = Mathf.Max(targetSize.x, targetSize.y);
                float x = (float)j / maxSide * scale + offset_x;
                float y = (float)i / maxSide * scale + offset_y;

                float clipped_noise = Mathf.Clamp01(Mathf.PerlinNoise(x, y));
                float added_volume = clipped_noise * max_added_volume;

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