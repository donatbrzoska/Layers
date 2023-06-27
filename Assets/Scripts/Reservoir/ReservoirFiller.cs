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

    public void Fill(ColumnInfo[] targetInfo, Paint[] target, Vector3Int targetSize)
    {
        VolumeFiller.Fill(targetInfo, target, targetSize);
        ColorFiller.Fill(targetInfo, target, targetSize);
    }
}