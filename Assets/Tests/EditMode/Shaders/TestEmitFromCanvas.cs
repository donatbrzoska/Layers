using NUnit.Framework;
using UnityEngine;

public class TestEmitFromCanvas
{
    Rakel Rakel;
    float RakelLength = 4;
    float RakelWidth = 2;

    Canvas_ Canvas;
    float CanvasWidth = 15;
    float CanvasHeight = 10;

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
        Rakel = new Rakel(RakelLength, RakelWidth, 1, MAX_LAYERS, 0.5f, 0);

        Canvas = new Canvas_(CanvasWidth, CanvasHeight, MAX_LAYERS, new Vector3(0, 0, 0), 1, 0.015f, 0);

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

    // TODO Parametrize and include MIN_VOLUME_TO_STAY
    [Test]
    public void EmitOneLayer_Unrotated_Untilted()
    {
        // Arrange
        ShaderRegion canvasEmitSR = Rakel.Reservoir.GetFullShaderRegion();
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen, ColorSpace.RGB), new FlatVolumeFiller(1)));
        Canvas.Reservoir.Duplicate(false);

        Rakel.UpdateState(
            new Vector3(-5, 0.5f, 0), 0, 0,
            0, 0);

        // Act
        CanvasEmittedPaint = Canvas.EmitPaint(
            Rakel,
            canvasEmitSR,
            0);


        // Assert
        CanvasEmittedPaint.Readback();
        float[] canvasEmittedVolumes = new float[CanvasEmittedPaint.InfoData.Length];
        for (int i = 0; i < canvasEmittedVolumes.Length; i++)
        {
            canvasEmittedVolumes[i] = CanvasEmittedPaint.InfoData[i].Volume;
        }

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
            0.9f * RakelLength * RakelWidth * Paint.UNIT,
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
            new Vector3(-5, 0.5f, -Paint.VOLUME_THICKNESS), 0, 0,
            0, 0);

        // Act
        CanvasEmittedPaint = Canvas.EmitPaint(
            Rakel,
            canvasEmitSR,
            0);


        // Assert

        // 1. Check emitted paint
        CanvasEmittedPaint.Readback();
        float[] canvasEmittedVolumes = new float[CanvasEmittedPaint.InfoData.Length];
        for (int i = 0; i < canvasEmittedVolumes.Length; i++)
        {
            canvasEmittedVolumes[i] = CanvasEmittedPaint.InfoData[i].Volume;
        }

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
            RakelLength * RakelWidth * Paint.UNIT,
            Sum(canvasEmittedVolumes));

        // 2. Check paint on canvas
        Canvas.Reservoir.PaintGrid.Readback();
        float[] canvasVolumes = new float[Canvas.Reservoir.PaintGrid.InfoData.Length];
        for (int i = 0; i < Canvas.Reservoir.PaintGrid.InfoData.Length; i++)
        {
            canvasVolumes[i] = Canvas.Reservoir.PaintGrid.InfoData[i].Volume;
        }
        AssertUtil.AssertFloatsEqual(
            2 * CanvasWidth * CanvasHeight * Paint.UNIT - Sum(canvasEmittedVolumes),
            Sum(canvasVolumes));
    }

    [Test]
    public void EmitOneOfTwoLayers_Rotated30_Untilted()
    {
        // Arrange
        ShaderRegion canvasEmitSR = Rakel.Reservoir.GetFullShaderRegion();
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen, ColorSpace.RGB), new FlatVolumeFiller(2)));
        Canvas.Reservoir.Duplicate(false);

        Rakel.UpdateState(
            new Vector3(-5, 0.5f, -Paint.VOLUME_THICKNESS), 0, 0,
            30, 0);

        // Act
        CanvasEmittedPaint = Canvas.EmitPaint(
            Rakel,
            canvasEmitSR,
            0);


        // Assert
        CanvasEmittedPaint.Readback();
        float[] canvasEmittedVolumes = new float[CanvasEmittedPaint.InfoData.Length];
        for (int i = 0; i < canvasEmittedVolumes.Length; i++)
        {
            canvasEmittedVolumes[i] = CanvasEmittedPaint.InfoData[i].Volume;
        }

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
            RakelLength * RakelWidth * Paint.UNIT,
            Sum(canvasEmittedVolumes));
    }
}
