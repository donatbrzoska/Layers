using UnityEngine;

public interface IRakel
{   public Vector3 Anchor { get; }
    public float Length { get; }
    public float Width { get; }
    public void Apply(
        Vector3 rakelPosition,
        float rakelRotation,
        float rakelTilt,
        TransferConfiguration transferConfiguration,
        OilPaintCanvas oilPaintCanvas);
    public void Fill(_Color color, int volume, ReservoirFiller filler);
    public void Dispose();
}
