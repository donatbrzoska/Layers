using UnityEngine;

public class FlatColorFiller : ColorFiller
{
    Color_ Color_;

    public FlatColorFiller(Color_ color_)
    {
        Color_ = color_;
    }

    public override void Fill(Paint[] target, Vector2Int targetSize)
    {
        for (int i = 0; i < target.Length; i++)
        {
            target[i].Color = Colors.GetColor(Color_);
        }
    }
}
