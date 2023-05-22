using NUnit.Framework;
using UnityEngine;

public class TestEmitFromCanvasShader
{
    RakelConfiguration RakelConfig;
    ShaderRegionFactory ShaderRegionFactory;
    Rakel Rakel;
    Canvas_ Canvas;

    ComputeBuffer CanvasEmittedPaint;

    public int Sum(int[] values)
    {
        int res = 0;
        foreach (int e in values)
        {
            res += e;
        }
        return res;
    }

    [SetUp]
    public void Setup()
    {
        RakelConfig = new RakelConfiguration();
        RakelConfig.Length = 4;
        RakelConfig.Width = 2;
        RakelConfig.Resolution = 1;
        ShaderRegionFactory = new ShaderRegionFactory(new Vector2Int(32, 1));
        ComputeShaderEngine cse = new ComputeShaderEngine(false);

        Rakel = new Rakel(RakelConfig, ShaderRegionFactory, cse);

        Canvas = new Canvas_(1, ShaderRegionFactory, cse);
    }

    [TearDown]
    public void Teardown()
    {
        CanvasEmittedPaint.Dispose();
        Canvas.Dispose();
        Rakel.Dispose();
    }

    [Test]
    public void Volume_Unrotated_Untilted()
    {
        // Arrange
        ShaderRegion canvasEmitSR = Rakel.ApplicationReservoir.GetShaderRegion();
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen), new FlatVolumeFiller(1)));
        Canvas.Reservoir.Duplicate(
            100, // unused
            5, // unused
            false);

        Rakel.UpdateState(new Vector3(-6, -0.5f, 0), 0, 0);


        // Act
        CanvasEmittedPaint = Canvas.EmitPaint(
            Rakel,
            canvasEmitSR,
            TransferMapMode.PolygonClipping,
            3, // unused
            1,
            false);


        // Assert
        Paint[] canvasEmittedPaintData = new Paint[canvasEmitSR.PixelCount];
        CanvasEmittedPaint.GetData(canvasEmittedPaintData);
        int[] canvasEmittedVolumes = new int[canvasEmitSR.PixelCount];
        for (int i = 0; i < canvasEmittedVolumes.Length; i++)
        {
            canvasEmittedVolumes[i] = canvasEmittedPaintData[i].Volume;
        }

        int Q = (int)(0.25f * Paint.UNIT); // quarter
        int H = (int)(0.5f * Paint.UNIT); // half
        int F = Paint.UNIT; // full
        Assert.AreEqual(
            new int[] { // remember: these arrays are upside down compared to the actual pixels
                H, F,
                H, F,
                H, F,
                H, F,
            },
            canvasEmittedVolumes);

        Assert.AreEqual(
            RakelConfig.Length * RakelConfig.Width * Paint.UNIT * 0.75f,
            Sum(canvasEmittedVolumes));
    }

    [Test]
    public void Volume_Rotated30_Untilted()
    {
        // Arrange
        ShaderRegion canvasEmitSR = Rakel.ApplicationReservoir.GetShaderRegion();
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen), new FlatVolumeFiller(1)));
        Canvas.Reservoir.Duplicate(
            100, // unused
            5, // unused
            false);

        Rakel.UpdateState(new Vector3(-6, -0.5f, 0), 30, 0);


        // Act
        CanvasEmittedPaint = Canvas.EmitPaint(
            Rakel,
            canvasEmitSR,
            TransferMapMode.PolygonClipping,
            3, // unused
            1,
            false);


        // Assert
        Paint[] canvasEmittedPaintData = new Paint[canvasEmitSR.PixelCount];
        CanvasEmittedPaint.GetData(canvasEmittedPaintData);
        int[] canvasEmittedVolumes = new int[canvasEmitSR.PixelCount];
        for (int i = 0; i < canvasEmittedVolumes.Length; i++)
        {
            canvasEmittedVolumes[i] = canvasEmittedPaintData[i].Volume;
        }

        Assert.AreEqual(
            new int[] { // remember: these arrays are upside down compared to the actual pixels
                2071,  84527,
                44337, 99998,
                93781, 99999,
                99998, 99999,
            },
            canvasEmittedVolumes);

        Assert.AreEqual(
            624710,
            Sum(canvasEmittedVolumes));
    }
}
