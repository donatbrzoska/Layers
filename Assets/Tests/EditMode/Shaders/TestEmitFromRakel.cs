using System;
using NUnit.Framework;
using UnityEngine;

public class TestEmitFromRakel
{
    private float BASE_SINK_MAX = 0;
    private float TILT_SINK_MAX = 0;
    private const int AUTO_Z_ENABLED = 0;
    private const int ZZERO = 0;
    private const float PRESSURE = 0;

    Canvas_ Canvas;
    float RakelLength = 4;
    float RakelWidth = 2;
    Rakel Rakel;

    int MAX_LAYERS = 20;

    ColorFiller ColorFiller;

    PaintGrid RakelEmittedPaint;

    public float Sum(float[] values)
    {
        float res = 0;
        foreach (float e in values)
        {
            res += e;
        }
        return res;
    }

    [SetUp]
    public void Setup()
    {
        Canvas = new Canvas_(15, 10, MAX_LAYERS, 1, new Vector3(0, 0, 0), 1, 0, 0);

        Rakel = new Rakel(RakelLength, RakelWidth, 1, MAX_LAYERS, 1, 0.5f, 0);

        ColorFiller = new FlatColorFiller(Color_.CadmiumGreen, ColorSpace.RGB);

        new FileLogger_().OnEnable();
    }

    [TearDown]
    public void Teardown()
    {
        RakelEmittedPaint.Dispose();
        Rakel.Dispose();
        Canvas.Dispose();

        new FileLogger_().OnDisable();
    }

    [Test]
    public void EmitOneLayer_IncompleteSurfacesTouch_Unrotated_Untilted()
    {
        int INCOMPLETE_SURFACES_TOUCH_VOLUME = 1;
        int APPLIED_LAYERS = 1;

        // Arrange
        Rakel.Fill(new ReservoirFiller(ColorFiller, new FlatVolumeFiller(INCOMPLETE_SURFACES_TOUCH_VOLUME)));
        Rakel.UpdateState(
            new Vector3(-5, -0.5f, APPLIED_LAYERS * -Paint.VOLUME_THICKNESS), BASE_SINK_MAX, TILT_SINK_MAX, AUTO_Z_ENABLED, ZZERO, PRESSURE,
            0,
            0);
        ShaderRegion rakelEmitSR = new ShaderRegion(
            Canvas.MapToPixelInRange(Rakel.Info.UpperLeft),
            Canvas.MapToPixelInRange(Rakel.Info.UpperRight),
            Canvas.MapToPixelInRange(Rakel.Info.LowerLeft),
            Canvas.MapToPixelInRange(Rakel.Info.LowerRight)
        );

        ComputeBuffer rakelMappedInfo = Rakel.CalculateRakelMappedInfo(rakelEmitSR, Canvas);
        Rakel.Reservoir.Duplicate();
        Rakel.CalculateRakelMappedInfo_Part2(Canvas, rakelMappedInfo, rakelEmitSR, 0);


        // Act
        RakelEmittedPaint = Rakel.EmitPaint(
            rakelEmitSR,
            Canvas,
            rakelMappedInfo);


        // Assert
        RakelEmittedPaint.ReadbackInfo();
        float[] rakelEmittedVolumes = RakelEmittedPaint.GetVolumes();

        //LogUtil.Log(rakelEmittedVolumes, rakelEmitSR.Size.y, false);

        float Q = 0.25f * Paint.UNIT;
        float H = 0.5f * Paint.UNIT;
        float F = Paint.UNIT;
        AssertUtil.AssertFloatsAreEqual(
            new float[] { // remember: these arrays are upside down compared to the actual pixels
                0, 0, 0,
                0, F, 0,
                0, F, 0,
                0, F, 0,
                0, 0, 0,
            },
            rakelEmittedVolumes);

        Assert.AreEqual(
            3 * Paint.UNIT,
            Sum(rakelEmittedVolumes));
    }

    [Test]
    public void EmitOneLayer_CompleteSurfacesTouch_Unrotated_Untilted()
    {
        int COMPLETE_SURFACES_TOUCH_VOLUME = 4;
        int LAYERS = 1;

        // Arrange
        Rakel.Fill(new ReservoirFiller(ColorFiller, new FlatVolumeFiller(COMPLETE_SURFACES_TOUCH_VOLUME)));
        Rakel.UpdateState(
            new Vector3(-5, -0.5f, LAYERS * -Paint.VOLUME_THICKNESS), BASE_SINK_MAX, TILT_SINK_MAX, AUTO_Z_ENABLED, ZZERO, PRESSURE,
            0,
            0);
        ShaderRegion rakelEmitSR = new ShaderRegion(
            Canvas.MapToPixelInRange(Rakel.Info.UpperLeft),
            Canvas.MapToPixelInRange(Rakel.Info.UpperRight),
            Canvas.MapToPixelInRange(Rakel.Info.LowerLeft),
            Canvas.MapToPixelInRange(Rakel.Info.LowerRight)
        );

        ComputeBuffer rakelMappedInfo = Rakel.CalculateRakelMappedInfo(rakelEmitSR, Canvas);
        Rakel.Reservoir.Duplicate();
        Rakel.CalculateRakelMappedInfo_Part2(Canvas, rakelMappedInfo, rakelEmitSR, 0);


        // Act
        RakelEmittedPaint = Rakel.EmitPaint(
            rakelEmitSR,
            Canvas,
            rakelMappedInfo);


        // Assert
        RakelEmittedPaint.ReadbackInfo();
        float[] rakelEmittedVolumes = RakelEmittedPaint.GetVolumes();

        //LogUtil.Log(rakelEmittedVolumes, rakelEmitSR.Size.y, false);

        float Q = 0.25f * Paint.UNIT;
        float H = 0.5f * Paint.UNIT;
        float F = Paint.UNIT;
        AssertUtil.AssertFloatsAreEqual(
            new float[] { // remember: these arrays are upside down compared to the actual pixels
                Q, H, Q,
                H, F, H,
                H, F, H,
                H, F, H,
                Q, H, Q,
            },
            rakelEmittedVolumes);

        Assert.AreEqual(
            RakelLength * RakelWidth * Paint.UNIT,
            Sum(rakelEmittedVolumes));
    }

    [Test]
    public void EmitOneLayer_CompleteSurfacesTouch_Rotated30_Untilted()
    {
        int BASICALLY_COMPLETE_SURFACES_TOUCH_VOLUME = 20;
        int LAYERS = 1;

        // Arrange
        Rakel.Fill(new ReservoirFiller(ColorFiller, new FlatVolumeFiller(BASICALLY_COMPLETE_SURFACES_TOUCH_VOLUME)));
        Rakel.UpdateState(
            new Vector3(-5, -0.5f, LAYERS * -Paint.VOLUME_THICKNESS), BASE_SINK_MAX, TILT_SINK_MAX, AUTO_Z_ENABLED, ZZERO, PRESSURE,
            30,
            0);
        ShaderRegion rakelEmitSR = new ShaderRegion(
            Canvas.MapToPixelInRange(Rakel.Info.UpperLeft),
            Canvas.MapToPixelInRange(Rakel.Info.UpperRight),
            Canvas.MapToPixelInRange(Rakel.Info.LowerLeft),
            Canvas.MapToPixelInRange(Rakel.Info.LowerRight)
        );

        ComputeBuffer rakelMappedInfo = Rakel.CalculateRakelMappedInfo(rakelEmitSR, Canvas);
        Rakel.Reservoir.Duplicate();
        Rakel.CalculateRakelMappedInfo_Part2(Canvas, rakelMappedInfo, rakelEmitSR, 0);


        // Act
        RakelEmittedPaint = Rakel.EmitPaint(
            rakelEmitSR,
            Canvas,
            rakelMappedInfo);


        // Assert
        RakelEmittedPaint.ReadbackInfo();
        float[] rakelEmittedVolumes = RakelEmittedPaint.GetVolumes();

        LogUtil.Log(rakelEmittedVolumes, rakelEmitSR.Size.y, false);

        AssertUtil.AssertFloatsAreEqual(
            new float[] { // remember: these arrays are upside down compared to the actual pixels
                0.000f,  0.000f,  0.054f,  0.000f,  0.000f,
                0.173f,  0.801f,  0.655f,  0.000f,  0.000f,
                0.116f,  0.961f,  0.997f,  0.235f,  0.000f,
                0.000f,  0.500f,  1.000f,  0.801f,  0.000f,
                0.000f,  0.000f,  0.881f,  0.655f,  0.054f,
                0.000f,  0.000f,  0.062f,  0.000f,  0.000f,
            },
            rakelEmittedVolumes);

        AssertUtil.AssertFloatsEqual(
            RakelLength * RakelWidth * Paint.UNIT,
            Sum(rakelEmittedVolumes),
            0.1f); // lower precision, because surfaces_touch is really hard to complete with a rotated rakel
    }

    [Test]
    public void EmitTwoLayers_CompleteSurfacesTouch_Unrotated_Untilted()
    {
        int COMPLETE_SURFACES_TOUCH_VOLUME = 8;
        int LAYERS = 2;

        // Arrange
        Rakel.Fill(new ReservoirFiller(ColorFiller, new FlatVolumeFiller(COMPLETE_SURFACES_TOUCH_VOLUME)));
        Rakel.UpdateState(
            new Vector3(-5, -0.5f, LAYERS * -Paint.VOLUME_THICKNESS), BASE_SINK_MAX, TILT_SINK_MAX, AUTO_Z_ENABLED, ZZERO, PRESSURE,
            0,
            0);
        ShaderRegion rakelEmitSR = new ShaderRegion(
            Canvas.MapToPixelInRange(Rakel.Info.UpperLeft),
            Canvas.MapToPixelInRange(Rakel.Info.UpperRight),
            Canvas.MapToPixelInRange(Rakel.Info.LowerLeft),
            Canvas.MapToPixelInRange(Rakel.Info.LowerRight)
        );

        ComputeBuffer rakelMappedInfo = Rakel.CalculateRakelMappedInfo(rakelEmitSR, Canvas);
        Rakel.Reservoir.Duplicate();
        Rakel.CalculateRakelMappedInfo_Part2(Canvas, rakelMappedInfo, rakelEmitSR, 0);


        // Act
        RakelEmittedPaint = Rakel.EmitPaint(
            rakelEmitSR,
            Canvas,
            rakelMappedInfo);


        // Assert
        RakelEmittedPaint.ReadbackInfo();
        float[] rakelEmittedVolumes = RakelEmittedPaint.GetVolumes();

        //LogUtil.Log(rakelEmittedVolumes, rakelEmitSR.Size.y, false);

        float Q = 0.25f * LAYERS * Paint.UNIT;
        float H = 0.5f * LAYERS * Paint.UNIT;
        float F = LAYERS * Paint.UNIT;
        AssertUtil.AssertFloatsAreEqual(
            new float[] { // remember: these arrays are upside down compared to the actual pixels
                Q, H, Q,
                H, F, H,
                H, F, H,
                H, F, H,
                Q, H, Q,
            },
            rakelEmittedVolumes);

        Assert.AreEqual(
            RakelLength * RakelWidth * LAYERS * Paint.UNIT,
            Sum(rakelEmittedVolumes));
    }
}