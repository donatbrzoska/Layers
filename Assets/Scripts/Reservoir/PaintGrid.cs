using UnityEngine;

public struct ColumnInfo
{
    public static int SizeInBytes = 2 * sizeof(int) + sizeof(float);

    public int Size;
    public int WriteIndex;
    public float Volume;

    public ColumnInfo(int size, int writeIndex, float volume)
    {
        Size = size;
        WriteIndex = writeIndex;
        Volume = volume;
    }

    public override bool Equals(object other_)
    {
        ColumnInfo other = (ColumnInfo)other_;
        return Size == other.Size && WriteIndex == other.WriteIndex && Mathf.Abs(other.Volume - Volume) < 0.01f;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString() + string.Format("(Size={0}, WriteIndex={1}, Volume={2})", Size, WriteIndex, Volume);
    }

    public static bool operator ==(ColumnInfo a, ColumnInfo b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(ColumnInfo a, ColumnInfo b)
    {
        return !a.Equals(b);
    }
}

public class PaintGrid
{
    public ComputeBuffer Info;
    public ColumnInfo[] InfoData;
    public ComputeBuffer Content;
    public Paint[] ContentData;
    public Vector3Int Size;
    public float CellVolume;
    public int DiffuseDepth;
    public float DiffuseRatio;

    public PaintGrid(Vector3Int size, float cellVolume, int diffuseDepth, float diffuseRatio)
    {
        Size = size;
        CellVolume = cellVolume;
        DiffuseDepth = diffuseDepth;
        DiffuseRatio = diffuseRatio;

        Info = new ComputeBuffer(size.x * size.y, ColumnInfo.SizeInBytes);
        InfoData = new ColumnInfo[size.x * size.y];
        Info.SetData(InfoData);

        Content = new ComputeBuffer(size.x * size.y * size.z, Paint.SizeInBytes);
        ContentData = new Paint[size.x * size.y * size.z];
        Content.SetData(ContentData);
    }

    public void Fill(ReservoirFiller filler)
    {
        filler.Fill(InfoData, ContentData, Size, CellVolume);
        Info.SetData(InfoData);
        Content.SetData(ContentData);
    }

    // Only used for testing purposes
    public void Readback()
    {
        Info.GetData(InfoData);
    }

    // Only used for testing purposes
    public ColumnInfo Get(int x, int y)
    {
        return InfoData[IndexUtil.XY(x, y, Size.x)];
    }

    // Only used for testing purposes
    public int[] GetSizes()
    {
        int[] sizes = new int[InfoData.Length];
        for (int i = 0; i < sizes.Length; i++)
        {
            sizes[i] = InfoData[i].Size;
        }
        return sizes;
    }

    // Only used for testing purposes
    public int[] GetWriteIndexes()
    {
        int[] indexes = new int[InfoData.Length];
        for (int i = 0; i < indexes.Length; i++)
        {
            indexes[i] = InfoData[i].WriteIndex;
        }
        return indexes;
    }

    // Only used for testing purposes
    public float[] GetVolumes()
    {
        float[] volumes = new float[InfoData.Length];
        for (int i = 0; i < volumes.Length; i++)
        {
            volumes[i] = InfoData[i].Volume;
        }
        return volumes;
    }

    public void Dispose()
    {
        Info.Dispose();
        Content.Dispose();
    }
}
