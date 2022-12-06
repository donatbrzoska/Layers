using UnityEngine;

public class PerlinNoiseFiller : ReservoirFiller
{
    public override void Fill(Paint paint, Color[] colorsTarget, int[] volumesTarget, Vector2Int targetSize)
    {
        int max_added_volume = paint.Volume;

        float scale = 10f;
        float offset_x = UnityEngine.Random.Range(0f, 1000f);
        float offset_y = UnityEngine.Random.Range(0f, 1000f);

        float[,] added_volumes = new float[targetSize.y, targetSize.x];
        float added_volumes_min = int.MaxValue;

        for (int i = 0; i < targetSize.y; i++)
        {
            for (int j = 0; j < targetSize.x; j++)
            {
                float x = (float)j / targetSize.x * scale + offset_x;
                float y = (float)i / targetSize.y * scale + offset_y;

                float clipped_noise = Mathf.Max(Mathf.Min(Mathf.PerlinNoise(x, y), 1), 0);
                float added_volume = clipped_noise * max_added_volume;

                added_volumes[i, j] = added_volume;

                if (added_volume < added_volumes_min)
                    added_volumes_min = added_volume;
            }
        }

        for (int i = 0; i < targetSize.y; i++)
        {
            for (int j = 0; j < targetSize.x; j++)
            {
                Paint actual = new Paint(paint);
                actual.Volume += (int)(Mathf.Pow(added_volumes[i, j] - added_volumes_min, 2) / 2);
                actual.Volume *= 100;
                //actual.Volume += (int) (added_volumes[i, j] - added_volumes_min);

                colorsTarget[XY(j, i, targetSize.x)] = actual.Color;
                volumesTarget[XY(j, i, targetSize.x)] = actual.Volume;
            }
        }

        //for (int i = 0; i < Height; i++)
        //{
        //    for (int j = 0; j < Width; j++)
        //    {
        //        ApplicationReservoir.Set(j, i, paint);
        //    }
        //}
    }
}