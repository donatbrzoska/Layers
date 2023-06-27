using UnityEngine;

public abstract class ColorFiller
{
    protected ColorSpace ColorSpace;

    public ColorFiller(ColorSpace colorSpace)
    {
        ColorSpace = colorSpace;
    }

    protected void SetColor(ColumnInfo[] targetInfo, Paint[] target, Vector3Int targetSize, int x, int y, Color color)
    {
        Color actualColor = color;
        if (ColorSpace == ColorSpace.RYB)
        {
            actualColor = Colors.RGB2RYB(actualColor);
        }

        ColumnInfo ci = targetInfo[IndexUtil.XY(x, y, targetSize.x)];
        for (int z = 0; z < ci.Size; z++)
        {
            target[IndexUtil.XYZ(x, y, z, targetSize.x, targetSize.y)].Color = actualColor;
        }
    }

    public abstract void Fill(ColumnInfo[] targetInfo, Paint[] target, Vector3Int targetSize);
}
