using NUnit.Framework;
using UnityEngine;

public class TestEmitFromCanvas
{
    private float BASE_SINK_MAX = 0;
    private float LAYER_SINK_MAX_RATIO = 0;
    private float TILT_SINK_MAX = 0;
    private const bool AUTO_Z_ENABLED = false;
    private const bool ZZERO = false;
    private const bool FINAL_UPDATE_FOR_STROKE = true;
    private const float PRESSURE = 0;

    private float PICKUP_DIST_MAX = 0;
    private float PICKUP_VOLUME_MIN = 0;

    private const bool TILT_NOISE_ENABLED = false;
    private const float TILT_NOISE_FREQUENCY = 0;
    private const float TILT_NOISE_AMPLITUDE = 0;
    private const float FLOATING_Z_LENGTH = 0;

    private const bool CSB_ENABLED = false;
    private const bool CSB_DELETE = false;
    private const bool PAINT_DOES_PICKUP = false;

    private const float CELL_VOLUME = 1;

    private const float FILL_WIDTH_PART = 1;

    int Resolution;

    Rakel Rakel;
    float RakelLength;
    float RakelWidth;
    ComputeBuffer CanvasMappedInfo;

    Canvas_ Canvas;
    float CanvasWidth;
    float CanvasHeight;

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
        CreateCanvasMappedInfo();

        new FileLogger_().OnEnable();
    }

    private void CreateRakel()
    {
        Rakel?.Dispose();
        Rakel = new Rakel(RakelLength, RakelWidth, Resolution, MAX_LAYERS, CELL_VOLUME);
    }

    private void CreateCanvas()
    {
        Canvas?.Dispose();
        Canvas = new Canvas_(CanvasWidth, CanvasHeight, MAX_LAYERS, CELL_VOLUME, new Vector3(0, 0, 0), Resolution, 0.015f, 0);
    }

    private void CreateCanvasMappedInfo()
    {
        CanvasMappedInfo?.Dispose();
        CanvasMappedInfo = MappedInfo.CreateBuffer(Rakel.Reservoir.Size2D);
    }

    private int Pixels(float length, float width)
    {
        return (int)length * Resolution * (int)width * Resolution;
    }

    [TearDown]
    public void Teardown()
    {
        Canvas.Dispose();
        Rakel.Dispose();
        CanvasMappedInfo.Dispose();

        new FileLogger_().OnDisable();
    }

    // TODO Parametrize and include MIN_VOLUME_TO_STAY
    [Test]
    public void EmitOneLayer_Unrotated_Untilted()
    {
        // Arrange
        ShaderRegion canvasEmitSR = Rakel.Reservoir.GetFullShaderRegion();
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen, ColorSpace.RGB), new FlatVolumeFiller(FILL_WIDTH_PART, 1)));
        Canvas.Reservoir.DoImprintCopy(false);

        Rakel.NewStroke(TILT_NOISE_ENABLED, TILT_NOISE_FREQUENCY, TILT_NOISE_AMPLITUDE, FLOATING_Z_LENGTH);
        Rakel.UpdateState(
            new Vector3(-5, 0.5f, 0), BASE_SINK_MAX, LAYER_SINK_MAX_RATIO, TILT_SINK_MAX, AUTO_Z_ENABLED, ZZERO, FINAL_UPDATE_FOR_STROKE, PRESSURE,
            0, 0);

        // Act
        Canvas.EmitPaint(
            Rakel,
            CanvasMappedInfo,
            canvasEmitSR,
            PICKUP_DIST_MAX, PICKUP_VOLUME_MIN,
            Canvas.Reservoir.GetFullShaderRegion(),
            CSB_ENABLED, CSB_DELETE, PAINT_DOES_PICKUP);


        // Assert
        Rakel.Reservoir.PaintGridInputBuffer.ReadbackInfo();
        float[] canvasEmittedVolumes = Rakel.Reservoir.PaintGridInputBuffer.GetVolumes(canvasEmitSR);

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
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen, ColorSpace.RGB), new FlatVolumeFiller(FILL_WIDTH_PART, 2)));
        Canvas.Reservoir.DoImprintCopy(false);

        Rakel.NewStroke(TILT_NOISE_ENABLED, TILT_NOISE_FREQUENCY, TILT_NOISE_AMPLITUDE, FLOATING_Z_LENGTH);
        Rakel.UpdateState(
            new Vector3(-5, 0.5f, -Paint.VOLUME_THICKNESS), BASE_SINK_MAX, LAYER_SINK_MAX_RATIO, TILT_SINK_MAX, AUTO_Z_ENABLED, ZZERO, FINAL_UPDATE_FOR_STROKE, PRESSURE,
            0, 0);

        // Act
        Canvas.EmitPaint(
            Rakel,
            CanvasMappedInfo,
            canvasEmitSR,
            PICKUP_DIST_MAX, PICKUP_VOLUME_MIN,
            Canvas.Reservoir.GetFullShaderRegion(),
            CSB_ENABLED, CSB_DELETE, PAINT_DOES_PICKUP);


        // Assert

        // 1. Check emitted paint
        Rakel.Reservoir.PaintGridInputBuffer.ReadbackInfo();
        float[] canvasEmittedVolumes = Rakel.Reservoir.PaintGridInputBuffer.GetVolumes(canvasEmitSR);

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
        float[] canvasVolumes = Canvas.Reservoir.PaintGrid.GetVolumes(Canvas.Reservoir.GetFullShaderRegion());
        AssertUtil.AssertFloatsEqual(
            2 * Pixels(CanvasWidth, CanvasHeight) * Paint.UNIT - Sum(canvasEmittedVolumes),
            Sum(canvasVolumes));
    }

    [Test] //?
    public void EmitFourOfEightLayers_Unrotated_Untilted()
    {
        RakelWidth = 6;
        CreateRakel();
        CreateCanvasMappedInfo();
        int INIT_LAYERS = 8;
        int PICKED_UP_LAYERS = 4;

        // Arrange
        ShaderRegion canvasEmitSR = Rakel.Reservoir.GetFullShaderRegion();
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen, ColorSpace.RGB), new FlatVolumeFiller(FILL_WIDTH_PART, INIT_LAYERS)));
        Canvas.Reservoir.DoImprintCopy(false);

        Rakel.NewStroke(TILT_NOISE_ENABLED, TILT_NOISE_FREQUENCY, TILT_NOISE_AMPLITUDE, FLOATING_Z_LENGTH);
        Rakel.UpdateState(
            new Vector3(-5, 0.5f, (INIT_LAYERS - PICKED_UP_LAYERS) * -Paint.VOLUME_THICKNESS), BASE_SINK_MAX, LAYER_SINK_MAX_RATIO, TILT_SINK_MAX, AUTO_Z_ENABLED, ZZERO, FINAL_UPDATE_FOR_STROKE, PRESSURE,
            0, 0);

        // Act
        Canvas.EmitPaint(
            Rakel,
            CanvasMappedInfo,
            canvasEmitSR,
            PICKUP_DIST_MAX, PICKUP_VOLUME_MIN,
            Canvas.Reservoir.GetFullShaderRegion(),
            CSB_ENABLED, CSB_DELETE, PAINT_DOES_PICKUP);


        // Assert

        // 1. Check emitted paint
        Rakel.Reservoir.PaintGridInputBuffer.ReadbackInfo();
        float[] canvasEmittedVolumes = Rakel.Reservoir.PaintGridInputBuffer.GetVolumes(canvasEmitSR);

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
        float[] canvasVolumes = Canvas.Reservoir.PaintGrid.GetVolumes(Canvas.Reservoir.GetFullShaderRegion());

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
        CreateCanvasMappedInfo();
        int INIT_LAYERS = 8;
        int PICKED_UP_LAYERS = 4;

        // Arrange
        ShaderRegion canvasEmitSR = Rakel.Reservoir.GetFullShaderRegion();
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen, ColorSpace.RGB), new FlatVolumeFiller(FILL_WIDTH_PART, INIT_LAYERS)));
        Canvas.Reservoir.DoImprintCopy(false);

        Rakel.NewStroke(TILT_NOISE_ENABLED, TILT_NOISE_FREQUENCY, TILT_NOISE_AMPLITUDE, FLOATING_Z_LENGTH);
        Rakel.UpdateState(
            new Vector3(-5, 0.5f, (INIT_LAYERS - PICKED_UP_LAYERS) * -Paint.VOLUME_THICKNESS), BASE_SINK_MAX, LAYER_SINK_MAX_RATIO, TILT_SINK_MAX, AUTO_Z_ENABLED, ZZERO, FINAL_UPDATE_FOR_STROKE, PRESSURE,
            0, 0);

        // Act
        Canvas.EmitPaint(
            Rakel,
            CanvasMappedInfo,
            canvasEmitSR,
            PICKUP_DIST_MAX, PICKUP_VOLUME_MIN,
            Canvas.Reservoir.GetFullShaderRegion(),
            CSB_ENABLED, CSB_DELETE, PAINT_DOES_PICKUP);


        // Assert

        // 1. Check emitted paint
        Rakel.Reservoir.PaintGridInputBuffer.ReadbackInfo();
        float[] canvasEmittedVolumes = Rakel.Reservoir.PaintGridInputBuffer.GetVolumes(canvasEmitSR);

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
        float[] canvasVolumes = Canvas.Reservoir.PaintGrid.GetVolumes(Canvas.Reservoir.GetFullShaderRegion());

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
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen, ColorSpace.RGB), new FlatVolumeFiller(FILL_WIDTH_PART, 2)));
        Canvas.Reservoir.DoImprintCopy(false);

        Rakel.NewStroke(TILT_NOISE_ENABLED, TILT_NOISE_FREQUENCY, TILT_NOISE_AMPLITUDE, FLOATING_Z_LENGTH);
        Rakel.UpdateState(
            new Vector3(-5, 0.5f, -Paint.VOLUME_THICKNESS), BASE_SINK_MAX, LAYER_SINK_MAX_RATIO, TILT_SINK_MAX, AUTO_Z_ENABLED, ZZERO, FINAL_UPDATE_FOR_STROKE, PRESSURE,
            30, 0);

        // Act
        Canvas.EmitPaint(
            Rakel,
            CanvasMappedInfo,
            canvasEmitSR,
            PICKUP_DIST_MAX, PICKUP_VOLUME_MIN,
            Canvas.Reservoir.GetFullShaderRegion(),
            CSB_ENABLED, CSB_DELETE, PAINT_DOES_PICKUP);


        // Assert
        Rakel.Reservoir.PaintGridInputBuffer.ReadbackInfo();
        float[] canvasEmittedVolumes = Rakel.Reservoir.PaintGridInputBuffer.GetVolumes(canvasEmitSR);

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
