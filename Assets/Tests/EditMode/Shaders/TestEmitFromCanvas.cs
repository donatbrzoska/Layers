using NUnit.Framework;
using UnityEngine;

public class TestEmitFromCanvas
{
    private const float PICKUP_DISTANCE_MAX = 0.1f;

    Rakel Rakel;
    float RakelLength = 4;
    float RakelWidth = 2;
    Canvas_ Canvas;

    ComputeBuffer CanvasEmittedPaint;

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
        Rakel = new Rakel(RakelLength, RakelWidth, 1, 0.5f, 0);

        Canvas = new Canvas_(1, 0.015f);

        new FileLogger_().OnEnable();
    }

    [TearDown]
    public void Teardown()
    {
        CanvasEmittedPaint.Dispose();
        Canvas.Dispose();
        Rakel.Dispose();

        new FileLogger_().OnDisable();
    }

    [Test]
    public void Volume_Unrotated_Untilted()
    {
        // Arrange
        ShaderRegion canvasEmitSR = Rakel.ApplicationReservoir.GetFullShaderRegion();
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen), new FlatVolumeFiller(1)));
        Canvas.Reservoir.Duplicate(false);

        Rakel.UpdateState(
            new Vector3(-8, 0.5f, 0), // choose z for max paint pickup
            0, 0);

        // Act
        CanvasEmittedPaint = Canvas.EmitPaint(
            Rakel,
            canvasEmitSR,
            PICKUP_DISTANCE_MAX,
            0,
            1,
            false);


        // Assert
        Paint[] canvasEmittedPaintData = new Paint[canvasEmitSR.PixelCount];
        CanvasEmittedPaint.GetData(canvasEmittedPaintData);
        float[] canvasEmittedVolumes = new float[canvasEmitSR.PixelCount];
        for (int i = 0; i < canvasEmittedVolumes.Length; i++)
        {
            canvasEmittedVolumes[i] = canvasEmittedPaintData[i].Volume;
        }

        //LogUtil.Log(canvasEmittedVolumes, canvasEmitSC.Size.y, false);

        AssertUtil.AssertFloatsAreEqual(
            new float[] { // remember: these arrays are upside down compared to the actual pixels
                0.025f,  0.050f,
                0.025f,  0.050f,
                0.025f,  0.050f,
                0.025f,  0.050f,
            },
            canvasEmittedVolumes);

        AssertUtil.AssertFloatsEqual(
            0.298722476f,
            Sum(canvasEmittedVolumes));
    }

    [Test]
    public void Volume_Rotated30_Untilted()
    {
        // Arrange
        ShaderRegion canvasEmitSR = Rakel.ApplicationReservoir.GetFullShaderRegion();
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen), new FlatVolumeFiller(1)));
        Canvas.Reservoir.Duplicate(false);

        Rakel.UpdateState(
            new Vector3(-8, 0.5f, 0), // choose z for max paint pickup
            30, 0);


        // Act
        CanvasEmittedPaint = Canvas.EmitPaint(
            Rakel,
            canvasEmitSR,
            PICKUP_DISTANCE_MAX,
            0,
            1,
            false);


        // Assert
        Paint[] canvasEmittedPaintData = new Paint[canvasEmitSR.PixelCount];
        CanvasEmittedPaint.GetData(canvasEmittedPaintData);
        float[] canvasEmittedVolumes = new float[canvasEmitSR.PixelCount];
        for (int i = 0; i < canvasEmittedVolumes.Length; i++)
        {
            canvasEmittedVolumes[i] = canvasEmittedPaintData[i].Volume;
        }

        //LogUtil.Log(canvasEmittedVolumes, canvasEmitSC.Size.y, false);

        AssertUtil.AssertFloatsAreEqual(
            new float[] { // remember: these arrays are upside down compared to the actual pixels
                0.000f,  0.028f,
                0.008f,  0.049f,
                0.035f,  0.050f,
                0.050f,  0.050f,
            },
            canvasEmittedVolumes);

        AssertUtil.AssertFloatsEqual(
            0.268947482f,
            Sum(canvasEmittedVolumes));
    }

    [Test]
    public void Volume_Unrotated_Tilted5()
    {
        // Arrange
        ShaderRegion canvasEmitSR = Rakel.ApplicationReservoir.GetFullShaderRegion();
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen), new FlatVolumeFiller(1)));
        Canvas.Reservoir.Duplicate(false);

        Rakel.UpdateState(
            new Vector3(-4, -0.5f, 0), // choose z for max paint pickup
            0, 5);


        // Act
        CanvasEmittedPaint = Canvas.EmitPaint(
            Rakel,
            canvasEmitSR,
            PICKUP_DISTANCE_MAX,
            0,
            1,
            false);


        // Assert
        Paint[] canvasEmittedPaintData = new Paint[canvasEmitSR.PixelCount];
        CanvasEmittedPaint.GetData(canvasEmittedPaintData);
        float[] canvasEmittedVolumes = new float[canvasEmitSR.PixelCount];
        for (int i = 0; i < canvasEmittedVolumes.Length; i++)
        {
            canvasEmittedVolumes[i] = canvasEmittedPaintData[i].Volume;
        }

        //LogUtil.Log(canvasEmittedVolumes, canvasEmitSC.Size.y, false);

        AssertUtil.AssertFloatsAreEqual(
            new float[] { // remember: these arrays are upside down compared to the actual pixels
                0.034f,  0.000f,
                0.034f,  0.000f,
                0.034f,  0.000f,
                0.034f,  0.000f,
            },
            canvasEmittedVolumes);

        AssertUtil.AssertFloatsEqual(
            0.135346979f,
            Sum(canvasEmittedVolumes));
    }

    [Test]
    public void Volume_Rotated30_Tilted5()
    {
        // Arrange
        ShaderRegion canvasEmitSR = Rakel.ApplicationReservoir.GetFullShaderRegion();
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen), new FlatVolumeFiller(1)));
        Canvas.Reservoir.Duplicate(false);

        Rakel.UpdateState(new Vector3(-4, 0.5f, 0), 30, 5);


        // Act
        CanvasEmittedPaint = Canvas.EmitPaint(
            Rakel,
            canvasEmitSR,
            PICKUP_DISTANCE_MAX,
            0,
            1,
            false);


        // Assert
        Paint[] canvasEmittedPaintData = new Paint[canvasEmitSR.PixelCount];
        CanvasEmittedPaint.GetData(canvasEmittedPaintData);
        float[] canvasEmittedVolumes = new float[canvasEmitSR.PixelCount];
        for (int i = 0; i < canvasEmittedVolumes.Length; i++)
        {
            canvasEmittedVolumes[i] = canvasEmittedPaintData[i].Volume;
        }

        //LogUtil.Log(canvasEmittedVolumes, canvasEmitSC.Size.y, false);

        // when this is equal to the values in the non-rotated case, thats already helpful
        AssertUtil.AssertFloatsAreEqual(
            new float[] { // remember: these arrays are upside down compared to the actual pixels
                0.034f,  0.000f,
                0.034f,  0.000f,
                0.034f,  0.000f,
                0.034f,  0.000f,
            },
            canvasEmittedVolumes);

        AssertUtil.AssertFloatsEqual(
            0.135347947f,
            Sum(canvasEmittedVolumes));
    }
}
