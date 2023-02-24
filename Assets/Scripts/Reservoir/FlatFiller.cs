using System;
using UnityEngine;

public class FlatFiller : ReservoirFiller
{
    public FlatFiller(bool colorGradient = false) : base(colorGradient) { }

    public override void Fill(Color_ color, int volume, Paint[] target, Vector2Int targetSize)
    {
        // HACK multiply volume by 100, because the shader sees 100 as 1 unit of paint
        volume *= 1000;

        //int sum = 0;
        // set volume to reservoir
        for (int i = 0; i < targetSize.y; i++)
        {
            for (int j = 0; j < targetSize.x; j++)
            {
                target[XY(j, i, targetSize.x)] = new Paint(Colors.GetColor(color), volume);
                //sum += paint.Volume;
            }
        }
        //Debug.Log("Filled with " + sum);

        AddColorGradient(target, targetSize);
    }
}
