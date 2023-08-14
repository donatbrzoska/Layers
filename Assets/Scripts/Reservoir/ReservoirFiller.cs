using UnityEngine;

public class ReservoirFiller
{
    Color_ Color;
    ColorSpace ColorSpace;

    private float WidthRatio;
    private float BaseVolume;
    private float NoiseVolume;
    private float NoiseVolumeFrequencyX;
    private float NoiseVolumeFrequencyY;

    public ReservoirFiller(Color_ color, ColorSpace colorSpace, float widthRatio, float baseVolume, float noiseVolume, float noiseVolumeFrequencyX, float noiseVolumeFrequencyY)
    {
        Color = color;
        ColorSpace = colorSpace;

        WidthRatio = Mathf.Clamp01(widthRatio);
        BaseVolume = baseVolume * Paint.UNIT;
        NoiseVolume = noiseVolume * Paint.UNIT;
        NoiseVolumeFrequencyX = noiseVolumeFrequencyX;
        NoiseVolumeFrequencyY = noiseVolumeFrequencyY;
    }

    public ReservoirFiller(Color_ color, float baseVolume) : this(color, ColorSpace.RGB, 1, baseVolume, 0, 0, 0) { }

    protected void AddPaint(ColumnInfo[] targetInfo, Paint[] target, Vector3Int targetSize, float targetCellVolume, int x, int y, float volume)
    {
        float left = volume;
        int z = targetInfo[IndexUtil.XY(x, y, new Vector2Int(targetSize.x, targetSize.y))].Size;
        float volumeBefore = targetInfo[IndexUtil.XY(x, y, new Vector2Int(targetSize.x, targetSize.y))].Volume;

        while (left > 0 && z < targetSize.z)
        {
            // set volume
            float cellVolume = Mathf.Min(targetCellVolume, left);
            target[IndexUtil.XYZ(x, y, z, targetSize)].Volume = cellVolume;

            // set color
            Vector3 actualColor = Colors.GetColor(Color);
            if (ColorSpace == ColorSpace.RYB)
            {
                actualColor = Colors.RGB2RYB(actualColor);
            }
            target[IndexUtil.XYZ(x, y, z, targetSize)].Color = actualColor;

            left -= cellVolume;
            z++;
        }
        targetInfo[IndexUtil.XY(x, y, new Vector2Int(targetSize.x, targetSize.y))].Size = z;
        targetInfo[IndexUtil.XY(x, y, new Vector2Int(targetSize.x, targetSize.y))].WriteIndex = z;
        targetInfo[IndexUtil.XY(x, y, new Vector2Int(targetSize.x, targetSize.y))].Volume = volumeBefore + volume - left;
    }

    public virtual void Fill(ColumnInfo[] targetInfo, Paint[] target, Vector3Int targetSize, float targetCellVolume)
    {
        PerlinNoise perlinNoise = new PerlinNoise(new Vector2Int(targetSize.x, targetSize.y), new Vector2(NoiseVolumeFrequencyX, NoiseVolumeFrequencyY));

        // determine added volume
        float max_added_volume = NoiseVolume;
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
                AddPaint(targetInfo, target, targetSize, targetCellVolume, j, i, v);
            }
        }
    }
}