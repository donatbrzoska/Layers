using UnityEngine;

public class FlatVolumeFiller : VolumeFiller
{
    public FlatVolumeFiller(float baseVolume) : base(baseVolume) { }

    public override void Fill(Paint[] target, Vector2Int targetSize)
    {
        for (int i=0; i<target.Length; i++)
        {
            target[i].Volume = BaseVolume;
        }
    }
}
