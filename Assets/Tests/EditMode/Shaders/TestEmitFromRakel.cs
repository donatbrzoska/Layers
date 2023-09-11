using System;
using NUnit.Framework;
using UnityEngine;

public class TestEmitFromRakel
{
    private float BASE_SINK_MAX = 0;
    private float LAYER_SINK_MAX_RATIO = 0;
    private float TILT_SINK_MAX = 0;
    private const bool AUTO_Z_ENABLED = false;
    private const bool ZZERO = false;
    private const bool FINAL_UPDATE_FOR_STROKE = true;
    private const float PRESSURE = 0;

    private float EMIT_DIST_MAX = 0;
    private float EMIT_VOLUME_MIN = 0;

    private const bool TILT_NOISE_ENABLED = false;
    private const float TILT_NOISE_FREQUENCY = 0;
    private const float TILT_NOISE_AMPLITUDE = 0;
    private const float FLOATING_Z_LENGTH = 0;

    private const float CELL_VOLUME = 1;

    private const float FILL_WIDTH_PART = 1;

    Canvas_ Canvas;
    ComputeBuffer RakelMappedInfo;
    float RakelLength = 4;
    float RakelWidth = 2;
    Rakel Rakel;

    int MAX_LAYERS = 20;

    ColorFiller ColorFiller;

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
        Canvas = new Canvas_(15, 10, MAX_LAYERS, CELL_VOLUME, new Vector3(0, 0, 0), 1, 0, 0);

        Rakel = new Rakel(RakelLength, RakelWidth, 1, MAX_LAYERS, CELL_VOLUME);
        RakelMappedInfo = MappedInfo.CreateBuffer(Canvas.Reservoir.Size2D);

        ColorFiller = new FlatColorFiller(Color_.CadmiumGreen, ColorSpace.RGB);

        new FileLogger_().OnEnable();
    }

    [TearDown]
    public void Teardown()
    {
        Rakel.Dispose();
        Canvas.Dispose();

        RakelMappedInfo.Dispose();

        new FileLogger_().OnDisable();
    }

    [Test]
    public void EmitOneLayer_BarelySurfacesTouch_Unrotated_Untilted()
    {
        float BARELY_SURFACES_TOUCH_VOLUME = 1.1f;
        int APPLIED_LAYERS = 1;

        // Arrange
        Rakel.Fill(new ReservoirFiller(ColorFiller, new FlatVolumeFiller(FILL_WIDTH_PART, BARELY_SURFACES_TOUCH_VOLUME)));
        Rakel.NewStroke(TILT_NOISE_ENABLED, TILT_NOISE_FREQUENCY, TILT_NOISE_AMPLITUDE, FLOATING_Z_LENGTH);
        Rakel.UpdateState(
            new Vector3(-5, -0.5f, APPLIED_LAYERS * -Paint.VOLUME_THICKNESS), BASE_SINK_MAX, LAYER_SINK_MAX_RATIO, TILT_SINK_MAX, AUTO_Z_ENABLED, ZZERO, FINAL_UPDATE_FOR_STROKE, PRESSURE,
            0,
            0);
        ShaderRegion rakelEmitSR = new ShaderRegion(
            Canvas.MapToPixelInRange(Rakel.Info.UpperLeft),
            Canvas.MapToPixelInRange(Rakel.Info.UpperRight),
            Canvas.MapToPixelInRange(Rakel.Info.LowerLeft),
            Canvas.MapToPixelInRange(Rakel.Info.LowerRight)
        );

        Rakel.CalculateRakelMappedInfo(rakelEmitSR, Canvas, RakelMappedInfo);
        Rakel.Reservoir.DoImprintCopy();
        Rakel.CalculateRakelMappedInfo_Part2(Canvas, RakelMappedInfo, rakelEmitSR, EMIT_DIST_MAX, EMIT_VOLUME_MIN);


        // Act
        Rakel.EmitPaint(
            Canvas,
            RakelMappedInfo,
            rakelEmitSR);


        // Assert
        Canvas.Reservoir.PaintGridInputBuffer.ReadbackInfo();
        float[] rakelEmittedVolumes = Canvas.Reservoir.PaintGridInputBuffer.GetVolumes(rakelEmitSR);

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
    public void EmitOneLayer_CompleteSurfacesTouch_Unrotated_Untilted()
    {
        int COMPLETE_SURFACES_TOUCH_VOLUME = 4;
        int LAYERS = 1;

        // Arrange
        Rakel.Fill(new ReservoirFiller(ColorFiller, new FlatVolumeFiller(FILL_WIDTH_PART, COMPLETE_SURFACES_TOUCH_VOLUME)));
        Rakel.NewStroke(TILT_NOISE_ENABLED, TILT_NOISE_FREQUENCY, TILT_NOISE_AMPLITUDE, FLOATING_Z_LENGTH);
        Rakel.UpdateState(
            new Vector3(-5, -0.5f, LAYERS * -Paint.VOLUME_THICKNESS), BASE_SINK_MAX, LAYER_SINK_MAX_RATIO, TILT_SINK_MAX, AUTO_Z_ENABLED, ZZERO, FINAL_UPDATE_FOR_STROKE, PRESSURE,
            0,
            0);
        ShaderRegion rakelEmitSR = new ShaderRegion(
            Canvas.MapToPixelInRange(Rakel.Info.UpperLeft),
            Canvas.MapToPixelInRange(Rakel.Info.UpperRight),
            Canvas.MapToPixelInRange(Rakel.Info.LowerLeft),
            Canvas.MapToPixelInRange(Rakel.Info.LowerRight)
        );

        Rakel.CalculateRakelMappedInfo(rakelEmitSR, Canvas, RakelMappedInfo);
        Rakel.Reservoir.DoImprintCopy();
        Rakel.CalculateRakelMappedInfo_Part2(Canvas, RakelMappedInfo, rakelEmitSR, EMIT_DIST_MAX, EMIT_VOLUME_MIN);


        // Act
        Rakel.EmitPaint(
            Canvas,
            RakelMappedInfo,
            rakelEmitSR);


        // Assert
        Canvas.Reservoir.PaintGridInputBuffer.ReadbackInfo();
        float[] rakelEmittedVolumes = Canvas.Reservoir.PaintGridInputBuffer.GetVolumes(rakelEmitSR);

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
        Rakel.Fill(new ReservoirFiller(ColorFiller, new FlatVolumeFiller(FILL_WIDTH_PART, BASICALLY_COMPLETE_SURFACES_TOUCH_VOLUME)));
        Rakel.NewStroke(TILT_NOISE_ENABLED, TILT_NOISE_FREQUENCY, TILT_NOISE_AMPLITUDE, FLOATING_Z_LENGTH);
        Rakel.UpdateState(
            new Vector3(-5, -0.5f, LAYERS * -Paint.VOLUME_THICKNESS), BASE_SINK_MAX, LAYER_SINK_MAX_RATIO, TILT_SINK_MAX, AUTO_Z_ENABLED, ZZERO, FINAL_UPDATE_FOR_STROKE, PRESSURE,
            30,
            0);
        ShaderRegion rakelEmitSR = new ShaderRegion(
            Canvas.MapToPixelInRange(Rakel.Info.UpperLeft),
            Canvas.MapToPixelInRange(Rakel.Info.UpperRight),
            Canvas.MapToPixelInRange(Rakel.Info.LowerLeft),
            Canvas.MapToPixelInRange(Rakel.Info.LowerRight)
        );

        Rakel.CalculateRakelMappedInfo(rakelEmitSR, Canvas, RakelMappedInfo);
        Rakel.Reservoir.DoImprintCopy();
        Rakel.CalculateRakelMappedInfo_Part2(Canvas, RakelMappedInfo, rakelEmitSR, EMIT_DIST_MAX, EMIT_VOLUME_MIN);


        // Act
        Rakel.EmitPaint(
            Canvas,
            RakelMappedInfo,
            rakelEmitSR);


        // Assert
        Canvas.Reservoir.PaintGridInputBuffer.ReadbackInfo();
        float[] rakelEmittedVolumes = Canvas.Reservoir.PaintGridInputBuffer.GetVolumes(rakelEmitSR);

        LogUtil.Log(rakelEmittedVolumes, rakelEmitSR.Size.y, false);

        AssertUtil.AssertFloatsAreEqual(
            new float[] { // remember: these arrays are upside down compared to the actual pixels
                0.000f,  0.000f,  0.054f,  0.000f,  0.000f,
                0.173f,  0.801f,  0.655f,  0.000f,  0.000f,
                0.116f,  0.961f,  0.997f,  0.235f,  0.000f,
                0.000f,  0.500f,  1.000f,  0.801f,  0.000f,
                0.000f,  0.039f,  0.881f,  0.655f,  0.054f,
                0.000f,  0.000f,  0.062f,  0.000f,  0.000f,
            },
            rakelEmittedVolumes);

        AssertUtil.AssertFloatsEqual(
            RakelLength * RakelWidth * Paint.UNIT,
            //Sum(rakelEmittedVolumes));
            Sum(rakelEmittedVolumes),
            // Lower precision, because surfaces_touch is really hard to complete with a rotated rakel.
            // This is because at the border, total overlap would be 0.5 for example and then paint_thickness_rakel
            // volume will be less than what is necessary for surfaces touch
            0.1f);
    }

    [Test]
    public void EmitTwoLayers_CompleteSurfacesTouch_Unrotated_Untilted()
    {
        int COMPLETE_SURFACES_TOUCH_VOLUME = 8;
        int LAYERS = 2;

        // Arrange
        Rakel.Fill(new ReservoirFiller(ColorFiller, new FlatVolumeFiller(FILL_WIDTH_PART, COMPLETE_SURFACES_TOUCH_VOLUME)));
        Rakel.NewStroke(TILT_NOISE_ENABLED, TILT_NOISE_FREQUENCY, TILT_NOISE_AMPLITUDE, FLOATING_Z_LENGTH);
        Rakel.UpdateState(
            new Vector3(-5, -0.5f, LAYERS * -Paint.VOLUME_THICKNESS), BASE_SINK_MAX, LAYER_SINK_MAX_RATIO, TILT_SINK_MAX, AUTO_Z_ENABLED, ZZERO, FINAL_UPDATE_FOR_STROKE, PRESSURE,
            0,
            0);
        ShaderRegion rakelEmitSR = new ShaderRegion(
            Canvas.MapToPixelInRange(Rakel.Info.UpperLeft),
            Canvas.MapToPixelInRange(Rakel.Info.UpperRight),
            Canvas.MapToPixelInRange(Rakel.Info.LowerLeft),
            Canvas.MapToPixelInRange(Rakel.Info.LowerRight)
        );

        Rakel.CalculateRakelMappedInfo(rakelEmitSR, Canvas, RakelMappedInfo);
        Rakel.Reservoir.DoImprintCopy();
        Rakel.CalculateRakelMappedInfo_Part2(Canvas, RakelMappedInfo, rakelEmitSR, EMIT_DIST_MAX, EMIT_VOLUME_MIN);


        // Act
        Rakel.EmitPaint(
            Canvas,
            RakelMappedInfo,
            rakelEmitSR);


        // Assert
        Canvas.Reservoir.PaintGridInputBuffer.ReadbackInfo();
        float[] rakelEmittedVolumes = Canvas.Reservoir.PaintGridInputBuffer.GetVolumes(rakelEmitSR);

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