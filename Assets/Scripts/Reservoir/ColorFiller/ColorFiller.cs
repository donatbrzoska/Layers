using UnityEngine;

public abstract class ColorFiller
{
    protected ColorSpace ColorSpace;

    public ColorFiller(ColorSpace colorSpace)
    {
        ColorSpace = colorSpace;
    }

    protected void SetColor(ColumnInfo[] targetInfo, Paint[] target, Vector3Int targetSize, int x, int y, Vector3 color)
    {
        Vector3 actualColor = color;
        if (ColorSpace == ColorSpace.RYB)
        {
            actualColor = Colors.RGB2RYB(actualColor);
        }

        ColumnInfo ci = targetInfo[IndexUtil.XY(x, y, new Vector2Int(targetSize.x, targetSize.y))];
        for (int z = 0; z < ci.Size; z++)
        {
            target[IndexUtil.XYZ(x, y, z, targetSize)].Color = actualColor;
        }
    }

    public abstract void Fill(ColumnInfo[] targetInfo, Paint[] target, Vector3Int targetSize);
}
