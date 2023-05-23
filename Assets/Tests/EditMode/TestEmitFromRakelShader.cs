﻿using System;
using NUnit.Framework;
using UnityEngine;

public class TestEmitFromRakelShader
{
    WorldSpaceCanvas WorldSpaceCanvas;
    float RakelLength = 4;
    float RakelWidth = 2;
    Rakel Rakel;

    ComputeBuffer RakelEmittedPaint;

    public int Sum(int[] values)
    {
        int res = 0;
        foreach (int e in values) {
            res += e;
        }
        return res;
    }

    [SetUp]
    public void Setup()
    {
        WorldSpaceCanvas = new WorldSpaceCanvas(10, 15, 1, new Vector3(0, 0, 0));

        Rakel = new Rakel(RakelLength, RakelWidth, 1);

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
        int[] rakelEmittedVolumes = new int[rakelEmitSC.PixelCount];
        for (int i = 0; i < rakelEmittedVolumes.Length; i++)
        {
            rakelEmittedVolumes[i] = rakelEmittedPaintData[i].Volume;
        }

        int Q = (int)(0.25f * Paint.UNIT); // quarter
        int H = (int)(0.5f * Paint.UNIT); // half
        int F = Paint.UNIT; // full
        Assert.AreEqual(
            new int[] { // remember: these arrays are upside down compared to the actual pixels
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
        int[] rakelEmittedVolumes = new int[rakelEmitSC.PixelCount];
        for (int i = 0; i < rakelEmittedVolumes.Length; i++)
        {
            rakelEmittedVolumes[i] = rakelEmittedPaintData[i].Volume;
        }

        Assert.AreEqual(
            new int[] { // remember: these arrays are upside down compared to the actual pixels
                0,      0,      0,      0,      0,      0,      0,
                0,      0,      0,   6217,      0,      0,      0,
                0,   5384,  65468,  88118,   3867,      0,      0,
                0,    833,  80107,  99999,  50000,      0,      0,
                0,      0,  23482,  99721,  96132,  11603,      0,
                0,      0,      0,  65468,  80107,  17265,      0,
                0,      0,      0,   5384,    833,      0,      0,
                0,      0,      0,      0,      0,      0,      0,
            },
            rakelEmittedVolumes);

        Assert.AreEqual(
            799988, // (int) (RakelConfig.Length * RakelConfig.Width * Paint.UNIT),
            Sum(rakelEmittedVolumes));
    }

    [Test]
    public void Volume_Unrotated_Tilted60()
    {
        // Arrange
        Rakel.Fill(new ReservoirFiller(new FlatColorFiller(Color_.CadmiumGreen), new FlatVolumeFiller(1)));
        Rakel.UpdateState(new Vector3(-3, -0.5f, 0), 0, 60);
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
        int[] rakelEmittedVolumes = new int[rakelEmitSC.PixelCount];
        for (int i = 0; i < rakelEmittedVolumes.Length; i++)
        {
            rakelEmittedVolumes[i] = rakelEmittedPaintData[i].Volume;
        }

        Assert.AreEqual(
            new int[] { // remember: these arrays are upside down compared to the actual pixels
                0,      0,      0,      0,
                0,  25000,  25000,      0,
                0,  50000,  50000,      0,
                0,  50000,  50000,      0,
                0,  50000,  50000,      0,
                0,  24999,  25000,      0,
                0,      0,      0,      0,
            },
            rakelEmittedVolumes);

        Assert.AreEqual(
            0.5f * RakelLength * RakelWidth * Paint.UNIT - 1,
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
        int[] rakelEmittedVolumes = new int[rakelEmitSC.PixelCount];
        for (int i = 0; i < rakelEmittedVolumes.Length; i++)
        {
            rakelEmittedVolumes[i] = rakelEmittedPaintData[i].Volume;
        }

        Assert.AreEqual(
            new int[] { // remember: these arrays are upside down compared to the actual pixels
                0,      0,      0,      0,      0,      0,
                0,      0,   6216,      0,      0,      0,
                0,  15469,  87840,      0,      0,      0,
                0,      0,  65469,  46132,      0,      0,
                0,      0,  11601,  92264,  11602,      0,
                0,      0,      0,  34529,  17264,      0,
                0,      0,      0,      0,      0,      0,
            },
            rakelEmittedVolumes);

        Assert.AreEqual(
            388386, // 0.5f * RakelLength * RakelWidth * Paint.UNIT,
            Sum(rakelEmittedVolumes));
    }
}