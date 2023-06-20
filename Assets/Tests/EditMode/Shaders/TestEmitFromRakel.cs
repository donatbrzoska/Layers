using System;
using NUnit.Framework;
using UnityEngine;

public class TestEmitFromRakel
{
    private const float EMIT_DISTANCE_MAX = 0.1f;

    Canvas_ Canvas;
    float RakelLength = 4;
    float RakelWidth = 2;
    Rakel Rakel;

    ColorFiller ColorFiller;

    ComputeBuffer RakelEmittedPaint;

    public float Sum(float[] values)
    {
        float res = 0;
        foreach (float e in values) {
            res += e;
        }
        return res;
    }

    [SetUp]
    public void Setup()
    {
        Canvas = new Canvas_(15, 10, new Vector3(0, 0, 0), 1, 0, 0);

        Rakel = new Rakel(RakelLength, RakelWidth, 1, 0.5f, 0);

        ColorFiller = new FlatColorFiller(Color_.CadmiumGreen, ColorSpace.RGB);

        new FileLogger_().OnEnable();
    }

    [TearDown]
    public void Teardown()
    {
        RakelEmittedPaint.Dispose();
        Rakel.Dispose();

        new FileLogger_().OnDisable();
    }

    [Test]
    public void Volume_Unrotated_Untilted()
    {
        // Arrange
        Rakel.Fill(new ReservoirFiller(ColorFiller, new FlatVolumeFiller(1)));
        Rakel.UpdateState(
            new Vector3(-3, -0.5f, -0.5f * EMIT_DISTANCE_MAX), 0, // choose z for max paint emission
            0,
            0);
        ShaderRegion rakelEmitSR = new ShaderRegion(
            Canvas.MapToPixelInRange(Rakel.Info.UpperLeft),
            Canvas.MapToPixelInRange(Rakel.Info.UpperRight),
            Canvas.MapToPixelInRange(Rakel.Info.LowerLeft),
            Canvas.MapToPixelInRange(Rakel.Info.LowerRight),
            1 // not really needed for polygon clipping
        );

        ComputeBuffer rakelMappedInfo = Rakel.TransformToRakelOrigin(rakelEmitSR, Canvas, false);
        Rakel.CalculateReservoirPixel(rakelMappedInfo, rakelEmitSR, false);

        // Act
        RakelEmittedPaint = Rakel.EmitPaint(
            rakelEmitSR,
            Canvas,
            rakelMappedInfo,
            EMIT_DISTANCE_MAX,
            0,
            1,
            0,
            1, // unused
            false);


        // Assert
        Paint[] rakelEmittedPaintData = new Paint[rakelEmitSR.PixelCount];
        RakelEmittedPaint.GetData(rakelEmittedPaintData);
        float[] rakelEmittedVolumes = new float[rakelEmitSR.PixelCount];
        for (int i = 0; i < rakelEmittedVolumes.Length; i++)
        {
            rakelEmittedVolumes[i] = rakelEmittedPaintData[i].Volume;
        }

        //LogUtil.Log(rakelEmittedVolumes, rakelEmitSC.Size.y, false);

        float Q = 0.25f * Paint.UNIT; // quarter
        float H = 0.5f * Paint.UNIT; // half
        float F = Paint.UNIT; // full
        AssertUtil.AssertFloatsAreEqual(
            new float[] { // remember: these arrays are upside down compared to the actual pixels
                0, 0, 0, 0, 0,
                0, Q, H, Q, 0,
                0, H, F, H, 0,
                0, H, F, H, 0,
                0, H, F, H, 0,
                0, Q, H, Q, 0,
                0, 0, 0, 0, 0,
            },
            rakelEmittedVolumes);

        Assert.AreEqual(
            RakelLength * RakelWidth * Paint.UNIT,
            Sum(rakelEmittedVolumes));
    }

    [Test]
    public void Volume_Rotated30_Untilted()
    {
        // Arrange
        Rakel.Fill(new ReservoirFiller(ColorFiller, new FlatVolumeFiller(1)));
        Rakel.UpdateState(
            new Vector3(-3, -0.5f, -0.5f * EMIT_DISTANCE_MAX), 0, // choose z for max paint emission
            30,
            0);
        ShaderRegion rakelEmitSR = new ShaderRegion(
            Canvas.MapToPixelInRange(Rakel.Info.UpperLeft),
            Canvas.MapToPixelInRange(Rakel.Info.UpperRight),
            Canvas.MapToPixelInRange(Rakel.Info.LowerLeft),
            Canvas.MapToPixelInRange(Rakel.Info.LowerRight),
            1 // not really needed for polygon clipping
        );

        ComputeBuffer rakelMappedInfo = Rakel.TransformToRakelOrigin(rakelEmitSR, Canvas, false);
        Rakel.CalculateReservoirPixel(rakelMappedInfo, rakelEmitSR, false);

        // Act
        RakelEmittedPaint = Rakel.EmitPaint(
            rakelEmitSR,
            Canvas,
            rakelMappedInfo,
            EMIT_DISTANCE_MAX,
            0,
            1,
            0,
            1, // unused
            false);


        // Assert
        Paint[] rakelEmittedPaintData = new Paint[rakelEmitSR.PixelCount];
        RakelEmittedPaint.GetData(rakelEmittedPaintData);
        float[] rakelEmittedVolumes = new float[rakelEmitSR.PixelCount];
        for (int i = 0; i < rakelEmittedVolumes.Length; i++)
        {
            rakelEmittedVolumes[i] = rakelEmittedPaintData[i].Volume;
        }

        //LogUtil.Log(rakelEmittedVolumes, rakelEmitSC.Size.y, false);

        AssertUtil.AssertFloatsAreEqual(
            new float[] { // remember: these arrays are upside down compared to the actual pixels
                 0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,
                 0.000f,  0.000f,  0.008f,  0.054f,  0.000f,  0.000f,  0.000f,
                 0.000f,  0.173f,  0.801f,  0.655f,  0.000f,  0.000f,  0.000f,
                 0.000f,  0.116f,  0.961f,  0.997f,  0.235f,  0.000f,  0.000f,
                 0.000f,  0.000f,  0.500f,  1.000f,  0.801f,  0.008f,  0.000f,
                 0.000f,  0.000f,  0.039f,  0.881f,  0.655f,  0.054f,  0.000f,
                 0.000f,  0.000f,  0.000f,  0.062f,  0.000f,  0.000f,  0.000f,
                 0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,
            },
            rakelEmittedVolumes);

        AssertUtil.AssertFloatsEqual(
            RakelLength * RakelWidth * Paint.UNIT,
            Sum(rakelEmittedVolumes));
    }

    [Test]
    public void Volume_Unrotated_Tilted60()
    {
        // Arrange
        Rakel.Fill(new ReservoirFiller(ColorFiller, new FlatVolumeFiller(1)));
        Rakel.UpdateState(
            new Vector3(
                -4,
                -0.5f,
                -0.5f * EMIT_DISTANCE_MAX), 0,
            0,
            60);

        ShaderRegion rakelEmitSR = new ShaderRegion(
            Canvas.MapToPixelInRange(Rakel.Info.UpperLeft),
            Canvas.MapToPixelInRange(Rakel.Info.UpperRight),
            Canvas.MapToPixelInRange(Rakel.Info.LowerLeft),
            Canvas.MapToPixelInRange(Rakel.Info.LowerRight),
            1 // not really needed for polygon clipping
        );

        ComputeBuffer rakelMappedInfo = Rakel.TransformToRakelOrigin(rakelEmitSR, Canvas, false);
        Rakel.CalculateReservoirPixel(rakelMappedInfo, rakelEmitSR, false);

        // Act
        RakelEmittedPaint = Rakel.EmitPaint(
            rakelEmitSR,
            Canvas,
            rakelMappedInfo,
            EMIT_DISTANCE_MAX,
            0,
            1,
            0,
            1, // unused
            false);


        // Assert
        Paint[] rakelEmittedPaintData = new Paint[rakelEmitSR.PixelCount];
        RakelEmittedPaint.GetData(rakelEmittedPaintData);
        float[] rakelEmittedVolumes = new float[rakelEmitSR.PixelCount];
        for (int i = 0; i < rakelEmittedVolumes.Length; i++)
        {
            rakelEmittedVolumes[i] = rakelEmittedPaintData[i].Volume;
        }

        //LogUtil.Log(rakelEmittedVolumes, rakelEmitSC.Size.y, false);

        AssertUtil.AssertFloatsAreEqual(
            new float[] { // remember: these arrays are upside down compared to the actual pixels
                0.000f,  0.000f,  0.000f,  0.000f,
                0.000f,  0.012f,  0.000f,  0.000f,
                0.000f,  0.024f,  0.000f,  0.000f,
                0.000f,  0.024f,  0.000f,  0.000f,
                0.000f,  0.024f,  0.000f,  0.000f,
                0.000f,  0.012f,  0.000f,  0.000f,
                0.000f,  0.000f,  0.000f,  0.000f,
            },
            rakelEmittedVolumes);

        AssertUtil.AssertFloatsEqual(
            0.0958637297f * Paint.UNIT,
            Sum(rakelEmittedVolumes));
    }

    [Test]
    public void Volume_Rotated30_Tilted60()
    {
        // Arrange
        Rakel.Fill(new ReservoirFiller(ColorFiller, new FlatVolumeFiller(1)));
        Rakel.UpdateState(
            new Vector3(
                -3,
                -0.5f,
                -0.1f * EMIT_DISTANCE_MAX), 0,
            30,
            10);

        ShaderRegion rakelEmitSR = new ShaderRegion(
            Canvas.MapToPixelInRange(Rakel.Info.UpperLeft),
            Canvas.MapToPixelInRange(Rakel.Info.UpperRight),
            Canvas.MapToPixelInRange(Rakel.Info.LowerLeft),
            Canvas.MapToPixelInRange(Rakel.Info.LowerRight),
            1 // not really needed for polygon clipping
        );

        ComputeBuffer rakelMappedInfo = Rakel.TransformToRakelOrigin(rakelEmitSR, Canvas, false);
        Rakel.CalculateReservoirPixel(rakelMappedInfo, rakelEmitSR, false);

        // Act
        RakelEmittedPaint = Rakel.EmitPaint(
            rakelEmitSR,
            Canvas,
            rakelMappedInfo,
            EMIT_DISTANCE_MAX,
            0,
            1,
            0,
            1, // unused
            false);


        // Assert
        Paint[] rakelEmittedPaintData = new Paint[rakelEmitSR.PixelCount];
        RakelEmittedPaint.GetData(rakelEmittedPaintData);
        float[] rakelEmittedVolumes = new float[rakelEmitSR.PixelCount];
        for (int i = 0; i < rakelEmittedVolumes.Length; i++)
        {
            rakelEmittedVolumes[i] = rakelEmittedPaintData[i].Volume;
        }

        //LogUtil.Log(rakelEmittedVolumes, rakelEmitSC.Size.y, false);

        AssertUtil.AssertFloatsAreEqual(
            new float[] { // remember: these arrays are upside down compared to the actual pixels
                 0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,
                 0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,
                 0.000f,  0.093f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,
                 0.000f,  0.069f,  0.042f,  0.000f,  0.000f,  0.000f,  0.000f,
                 0.000f,  0.000f,  0.108f,  0.000f,  0.000f,  0.000f,  0.000f,
                 0.000f,  0.000f,  0.016f,  0.403f,  0.000f,  0.000f,  0.000f,
                 0.000f,  0.000f,  0.000f,  0.018f,  0.000f,  0.000f,  0.000f,
                 0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,
            },
            rakelEmittedVolumes);

        AssertUtil.AssertFloatsEqual(
            0.749224544f,
            Sum(rakelEmittedVolumes));
    }
}