using UnityEngine;

public abstract class VolumeFiller
{
    protected float WidthRatio;
    protected float BaseVolume;

    protected VolumeFiller(float widthRatio, float baseVolume)
    {
        WidthRatio = Mathf.Clamp01(widthRatio);
        BaseVolume = baseVolume * Paint.UNIT;
    }

    protected void SetVolume(ColumnInfo[] targetInfo, Paint[] target, Vector3Int targetSize, float targetCellVolume, int x, int y, float volume)
    {
        float left = volume;
        int z = 0;

        while (left > 0 && z < targetSize.z)
        {
            float cellVolume = Mathf.Min(targetCellVolume, left);
            target[IndexUtil.XYZ(x, y, z, targetSize)].Volume = cellVolume;

            left -= cellVolume;
            z++;
        }
        targetInfo[IndexUtil.XY(x, y, new Vector2Int(targetSize.x, targetSize.y))].Size = z;
        targetInfo[IndexUtil.XY(x, y, new Vector2Int(targetSize.x, targetSize.y))].WriteIndex = z;
        targetInfo[IndexUtil.XY(x, y, new Vector2Int(targetSize.x, targetSize.y))].Volume = volume - left;
    }

    public abstract void Fill(ColumnInfo[] targetInfo, Paint[] target, Vector3Int targetSize, float targetCellVolume);
}
