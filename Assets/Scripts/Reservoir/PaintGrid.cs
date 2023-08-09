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
    public Vector2Int Size2D
    {
        get
        {
            return new Vector2Int(Size.x, Size.y);
        }
    }
    public float CellVolume;

    public PaintGrid(Vector3Int size, float cellVolume)
    {
        Size = size;
        CellVolume = cellVolume;

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
    public void ReadbackInfo()
    {
        Info.GetData(InfoData);
    }

    // Only used for testing purposes
    public void ReadbackContent()
    {
        Content.GetData(ContentData);
    }

    // Only used for testing purposes
    public ColumnInfo Get(int x, int y)
    {
        return InfoData[IndexUtil.XY(x, y, Size2D)];
    }

    // Only used for testing purposes
    public Vector4[] GetColors()
    {
        Vector4[] colors = new Vector4[ContentData.Length];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = ContentData[i].Color;
        }
        return colors;
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
    public float[] GetVolumes(ShaderRegion shaderRegion)
    {
        float[] volumes = new float[shaderRegion.PixelCount];
        for (int i = 0; i < shaderRegion.Size.y; i++)
        {
            for (int j = 0; j < shaderRegion.Size.x; j++)
            {
                int y = i + shaderRegion.Position.y;
                int x = j + shaderRegion.Position.x;
                volumes[IndexUtil.XY(j, i, shaderRegion.Size)] = Get(x, y).Volume;
            }
        }
        return volumes;
    }

    public void Dispose()
    {
        Info.Dispose();
        Content.Dispose();
    }
}
