using UnityEngine;

public class PerlinNoiseFiller : ReservoirFiller
{
    public PerlinNoiseFiller(bool colorGradient = false) : base(colorGradient) { }

    // Adds some perlin noised volume to the desired volume
    public override void Fill(Paint paint, Paint[] target, Vector2Int targetSize)
    {
        // HACK multiply volume by 100, because the shader sees 100 as 1 unit of paint
        paint.Volume *= 1000;

        // determine added volume
        int max_added_volume = paint.Volume;

        float scale = 5f; // bigger values == larger areas of the perlin noise terrain == more frequent noise
        float offset_x = Random.Range(0f, 1000f);
        float offset_y = Random.Range(0f, 1000f);

        float[,] added_volumes = new float[targetSize.y, targetSize.x];
        float added_volumes_min = int.MaxValue;

        for (int i = 0; i < targetSize.y; i++)
        {
            for (int j = 0; j < targetSize.x; j++)
            {
                int maxSide = Mathf.Max(targetSize.x, targetSize.y);
                float x = (float)j / maxSide * scale + offset_x;
                float y = (float)i / maxSide * scale + offset_y;

                float clipped_noise = Mathf.Max(Mathf.Min(Mathf.PerlinNoise(x, y), 1), 0);
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
                Paint actual = new Paint(paint);
                actual.Volume += (int)(added_volumes[i, j] - added_volumes_min);
                //actual.Volume += (int)Mathf.Pow(added_volumes[i, j] - added_volumes_min, 2);
                //actual.Volume += (int)(Mathf.Pow(added_volumes[i, j] - added_volumes_min, 2) / 2);

                target[XY(j, i, targetSize.x)] = actual;
            }
        }

        AddColorGradient(target, targetSize);

        //int max_added_volume = paint.Volume * 100;

        //float scale = 20f;
        //float offset_x = UnityEngine.Random.Range(0f, 1000f);
        //float offset_y = UnityEngine.Random.Range(0f, 1000f);

        //for (int i = 0; i < targetSize.y; i++)
        //{
        //    for (int j = 0; j < targetSize.x; j++)
        //    {
        //        int maxSide = Mathf.Max(targetSize.x, targetSize.y);
        //        float x = (float)j / maxSide * scale + offset_x;
        //        float y = (float)i / maxSide * scale + offset_y;

        //        float clipped_noise = Mathf.Max(Mathf.Min(Mathf.PerlinNoise(x, y), 1), 0);

        //        Paint actual = new Paint(paint);
        //        actual.Volume = (int)(clipped_noise * max_added_volume);
        //        target[XY(j, i, targetSize.x)] = actual;
        //    }
        //}
    }
}