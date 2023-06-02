using System;
using NUnit.Framework;
using UnityEngine;

public class TestEmitFromRakel
{
    WorldSpaceCanvas WorldSpaceCanvas;
    float RakelLength = 4;
    float RakelWidth = 2;
    Rakel Rakel;

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
        WorldSpaceCanvas = new WorldSpaceCanvas(10, 15, 1, new Vector3(0, 0, 0));

        Rakel = new Rakel(RakelLength, RakelWidth, 1, 0.5f, 0);

        new FileLogger_().OnEnable();
    }

    [TearDown]
    public void Teardown()
    {
        RakelEmittedPaint.Dispose();
        Rakel.Dispose();
    }

    [Test]
    public void Volume_Unrotated_Untilted()
    {
        // Arrange
        Rakel.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen), new FlatVolumeFiller(1)));
        Rakel.UpdateState(new Vector3(-3, -0.5f, 0), 0, 0);
        ShaderCalculation rakelEmitSC = new ShaderCalculation(
            WorldSpaceCanvas.MapToPixelInRange(Rakel.UpperLeft),
            WorldSpaceCanvas.MapToPixelInRange(Rakel.UpperRight),
            WorldSpaceCanvas.MapToPixelInRange(Rakel.LowerLeft),
            WorldSpaceCanvas.MapToPixelInRange(Rakel.LowerRight),
            1 // not really needed for polygon clipping
        );


        // Act
        RakelEmittedPaint = Rakel.EmitPaint(
            rakelEmitSC,
            WorldSpaceCanvas,
            1,
            1, // unused
            false);


        // Assert
        Paint[] rakelEmittedPaintData = new Paint[rakelEmitSC.PixelCount];
        RakelEmittedPaint.GetData(rakelEmittedPaintData);
        float[] rakelEmittedVolumes = new float[rakelEmitSC.PixelCount];
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
        Rakel.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen), new FlatVolumeFiller(1)));
        Rakel.UpdateState(new Vector3(-3, -0.5f, 0), 30, 0);
        ShaderCalculation rakelEmitSC = new ShaderCalculation(
            WorldSpaceCanvas.MapToPixelInRange(Rakel.UpperLeft),
            WorldSpaceCanvas.MapToPixelInRange(Rakel.UpperRight),
            WorldSpaceCanvas.MapToPixelInRange(Rakel.LowerLeft),
            WorldSpaceCanvas.MapToPixelInRange(Rakel.LowerRight),
            1 // not really needed for polygon clipping
        );


        // Act
        RakelEmittedPaint = Rakel.EmitPaint(
            rakelEmitSC,
            WorldSpaceCanvas,
            1,
            1, // unused
            false);


        // Assert
        Paint[] rakelEmittedPaintData = new Paint[rakelEmitSC.PixelCount];
        RakelEmittedPaint.GetData(rakelEmittedPaintData);
        float[] rakelEmittedVolumes = new float[rakelEmitSC.PixelCount];
        for (int i = 0; i < rakelEmittedVolumes.Length; i++)
        {
            rakelEmittedVolumes[i] = rakelEmittedPaintData[i].Volume;
        }

        LogUtil.Log(rakelEmittedVolumes, rakelEmitSC.Size.y, false);

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
        Rakel.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen), new FlatVolumeFiller(1)));
        Rakel.UpdateState(new Vector3(-4, -0.5f, 0), 0, 60);
        ShaderCalculation rakelEmitSC = new ShaderCalculation(
            WorldSpaceCanvas.MapToPixelInRange(Rakel.UpperLeft),
            WorldSpaceCanvas.MapToPixelInRange(Rakel.UpperRight),
            WorldSpaceCanvas.MapToPixelInRange(Rakel.LowerLeft),
            WorldSpaceCanvas.MapToPixelInRange(Rakel.LowerRight),
            1 // not really needed for polygon clipping
        );


        // Act
        RakelEmittedPaint = Rakel.EmitPaint(
            rakelEmitSC,
            WorldSpaceCanvas,
            1,
            1, // unused
            false);


        // Assert
        Paint[] rakelEmittedPaintData = new Paint[rakelEmitSC.PixelCount];
        RakelEmittedPaint.GetData(rakelEmittedPaintData);
        float[] rakelEmittedVolumes = new float[rakelEmitSC.PixelCount];
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
                0, 0, 0, 0,
                0, Q, Q, 0,
                0, H, H, 0,
                0, H, H, 0,
                0, H, H, 0,
                0, Q, Q, 0,
                0, 0, 0, 0,
            },
            rakelEmittedVolumes);

        AssertUtil.AssertFloatsEqual(
            0.5f * RakelLength * RakelWidth * Paint.UNIT,
            Sum(rakelEmittedVolumes));
    }

    [Test]
    public void Volume_Rotated30_Tilted60()
    {
        // Arrange
        Rakel.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen), new FlatVolumeFiller(1)));
        Rakel.UpdateState(new Vector3(-3, -0.5f, 0), 30, 60);
        ShaderCalculation rakelEmitSC = new ShaderCalculation(
            WorldSpaceCanvas.MapToPixelInRange(Rakel.UpperLeft),
            WorldSpaceCanvas.MapToPixelInRange(Rakel.UpperRight),
            WorldSpaceCanvas.MapToPixelInRange(Rakel.LowerLeft),
            WorldSpaceCanvas.MapToPixelInRange(Rakel.LowerRight),
            1 // not really needed for polygon clipping
        );


        // Act
        RakelEmittedPaint = Rakel.EmitPaint(
            rakelEmitSC,
            WorldSpaceCanvas,
            1,
            1, // unused
            false);


        // Assert
        Paint[] rakelEmittedPaintData = new Paint[rakelEmitSC.PixelCount];
        RakelEmittedPaint.GetData(rakelEmittedPaintData);
        float[] rakelEmittedVolumes = new float[rakelEmitSC.PixelCount];
        for (int i = 0; i < rakelEmittedVolumes.Length; i++)
        {
            rakelEmittedVolumes[i] = rakelEmittedPaintData[i].Volume;
        }

        LogUtil.Log(rakelEmittedVolumes, rakelEmitSC.Size.y, false);

        AssertUtil.AssertFloatsAreEqual(
            new float[] { // remember: these arrays are upside down compared to the actual pixels
                 0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,
                 0.000f,  0.173f,  0.384f,  0.000f,  0.000f,  0.000f,
                 0.000f,  0.116f,  0.923f,  0.116f,  0.000f,  0.000f,
                 0.000f,  0.000f,  0.500f,  0.655f,  0.000f,  0.000f,
                 0.000f,  0.000f,  0.039f,  0.878f,  0.155f,  0.000f,
                 0.000f,  0.000f,  0.000f,  0.062f,  0.000f,  0.000f,
                 0.000f,  0.000f,  0.000f,  0.000f,  0.000f,  0.000f,
            },
            rakelEmittedVolumes);

        AssertUtil.AssertFloatsEqual(
            0.5f * RakelLength * RakelWidth * Paint.UNIT,
            Sum(rakelEmittedVolumes),
            0.001f);
    }
}