using UnityEngine;

public class FlatColorFiller : ColorFiller
{
    Color_ Color_;

    public FlatColorFiller(Color_ color_, ColorSpace colorSpace) : base(colorSpace)
    {
        Color_ = color_;
    }

    public override void Fill(ColumnInfo[] targetInfo, Paint[] target, Vector3Int targetSize)
    {
        for (int y = 0; y < targetSize.y; y++)
        {
            for (int x = 0; x < targetSize.x; x++)
            {
                SetColor(targetInfo, target, targetSize, x, y, Colors.GetColor(Color_));
            }
        }
    }
}
