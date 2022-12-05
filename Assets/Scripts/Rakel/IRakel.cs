﻿using UnityEngine;

public interface IRakel
{   public Vector3 Anchor { get; }
    public float Length { get; }
    public float Width { get; }
    public void Apply(
        Vector3 rakelPosition,
        float rakelRotation,
        float rakelTilt,
        WorldSpaceCanvas wsc,
        RenderTexture canvasTexture);
    public void Fill(Paint paint, ReservoirFiller filler);
    public void Dispose();
}
