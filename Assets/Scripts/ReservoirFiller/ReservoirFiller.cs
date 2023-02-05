using UnityEngine;

public abstract class ReservoirFiller
{
    protected bool ColorGradient;

    public ReservoirFiller(bool colorGradient)
    {
        ColorGradient = colorGradient;
    }

    protected void AddColorGradient(Paint[] target, Vector2Int targetSize)
    {
        if (ColorGradient)
        {
            for (int i = 0; i < targetSize.y; i++)
            {
                for (int j = 0; j < targetSize.x; j++)
                {
                    float r = i / (float)targetSize.y;
                    float g = j / (float)targetSize.x;
                    float b = 0.5f * (1 - g) + 0.5f * (1 - r);

                    //float r = i / (float)targetSize.y;
                    //float g = j / (float)targetSize.x;
                    //float b = 0.5f * g + 0.5f * r;

                    //float r = 1 - i / (float)targetSize.y;
                    //float g = 1 - j / (float)targetSize.x;
                    //float b = 1 - 0.5f * (1 - g) + 0.5f * (1 - r);

                    //float r = 1 - i / (float)targetSize.y;
                    //float g = 1 - j / (float)targetSize.x;
                    //float b = 1 - 0.5f * g * 0.5f * r;

                    //float r = i / (float)targetSize.y;
                    //float b = j / (float)targetSize.x;
                    //float g = 0.5f * (1 - b) + 0.5f * (1 - r);

                    //float g = i / (float)targetSize.y;
                    //float b = j / (float)targetSize.x;
                    //float r = 0.5f * (1 - b) + 0.5f * (1 - g);

                    target[XY(j, i, targetSize.x)].Color = new Color(r, g, b, 1);
                }
            }
        }
    }

    protected int XY(int x, int y, int width) {
        return y * width + x;
    }

    public abstract void Fill(Paint paint, Paint[] target, Vector2Int targetSize);
}