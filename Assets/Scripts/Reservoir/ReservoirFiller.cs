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

    public void Fill(ColumnInfo[] targetInfo, Paint[] target, Vector3Int targetSize, float targetCellVolume)
    {
        VolumeFiller.Fill(targetInfo, target, targetSize, targetCellVolume);
        ColorFiller.Fill(targetInfo, target, targetSize);
    }
}