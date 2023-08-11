using UnityEngine;

public class GradientColorFiller : ColorFiller
{
    ColorMode ColorMode;

    public GradientColorFiller(ColorMode colorMode, ColorSpace colorSpace) : base(colorSpace)
    {
        ColorMode = colorMode;
    }

    public override void Fill(ColumnInfo[] targetInfo, Paint[] target, Vector3Int targetSize)
    {
        for (int i = 0; i < targetSize.y; i++)
        {
            for (int j = 0; j < targetSize.x; j++)
            {
                float r = 0;
                float g = 0;
                float b = 0;

                switch (ColorMode)
                {
                    case ColorMode.Colorful:
                        r = i / (float)targetSize.y;
                        g = j / (float)targetSize.x;
                        b = 0.5f * (1 - g) + 0.5f * (1 - r);
                        break;
                    case ColorMode.Colorful2:
                        r = i / (float)targetSize.y;
                        b = j / (float)targetSize.x;
                        g = 0.5f * (1 - b) + 0.5f * (1 - r);
                        break;
                    case ColorMode.RedGreen:
                        r = i / (float)targetSize.y;
                        g = j / (float)targetSize.x;
                        b = 0.5f * g + 0.5f * r;
                        break;
                    case ColorMode.BluePurple:
                        r = 1 - i / (float)targetSize.y;
                        g = 1 - j / (float)targetSize.x;
                        b = 1 - 0.5f * (1 - g) + 0.5f * (1 - r);
                        break;
                    case ColorMode.Colorful3:
                        g = i / (float)targetSize.y;
                        b = j / (float)targetSize.x;
                        r = 0.5f * (1 - b) + 0.5f * (1 - g);
                        break;
                    default:
                        break;
                }

                SetColor(targetInfo, target, targetSize, j, i, new Vector3(r, g, b));
            }
        }
    }
}
