using NUnit.Framework;
using UnityEngine;

class IndexVolumeFiller : VolumeFiller
{
    public IndexVolumeFiller(float baseVolume) : base(baseVolume) { }

    public override void Fill(ColumnInfo[] targetInfo, Paint[] target, Vector3Int targetSize)
    {
        for (int y = 0; y < targetSize.y; y++)
        {
            string res = "";
            for (int x = 0; x < targetSize.x; x++)
            {
                SetVolume(targetInfo, target, targetSize, x, y, y * targetSize.x + x);
                res += string.Format("{0,3:0.}, ", y * targetSize.x + x);
            }
            //Debug.Log(res + "\n");
        }
    }
}

class OnesVolumeFiller : VolumeFiller
{
    public OnesVolumeFiller(float baseVolume) : base(baseVolume) { }

    public override void Fill(ColumnInfo[] targetInfo, Paint[] target, Vector3Int targetSize)
    {
        for (int y = 0; y < targetSize.y; y++)
        {
            string res = "";
            for (int x = 0; x < targetSize.x; x++)
            {
                SetVolume(targetInfo, target, targetSize, x, y, 1);
                res += string.Format("{0,3:0.}, ", y * targetSize.x + x);
            }
            //Debug.Log(res + "\n");
        }
    }
}

public class TestReservoirReduceVolume
{
    Reservoir Reservoir;
    ColorFiller ColorFiller;

    [SetUp]
    public void Setup()
    {
        ColorFiller = new FlatColorFiller(Color_.TitanWhite, ColorSpace.RGB);
        new FileLogger_().OnEnable();
    }

    [TearDown]
    public void Teardown()
    {
        Reservoir.Dispose();

        new FileLogger_().OnDisable();
    }

    [Test]
    public void exact_fit_max()
    {
        Vector2Int TEXTURE_SIZE = new Vector2Int(24, 16);

        // Arrange
        Reservoir = new Reservoir(1, TEXTURE_SIZE.x, TEXTURE_SIZE.y, TEXTURE_SIZE.x * TEXTURE_SIZE.y);
        Reservoir.Fill(new ReservoirFiller(ColorFiller, new IndexVolumeFiller(0)));

        Reservoir.Duplicate();


        // Act
        Vector2Int reductionPosition = Vector2Int.zero;
        Vector2Int reductionSize = TEXTURE_SIZE;
        // calculate points from reduction info because that is how the function is going to be used
        ShaderRegion sr = new ShaderRegion(
            reductionPosition,
            reductionPosition + new Vector2Int(reductionSize.x - 1, 0),
            reductionPosition + new Vector2Int(0, reductionSize.y - 1),
            reductionPosition + new Vector2Int(reductionSize.x - 1, reductionSize.y - 1));
        Reservoir.ReduceVolume(sr, ReduceFunction.Max, false);


        // Assert
        Reservoir.Readback();
        float result = Reservoir.GetFromDuplicate(reductionPosition.x, reductionPosition.y).Volume;

        Assert.AreEqual(383, result);
    }

    [Test]
    public void odd_width_max()
    {
        Vector2Int TEXTURE_SIZE = new Vector2Int(13, 9);

        // Arrange
        Reservoir = new Reservoir(1, TEXTURE_SIZE.x, TEXTURE_SIZE.y, TEXTURE_SIZE.x * TEXTURE_SIZE.y);
        Reservoir.Fill(new ReservoirFiller(ColorFiller, new IndexVolumeFiller(0)));

        Reservoir.Duplicate();


        // Act
        Vector2Int reductionPosition = new Vector2Int(1, 2);
        Vector2Int reductionSize = new Vector2Int(9, 6);
        // calculate points from reduction info because that is how the function is going to be used
        ShaderRegion sr = new ShaderRegion(
            reductionPosition,
            reductionPosition + new Vector2Int(reductionSize.x - 1, 0),
            reductionPosition + new Vector2Int(0, reductionSize.y - 1),
            reductionPosition + new Vector2Int(reductionSize.x - 1, reductionSize.y - 1));
        Reservoir.ReduceVolume(sr, ReduceFunction.Max, false);


        // Assert
        Reservoir.Readback();
        float result = Reservoir.GetFromDuplicate(reductionPosition.x, reductionPosition.y).Volume;

        Assert.AreEqual(100, result);
    }

    // this was really only written because it was easier to debug the already known scenario
    [Test]
    public void odd_width_add()
    {
        Vector2Int TEXTURE_SIZE = new Vector2Int(13, 9);

        // Arrange
        Reservoir = new Reservoir(1, TEXTURE_SIZE.x, TEXTURE_SIZE.y, TEXTURE_SIZE.x * TEXTURE_SIZE.y);
        Reservoir.Fill(new ReservoirFiller(ColorFiller, new OnesVolumeFiller(0)));

        Reservoir.Duplicate();


        // Act
        Vector2Int reductionPosition = new Vector2Int(1, 2);
        Vector2Int reductionSize = new Vector2Int(9, 6);
        // calculate points from reduction info because that is how the function is going to be used
        ShaderRegion sr = new ShaderRegion(
            reductionPosition,
            reductionPosition + new Vector2Int(reductionSize.x - 1, 0),
            reductionPosition + new Vector2Int(0, reductionSize.y - 1),
            reductionPosition + new Vector2Int(reductionSize.x - 1, reductionSize.y - 1));
        Reservoir.ReduceVolume(sr, ReduceFunction.Add, false);


        // Assert
        Reservoir.Readback();
        float result = Reservoir.GetFromDuplicate(reductionPosition.x, reductionPosition.y).Volume;

        Assert.AreEqual(reductionSize.x * reductionSize.y, result);
    }

    [Test]
    public void odd_height_add()
    {
        Vector2Int TEXTURE_SIZE = new Vector2Int(16, 24);

        // Arrange
        Reservoir = new Reservoir(1, TEXTURE_SIZE.x, TEXTURE_SIZE.y, TEXTURE_SIZE.x * TEXTURE_SIZE.y);
        Reservoir.Fill(new ReservoirFiller(ColorFiller, new OnesVolumeFiller(0)));

        Reservoir.Duplicate();


        // Act
        Vector2Int reductionPosition = new Vector2Int(3, 4);
        Vector2Int reductionSize = new Vector2Int(12, 19);
        // calculate points from reduction info because that is how the function is going to be used
        ShaderRegion sr = new ShaderRegion(
            reductionPosition,
            reductionPosition + new Vector2Int(reductionSize.x - 1, 0),
            reductionPosition + new Vector2Int(0, reductionSize.y - 1),
            reductionPosition + new Vector2Int(reductionSize.x - 1, reductionSize.y - 1));
        Reservoir.ReduceVolume(sr, ReduceFunction.Add, false);


        // Assert
        Reservoir.Readback();
        float result = Reservoir.GetFromDuplicate(reductionPosition.x, reductionPosition.y).Volume;

        Assert.AreEqual(reductionSize.x * reductionSize.y, result);
    }
}
