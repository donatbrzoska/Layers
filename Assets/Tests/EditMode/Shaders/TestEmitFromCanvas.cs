using NUnit.Framework;
using UnityEngine;

public class TestEmitFromCanvas
{
    private const int AUTO_Z_ENABLED = 0;
    private const int ZZERO = 0;
    private const float PRESSURE = 0;

    int Resolution;

    Rakel Rakel;
    float RakelLength;
    float RakelWidth;

    Canvas_ Canvas;
    float CanvasWidth;
    float CanvasHeight;

    PaintGrid CanvasEmittedPaint;

    int MAX_LAYERS = 20;

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
        Resolution = 1;
        RakelLength = 4;
        RakelWidth = 2;
        CanvasWidth = 15;
        CanvasHeight = 10;

        CreateRakel();
        CreateCanvas();

        new FileLogger_().OnEnable();
    }

    private void CreateRakel()
    {
        Rakel?.Dispose();
        Rakel = new Rakel(RakelLength, RakelWidth, Resolution, MAX_LAYERS, 1, 0.5f, 0);
    }

    private void CreateCanvas()
    {
        Canvas?.Dispose();
        Canvas = new Canvas_(CanvasWidth, CanvasHeight, MAX_LAYERS, 1, new Vector3(0, 0, 0), Resolution, 0.015f, 0);
    }

    private int Pixels(float length, float width)
    {
        return (int)length * Resolution * (int)width * Resolution;
    }

    [TearDown]
    public void Teardown()
    {
        CanvasEmittedPaint.Dispose();
        Canvas.Dispose();
        Rakel.Dispose();

        new FileLogger_().OnDisable();
    }

    // TODO Parametrize and include MIN_VOLUME_TO_STAY
    [Test]
    public void EmitOneLayer_Unrotated_Untilted()
    {
        // Arrange
        ShaderRegion canvasEmitSR = Rakel.Reservoir.GetFullShaderRegion();
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen, ColorSpace.RGB), new FlatVolumeFiller(1)));
        Canvas.Reservoir.Duplicate(false);

        Rakel.UpdateState(
            new Vector3(-5, 0.5f, 0), AUTO_Z_ENABLED, ZZERO, PRESSURE,
            0, 0);

        // Act
        CanvasEmittedPaint = Canvas.EmitPaint(
            Rakel,
            canvasEmitSR,
            0);


        // Assert
        CanvasEmittedPaint.ReadbackInfo();
        float[] canvasEmittedVolumes = CanvasEmittedPaint.GetVolumes();

        //LogUtil.Log(canvasEmittedVolumes, canvasEmitSR.Size.y, false);

        AssertUtil.AssertFloatsAreEqual(
            new float[] { // remember: these arrays are upside down compared to the actual pixels
                0.900f,  0.900f,
                0.900f,  0.900f,
                0.900f,  0.900f,
                0.900f,  0.900f,
            },
            canvasEmittedVolumes);

        AssertUtil.AssertFloatsEqual(
            0.9f * Pixels(RakelLength, RakelWidth) * Paint.UNIT,
            Sum(canvasEmittedVolumes));
    }

    [Test]
    public void EmitOneOfTwoLayers_Unrotated_Untilted()
    {
        // Arrange
        ShaderRegion canvasEmitSR = Rakel.Reservoir.GetFullShaderRegion();
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen, ColorSpace.RGB), new FlatVolumeFiller(2)));
        Canvas.Reservoir.Duplicate(false);

        Rakel.UpdateState(
            new Vector3(-5, 0.5f, -Paint.VOLUME_THICKNESS), AUTO_Z_ENABLED, ZZERO, PRESSURE,
            0, 0);

        // Act
        CanvasEmittedPaint = Canvas.EmitPaint(
            Rakel,
            canvasEmitSR,
            0);


        // Assert

        // 1. Check emitted paint
        CanvasEmittedPaint.ReadbackInfo();
        float[] canvasEmittedVolumes = CanvasEmittedPaint.GetVolumes();

        //LogUtil.Log(canvasEmittedVolumes, canvasEmitSR.Size.y, false);

        float F = Paint.UNIT;
        AssertUtil.AssertFloatsAreEqual(
            new float[] { // remember: these arrays are upside down compared to the actual pixels
                F,  F,
                F,  F,
                F,  F,
                F,  F,
            },
            canvasEmittedVolumes);

        AssertUtil.AssertFloatsEqual(
            Pixels(RakelLength, RakelWidth) * Paint.UNIT,
            Sum(canvasEmittedVolumes));

        // 2. Check paint on canvas
        Canvas.Reservoir.PaintGrid.ReadbackInfo();
        float[] canvasVolumes = Canvas.Reservoir.PaintGrid.GetVolumes();
        AssertUtil.AssertFloatsEqual(
            2 * Pixels(CanvasWidth, CanvasHeight) * Paint.UNIT - Sum(canvasEmittedVolumes),
            Sum(canvasVolumes));
    }

    [Test] //?
    public void EmitFourOfEightLayers_Unrotated_Untilted()
    {
        RakelWidth = 6;
        CreateRakel();
        int INIT_LAYERS = 8;
        int PICKED_UP_LAYERS = 4;

        // Arrange
        ShaderRegion canvasEmitSR = Rakel.Reservoir.GetFullShaderRegion();
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen, ColorSpace.RGB), new FlatVolumeFiller(INIT_LAYERS)));
        Canvas.Reservoir.Duplicate(false);

        Rakel.UpdateState(
            new Vector3(-5, 0.5f, (INIT_LAYERS - PICKED_UP_LAYERS) * -Paint.VOLUME_THICKNESS), AUTO_Z_ENABLED, ZZERO, PRESSURE,
            0, 0);

        // Act
        CanvasEmittedPaint = Canvas.EmitPaint(
            Rakel,
            canvasEmitSR,
            0);


        // Assert

        // 1. Check emitted paint
        CanvasEmittedPaint.ReadbackInfo();
        float[] canvasEmittedVolumes = CanvasEmittedPaint.GetVolumes();

        //LogUtil.Log(canvasEmittedVolumes, canvasEmitSR.Size.y, false);

        float F = PICKED_UP_LAYERS * Paint.UNIT;
        AssertUtil.AssertFloatsAreEqual(
            new float[] { // remember: these arrays are upside down compared to the actual pixels
                F,  F,  F,  F,  F,  F,
                F,  F,  F,  F,  F,  F,
                F,  F,  F,  F,  F,  F,
                F,  F,  F,  F,  F,  F,
            },
            canvasEmittedVolumes);

        AssertUtil.AssertFloatsEqual(
            PICKED_UP_LAYERS * Pixels(RakelLength, RakelWidth) * Paint.UNIT,
            Sum(canvasEmittedVolumes));

        // 2. Check paint on canvas
        Canvas.Reservoir.PaintGrid.ReadbackInfo();
        float[] canvasVolumes = Canvas.Reservoir.PaintGrid.GetVolumes();

        //LogUtil.Log(canvasVolumes, Canvas.TextureSize.y, false);

        float sumCanvasVolumesBefore = INIT_LAYERS * Pixels(CanvasWidth, CanvasHeight) * Paint.UNIT;
        AssertUtil.AssertFloatsEqual(
            sumCanvasVolumesBefore - Sum(canvasEmittedVolumes),
            Sum(canvasVolumes));

        // 3. Check paint grid state
        int[] pgSizes = Canvas.Reservoir.PaintGrid.GetSizes();

        //LogUtil.Log(pgSizes, Canvas.TextureSize.y, false);

        Assert.AreEqual(
            new int[]
            {
                8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      4,      4,      4,      4,      4,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      4,      4,      4,      4,      4,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      4,      4,      4,      4,      4,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
            },
            pgSizes);
    }

    [Test] //?
    public void EmitFourOfEightLayers_HigherResolution_Unrotated_Untilted()
    {
        Resolution = 2;
        RakelWidth = 6;
        CreateRakel();
        CreateCanvas();
        int INIT_LAYERS = 8;
        int PICKED_UP_LAYERS = 4;

        // Arrange
        ShaderRegion canvasEmitSR = Rakel.Reservoir.GetFullShaderRegion();
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen, ColorSpace.RGB), new FlatVolumeFiller(INIT_LAYERS)));
        Canvas.Reservoir.Duplicate(false);

        Rakel.UpdateState(
            new Vector3(-5, 0.5f, (INIT_LAYERS - PICKED_UP_LAYERS) * -Paint.VOLUME_THICKNESS), AUTO_Z_ENABLED, ZZERO, PRESSURE,
            0, 0);

        // Act
        CanvasEmittedPaint = Canvas.EmitPaint(
            Rakel,
            canvasEmitSR,
            0);


        // Assert

        // 1. Check emitted paint
        CanvasEmittedPaint.ReadbackInfo();
        float[] canvasEmittedVolumes = CanvasEmittedPaint.GetVolumes();

        //LogUtil.Log(canvasEmittedVolumes, canvasEmitSR.Size.y, false);

        float F = PICKED_UP_LAYERS * Paint.UNIT;
        AssertUtil.AssertFloatsAreEqual(
            new float[] { // remember: these arrays are upside down compared to the actual pixels
                F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,
                F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,
                F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,
                F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,
                F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,
                F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,
                F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,
                F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,  F,
            },
            canvasEmittedVolumes);

        AssertUtil.AssertFloatsEqual(
            PICKED_UP_LAYERS * Pixels(RakelLength, RakelWidth) * Paint.UNIT,
            Sum(canvasEmittedVolumes),
            0.01f);

        // 2. Check paint on canvas
        Canvas.Reservoir.PaintGrid.ReadbackInfo();
        float[] canvasVolumes = Canvas.Reservoir.PaintGrid.GetVolumes();

        //LogUtil.Log(canvasVolumes, Canvas.TextureSize.y, false);

        float sumCanvasVolumesBefore = INIT_LAYERS * Pixels(CanvasWidth, CanvasHeight) * Paint.UNIT;
        AssertUtil.AssertFloatsEqual(
            sumCanvasVolumesBefore - Sum(canvasEmittedVolumes),
            Sum(canvasVolumes),
            0.01f);

        // 3. Check paint grid state
        int[] pgSizes = Canvas.Reservoir.PaintGrid.GetSizes();

        LogUtil.Log(pgSizes, Canvas.TextureSize.y, false);

        Assert.AreEqual(
            new int[]
            {
                8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      4,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
                8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,      8,
            },
            pgSizes);
    }

    [Test]
    public void EmitOneOfTwoLayers_Rotated30_Untilted()
    {
        // Arrange
        ShaderRegion canvasEmitSR = Rakel.Reservoir.GetFullShaderRegion();
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen, ColorSpace.RGB), new FlatVolumeFiller(2)));
        Canvas.Reservoir.Duplicate(false);

        Rakel.UpdateState(
            new Vector3(-5, 0.5f, -Paint.VOLUME_THICKNESS), AUTO_Z_ENABLED, ZZERO, PRESSURE,
            30, 0);

        // Act
        CanvasEmittedPaint = Canvas.EmitPaint(
            Rakel,
            canvasEmitSR,
            0);


        // Assert
        CanvasEmittedPaint.ReadbackInfo();
        float[] canvasEmittedVolumes = CanvasEmittedPaint.GetVolumes();

        //LogUtil.Log(canvasEmittedVolumes, canvasEmitSR.Size.y, false);

        float F = Paint.UNIT;
        AssertUtil.AssertFloatsAreEqual(
            new float[] { // remember: these arrays are upside down compared to the actual pixels
                F,  F,
                F,  F,
                F,  F,
                F,  F,
            },
            canvasEmittedVolumes);

        AssertUtil.AssertFloatsEqual(
            Pixels(RakelLength, RakelWidth) * Paint.UNIT,
            Sum(canvasEmittedVolumes));
    }
}
