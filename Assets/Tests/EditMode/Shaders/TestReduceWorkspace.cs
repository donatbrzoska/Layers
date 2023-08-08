using NUnit.Framework;
using UnityEngine;

class IndexVolumeFiller : VolumeFiller
{
    public IndexVolumeFiller(float widthPart, float baseVolume) : base(widthPart, baseVolume) { }

    public override void Fill(ColumnInfo[] targetInfo, Paint[] target, Vector3Int targetSize, float targetCellVolume)
    {
        for (int y = 0; y < targetSize.y; y++)
        {
            string res = "";
            for (int x = 0; x < targetSize.x; x++)
            {
                SetVolume(targetInfo, target, targetSize, targetCellVolume, x, y, y * targetSize.x + x);
                res += string.Format("{0,3:0.}, ", y * targetSize.x + x);
            }
            //Debug.Log(res + "\n");
        }
    }
}

class OnesVolumeFiller : VolumeFiller
{
    public OnesVolumeFiller(float widthPart, float baseVolume) : base(widthPart, baseVolume) { }

    public override void Fill(ColumnInfo[] targetInfo, Paint[] target, Vector3Int targetSize, float targetCellVolume)
    {
        for (int y = 0; y < targetSize.y; y++)
        {
            string res = "";
            for (int x = 0; x < targetSize.x; x++)
            {
                SetVolume(targetInfo, target, targetSize, targetCellVolume, x, y, 1);
                res += string.Format("{0,3:0.}, ", y * targetSize.x + x);
            }
            //Debug.Log(res + "\n");
        }
    }
}

public class TestReduceWorkspace
{
    private const float CELL_VOLUME = 1;

    private const float FILL_WIDTH_PART = 1;

    Vector2Int TextureSize;

    CanvasReservoir Reservoir;
    ComputeBuffer ReduceResult;

    ColorFiller ColorFiller;

    [SetUp]
    public void Setup()
    {
        ColorFiller = new FlatColorFiller(Color_.TitanWhite, ColorSpace.RGB);
        new FileLogger_().OnEnable();
    }

    public void SetupReservoir()
    {
        Reservoir = new CanvasReservoir(1, TextureSize.x, TextureSize.y, TextureSize.x * TextureSize.y, CELL_VOLUME);
    }

    [TearDown]
    public void Teardown()
    {
        Reservoir.Dispose();

        new FileLogger_().OnDisable();
    }

    float Execute(ShaderRegion shaderRegion, InternalReduceFunction reduceFunction, bool debugEnabled)
    {
        Reservoir.ReduceWorkspace(shaderRegion, reduceFunction, debugEnabled);

        ReduceResult = new ComputeBuffer(1, sizeof(float));
        Reservoir.ExtractReducedValue(shaderRegion, ReduceResult);
        float[] result = new float[1];
        ReduceResult.GetData(result);
        ReduceResult.Dispose();
        return result[0];
    }

    [Test]
    public void exact_fit_max()
    {
        TextureSize = new Vector2Int(24, 16);
        SetupReservoir();

        // Arrange
        Reservoir.Fill(new ReservoirFiller(ColorFiller, new IndexVolumeFiller(FILL_WIDTH_PART, 0)));

        Reservoir.CopyVolumesToWorkspace();


        // Act
        Vector2Int reductionPosition = Vector2Int.zero;
        Vector2Int reductionSize = TextureSize;
        // calculate points from reduction info because that is how the function is going to be used
        ShaderRegion sr = new ShaderRegion(
            reductionPosition,
            reductionPosition + new Vector2Int(reductionSize.x - 1, 0),
            reductionPosition + new Vector2Int(0, reductionSize.y - 1),
            reductionPosition + new Vector2Int(reductionSize.x - 1, reductionSize.y - 1));
        float result = Execute(sr, InternalReduceFunction.Max, false);


        // Assert
        Assert.AreEqual(383, result);
    }

    [Test]
    public void odd_width_max()
    {
        TextureSize = new Vector2Int(13, 9);
        SetupReservoir();

        // Arrange
        Reservoir.Fill(new ReservoirFiller(ColorFiller, new IndexVolumeFiller(FILL_WIDTH_PART, 0)));

        Reservoir.CopyVolumesToWorkspace();


        // Act
        Vector2Int reductionPosition = new Vector2Int(1, 2);
        Vector2Int reductionSize = new Vector2Int(9, 6);
        // calculate points from reduction info because that is how the function is going to be used
        ShaderRegion sr = new ShaderRegion(
            reductionPosition,
            reductionPosition + new Vector2Int(reductionSize.x - 1, 0),
            reductionPosition + new Vector2Int(0, reductionSize.y - 1),
            reductionPosition + new Vector2Int(reductionSize.x - 1, reductionSize.y - 1));
        float result = Execute(sr, InternalReduceFunction.Max, false);


        // Assert
        Assert.AreEqual(100, result);
    }

    // this was really only written because it was easier to debug the already known scenario
    [Test]
    public void odd_width_add()
    {
        TextureSize = new Vector2Int(13, 9);
        SetupReservoir();

        // Arrange
        Reservoir.Fill(new ReservoirFiller(ColorFiller, new OnesVolumeFiller(FILL_WIDTH_PART, 0)));

        Reservoir.CopyVolumesToWorkspace();


        // Act
        Vector2Int reductionPosition = new Vector2Int(1, 2);
        Vector2Int reductionSize = new Vector2Int(9, 6);
        // calculate points from reduction info because that is how the function is going to be used
        ShaderRegion sr = new ShaderRegion(
            reductionPosition,
            reductionPosition + new Vector2Int(reductionSize.x - 1, 0),
            reductionPosition + new Vector2Int(0, reductionSize.y - 1),
            reductionPosition + new Vector2Int(reductionSize.x - 1, reductionSize.y - 1));
        float result = Execute(sr, InternalReduceFunction.Add, false);


        // Assert
        Assert.AreEqual(reductionSize.x * reductionSize.y, result);
    }

    [Test]
    public void odd_height_add()
    {
        TextureSize = new Vector2Int(16, 24);
        SetupReservoir();

        // Arrange
        Reservoir.Fill(new ReservoirFiller(ColorFiller, new OnesVolumeFiller(FILL_WIDTH_PART, 0)));

        Reservoir.CopyVolumesToWorkspace();


        // Act
        Vector2Int reductionPosition = new Vector2Int(3, 4);
        Vector2Int reductionSize = new Vector2Int(12, 19);
        // calculate points from reduction info because that is how the function is going to be used
        ShaderRegion sr = new ShaderRegion(
            reductionPosition,
            reductionPosition + new Vector2Int(reductionSize.x - 1, 0),
            reductionPosition + new Vector2Int(0, reductionSize.y - 1),
            reductionPosition + new Vector2Int(reductionSize.x - 1, reductionSize.y - 1));
        float result = Execute(sr, InternalReduceFunction.Add, false);


        // Assert
        Assert.AreEqual(reductionSize.x * reductionSize.y, result);
    }
}
