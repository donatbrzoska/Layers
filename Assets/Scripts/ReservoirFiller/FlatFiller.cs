﻿using System;
using UnityEngine;

public class FlatFiller : ReservoirFiller
{
    public override void Fill(Paint paint, Paint[] target, Vector2Int targetSize)
    {
        // HACK multiply volume by 100, because the shader sees 100 as 1 unit of paint
        paint.Volume *= 1000;

        // set volume to reservoir
        for (int i = 0; i < targetSize.y; i++)
        {
            for (int j = 0; j < targetSize.x; j++)
            {
                target[XY(j, i, targetSize.x)] = new Paint(paint);
            }
        }
    }
}