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
        return Size == other.Size && WriteIndex == other.WriteIndex && Mathf.Abs(other.Volume - Volume) < 0.0001f;
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

    public PaintGrid(Vector3Int size)
    {
        Size = size;

        Info = new ComputeBuffer(size.x * size.y, ColumnInfo.SizeInBytes);
        InfoData = new ColumnInfo[size.x * size.y];
        Info.SetData(InfoData);

        Content = new ComputeBuffer(size.x * size.y * size.z, Paint.SizeInBytes);
        ContentData = new Paint[size.x * size.y * size.z];
        Content.SetData(ContentData);
    }

    public void Fill(ReservoirFiller filler)
    {
        filler.Fill(InfoData, ContentData, Size);
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

    public void Dispose()
    {
        Info.Dispose();
        Content.Dispose();
    }
}
