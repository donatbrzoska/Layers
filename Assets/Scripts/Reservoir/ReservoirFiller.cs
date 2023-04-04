using UnityEngine;

public class ReservoirFiller
{
    ColorFiller ColorFiller;
    VolumeFiller VolumeFiller;

    public ReservoirFiller(ColorFiller colorFiller, VolumeFiller volumeFiller)
    {
        ColorFiller = colorFiller;
        VolumeFiller = volumeFiller;
    }

    public void Fill(Paint[] target, Vector2Int targetSize)
    {
        ColorFiller.Fill(target, targetSize);
        VolumeFiller.Fill(target, targetSize);
    }
}