using NUnit.Framework;
using UnityEngine;

public class TestEmitFromCanvasShader
{
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
        Rakel = new Rakel(RakelLength, RakelWidth, 1);

        Canvas = new Canvas_(1);

        new FileLogger_().OnEnable();
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
        ShaderCalculation canvasEmitSC = Rakel.ApplicationReservoir.GetShaderCalculation();
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen), new FlatVolumeFiller(1)));
        Canvas.Reservoir.Duplicate(false);

        Rakel.UpdateState(new Vector3(-6, -0.5f, 0), 0, 0);


        // Act
        CanvasEmittedPaint = Canvas.EmitPaint(
            Rakel,
            canvasEmitSC,
            1,
            false);


        // Assert
        Paint[] canvasEmittedPaintData = new Paint[canvasEmitSC.PixelCount];
        CanvasEmittedPaint.GetData(canvasEmittedPaintData);
        float[] canvasEmittedVolumes = new float[canvasEmitSC.PixelCount];
        for (int i = 0; i < canvasEmittedVolumes.Length; i++)
        {
            canvasEmittedVolumes[i] = canvasEmittedPaintData[i].Volume;
        }

        //LogUtil.Log(canvasEmittedVolumes, canvasEmitSC.Size.y, false);

        float Q = 0.25f * Paint.UNIT; // quarter
        float H = 0.5f * Paint.UNIT; // half
        float F = Paint.UNIT; // full
        AssertUtil.AssertFloatsAreEqual(
            new float[] { // remember: these arrays are upside down compared to the actual pixels
                H, F,
                H, F,
                H, F,
                H, F,
            },
            canvasEmittedVolumes);

        AssertUtil.AssertFloatsEqual(
            RakelLength * RakelWidth * Paint.UNIT * 0.75f,
            Sum(canvasEmittedVolumes));
    }

    [Test]
    public void Volume_Rotated30_Untilted()
    {
        // Arrange
        ShaderCalculation canvasEmitSC = Rakel.ApplicationReservoir.GetShaderCalculation();
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen), new FlatVolumeFiller(1)));
        Canvas.Reservoir.Duplicate(false);

        Rakel.UpdateState(new Vector3(-6, -0.5f, 0), 30, 0);


        // Act
        CanvasEmittedPaint = Canvas.EmitPaint(
            Rakel,
            canvasEmitSC,
            1,
            false);


        // Assert
        Paint[] canvasEmittedPaintData = new Paint[canvasEmitSC.PixelCount];
        CanvasEmittedPaint.GetData(canvasEmittedPaintData);
        float[] canvasEmittedVolumes = new float[canvasEmitSC.PixelCount];
        for (int i = 0; i < canvasEmittedVolumes.Length; i++)
        {
            canvasEmittedVolumes[i] = canvasEmittedPaintData[i].Volume;
        }

        //LogUtil.Log(canvasEmittedVolumes, canvasEmitSC.Size.y, false);

        AssertUtil.AssertFloatsAreEqual(
            new float[] { // remember: these arrays are upside down compared to the actual pixels
                0.021f,  0.845f,
                0.443f,  1.000f,
                0.938f,  1.000f,
                1.000f,  1.000f,
            },
            canvasEmittedVolumes);

        AssertUtil.AssertFloatsEqual(
            6.247f,
            Sum(canvasEmittedVolumes));
    }

    [Test]
    public void Volume_Unrotated_Tilted60()
    {
        // Arrange
        ShaderCalculation canvasEmitSC = Rakel.ApplicationReservoir.GetShaderCalculation();
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen), new FlatVolumeFiller(1)));
        Canvas.Reservoir.Duplicate(false);

        Rakel.UpdateState(new Vector3(-3, -0.5f, 0), 0, 60);


        // Act
        CanvasEmittedPaint = Canvas.EmitPaint(
            Rakel,
            canvasEmitSC,
            1,
            false);


        // Assert
        Paint[] canvasEmittedPaintData = new Paint[canvasEmitSC.PixelCount];
        CanvasEmittedPaint.GetData(canvasEmittedPaintData);
        float[] canvasEmittedVolumes = new float[canvasEmitSC.PixelCount];
        for (int i = 0; i < canvasEmittedVolumes.Length; i++)
        {
            canvasEmittedVolumes[i] = canvasEmittedPaintData[i].Volume;
        }

        //LogUtil.Log(canvasEmittedVolumes, canvasEmitSC.Size.y, false);

        float H = 0.5f * Paint.UNIT; // half
        AssertUtil.AssertFloatsAreEqual(
            new float[] { // remember: these arrays are upside down compared to the actual pixels
                H, H,
                H, H,
                H, H,
                H, H,
            },
            canvasEmittedVolumes);

        AssertUtil.AssertFloatsEqual(
            RakelLength * RakelWidth * Paint.UNIT * 0.5f,
            Sum(canvasEmittedVolumes));
    }

    [Test]
    public void Volume_Rotated30_Tilted60()
    {
        // Arrange
        ShaderCalculation canvasEmitSC = Rakel.ApplicationReservoir.GetShaderCalculation();
        Canvas.Reservoir.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen), new FlatVolumeFiller(1)));
        Canvas.Reservoir.Duplicate(false);

        Rakel.UpdateState(new Vector3(-3, -0.5f, 0), 30, 60);


        // Act
        CanvasEmittedPaint = Canvas.EmitPaint(
            Rakel,
            canvasEmitSC,
            1,
            false);


        // Assert
        Paint[] canvasEmittedPaintData = new Paint[canvasEmitSC.PixelCount];
        CanvasEmittedPaint.GetData(canvasEmittedPaintData);
        float[] canvasEmittedVolumes = new float[canvasEmitSC.PixelCount];
        for (int i = 0; i < canvasEmittedVolumes.Length; i++)
        {
            canvasEmittedVolumes[i] = canvasEmittedPaintData[i].Volume;
        }

        //LogUtil.Log(canvasEmittedVolumes, canvasEmitSC.Size.y, false);

        float H = 0.5f * Paint.UNIT; // half
        AssertUtil.AssertFloatsAreEqual(
            new float[] { // remember: these arrays are upside down compared to the actual pixels
                H, H,
                H, H,
                H, H,
                H, H,
            },
            canvasEmittedVolumes);

        AssertUtil.AssertFloatsEqual(
            RakelLength * RakelWidth * Paint.UNIT * 0.5f,
            Sum(canvasEmittedVolumes));
    }
}
