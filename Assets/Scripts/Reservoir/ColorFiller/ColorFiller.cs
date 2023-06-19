using UnityEngine;

public abstract class ColorFiller
{
    protected ColorSpace ColorSpace;

    public ColorFiller(ColorSpace colorSpace)
    {
        ColorSpace = colorSpace;
    }

    protected void SetColor(Paint[] target, int index, Color color)
    {
        Color actualColor = color;
        if (ColorSpace == ColorSpace.RYB)
        {
            actualColor = Colors.RGB2RYB(actualColor);
        }
        target[index].Color = actualColor;
    }

    public abstract void Fill(Paint[] target, Vector2Int targetSize);
}
