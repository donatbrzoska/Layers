using UnityEngine;

public class FlatColorFiller : ColorFiller
{
    Color_ Color_;

    public FlatColorFiller(Color_ color_, ColorSpace colorSpace) : base(colorSpace)
    {
        Color_ = color_;
    }

    public override void Fill(Paint[] target, Vector2Int targetSize)
    {
        for (int i = 0; i < target.Length; i++)
        {
            SetColor(target, i, Colors.GetColor(Color_));
        }
    }
}
