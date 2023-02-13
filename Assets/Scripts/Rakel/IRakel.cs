﻿using UnityEngine;

public interface IRakel
{   public Vector3 Anchor { get; }
    public float Length { get; }
    public float Width { get; }
    public void Apply(
        Vector3 rakelPosition,
        float rakelRotation,
        float rakelTilt,
        EmitMode rakelEmitMode,
        int discardReservoirVolumeThreshhold,
        int reservoirSmoothingKernelSize,
        OilPaintCanvas oilPaintCanvas);
    public void Fill(Paint paint, ReservoirFiller filler);
    public void Dispose();
}
