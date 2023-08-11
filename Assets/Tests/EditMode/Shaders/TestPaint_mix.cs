using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestPaint_mix
{
    private const int KERNEL_ID_mix = 0;

    List<CSAttribute> Attributes;

    [SetUp]
    public void Setup()
    {
        Attributes = new List<CSAttribute>();

        new FileLogger_().OnEnable();
    }

    [TearDown]
    public void Teardown()
    {
        new FileLogger_().OnDisable();
    }

    private ComputeShaderTask Execute(int kernelID)
    {
        ComputeShaderTask cst = new ComputeShaderTask(
            "Tests/TestPaint",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            Attributes,
            true,
            kernelID);

        cst.Run();

        return cst;
    }

    [Test]
    public void mix_zero_volume()
    {
        // Arrange
        Paint a = new Paint();
        a.Color = new Vector3(0, 0, 0);
        a.Volume = 0;
        Paint b = new Paint();
        b.Color = new Vector3(1, 1, 1);
        b.Volume = 0;
        Paint placeholder = new Paint(new Vector3(1, 2, 3), 5);
        Paint[] cbData = new Paint[] { b, a, placeholder };

        ComputeBuffer cb = new ComputeBuffer(3, Paint.SizeInBytes);
        cb.SetData(cbData);
        Attributes.Add(new CSComputeBuffer("Paint_A_B_Result", cb));


        // Act
        Execute(KERNEL_ID_mix);


        // Assert
        cb.GetData(cbData);
        Paint result = cbData[2];

        Paint expected = new Paint();
        expected.Color = new Vector3(0, 0, 0);
        expected.Volume = 0;

        Assert.AreEqual(
            expected,
            result);

        cb.Dispose();
    }

    [Test]
    public void mix_zero_volume_other_way_around()
    {
        // Arrange
        Paint a = new Paint();
        a.Color = new Vector3(0, 0, 0);
        a.Volume = 0;
        Paint b = new Paint();
        b.Color = new Vector3(1, 1, 1);
        b.Volume = 0;
        Paint placeholder = new Paint(new Vector3(1, 2, 3), 5);
        Paint[] cbData = new Paint[] { b, a, placeholder };

        ComputeBuffer cb = new ComputeBuffer(3, Paint.SizeInBytes);
        cb.SetData(cbData);
        Attributes.Add(new CSComputeBuffer("Paint_A_B_Result", cb));


        // Act
        Execute(KERNEL_ID_mix);


        // Assert
        cb.GetData(cbData);
        Paint result = cbData[2];

        Paint expected = new Paint();
        expected.Color = new Vector3(0, 0, 0);
        expected.Volume = 0;

        Assert.AreEqual(
            expected,
            result);

        cb.Dispose();
    }

    [Test]
    public void mix_with_zero_volume()
    {
        // Arrange
        Paint a = new Paint();
        a.Color = new Vector3(1, 1, 1);
        a.Volume = 0.5f * Paint.UNIT;
        Paint b = new Paint();
        b.Color = new Vector3(0, 0, 0);
        b.Volume = 0;
        Paint placeholder = new Paint(new Vector3(1, 2, 3), 5);
        Paint[] cbData = new Paint[] { b, a, placeholder };

        ComputeBuffer cb = new ComputeBuffer(3, Paint.SizeInBytes);
        cb.SetData(cbData);
        Attributes.Add(new CSComputeBuffer("Paint_A_B_Result", cb));


        // Act
        Execute(KERNEL_ID_mix);


        // Assert
        cb.GetData(cbData);
        Paint result = cbData[2];

        Assert.AreEqual(
            a,
            result);

        cb.Dispose();
    }

    [Test]
    public void mix_zero_with_more_than_PAINT_UNIT()
    {
        // Arrange
        Paint a = new Paint();
        a.Color = new Vector3(1, 1, 1);
        a.Volume = 5.4f * Paint.UNIT;
        Paint b = new Paint();
        b.Color = new Vector3(0, 0, 0);
        b.Volume = 0;
        Paint placeholder = new Paint(new Vector3(1, 2, 3), 5);
        Paint[] cbData = new Paint[] { b, a, placeholder };

        ComputeBuffer cb = new ComputeBuffer(3, Paint.SizeInBytes);
        cb.SetData(cbData);
        Attributes.Add(new CSComputeBuffer("Paint_A_B_Result", cb));


        // Act
        Execute(KERNEL_ID_mix);


        // Assert
        cb.GetData(cbData);
        Paint result = cbData[2];

        Assert.AreEqual(
            a,
            result);

        cb.Dispose();
    }

    [Test]
    public void mix_more_than_PAINT_UNIT()
    {
        // Arrange
        Paint a = new Paint();
        a.Color = new Vector3(0, 0, 0);
        a.Volume = 1 * Paint.UNIT;
        Paint b = new Paint();
        b.Color = new Vector3(1, 1, 1);
        b.Volume = 3 * Paint.UNIT;
        Paint placeholder = new Paint(new Vector3(1, 2, 3), 5);
        Paint[] cbData = new Paint[] { b, a, placeholder };

        ComputeBuffer cb = new ComputeBuffer(3, Paint.SizeInBytes);
        cb.SetData(cbData);
        Attributes.Add(new CSComputeBuffer("Paint_A_B_Result", cb));


        // Act
        Execute(KERNEL_ID_mix);


        // Assert
        cb.GetData(cbData);
        Paint result = cbData[2];

        Paint expected = new Paint();
        expected.Color = new Vector3(0.75f * Paint.UNIT, 0.75f * Paint.UNIT, 0.75f * Paint.UNIT);
        expected.Volume = 4 * Paint.UNIT;

        Assert.AreEqual(
            expected,
            result);

        cb.Dispose();
    }

    [Test]
    public void mix_less_than_PAINT_UNIT()
    {
        // Arrange
        Paint a = new Paint();
        a.Color = new Vector3(0, 0, 0);
        a.Volume = 0.1f * Paint.UNIT;
        Paint b = new Paint();
        b.Color = new Vector3(1, 1, 1);
        b.Volume = 0.3f * Paint.UNIT;
        Paint placeholder = new Paint(new Vector3(1, 2, 3), 5);
        Paint[] cbData = new Paint[] { b, a, placeholder };

        ComputeBuffer cb = new ComputeBuffer(3, Paint.SizeInBytes);
        cb.SetData(cbData);
        Attributes.Add(new CSComputeBuffer("Paint_A_B_Result", cb));


        // Act
        Execute(KERNEL_ID_mix);


        // Assert
        cb.GetData(cbData);
        Paint result = cbData[2];

        Paint expected = new Paint();
        expected.Color = new Vector3(0.75f * Paint.UNIT, 0.75f * Paint.UNIT, 0.75f * Paint.UNIT);
        expected.Volume = 0.4f * Paint.UNIT;

        Assert.AreEqual(
            expected,
            result);

        cb.Dispose();
    }

    [Test]
    public void mix_white_white()
    {
        // Arrange
        Paint a = new Paint();
        a.Color = new Vector3(1, 1, 1);
        a.Volume = 1 * Paint.UNIT;
        Paint b = a;
        Paint placeholder = new Paint(new Vector3(1, 2, 3), 5);
        Paint[] cbData = new Paint[] { b, a, placeholder };

        ComputeBuffer cb = new ComputeBuffer(3, Paint.SizeInBytes);
        cb.SetData(cbData);
        Attributes.Add(new CSComputeBuffer("Paint_A_B_Result", cb));


        // Act
        Execute(KERNEL_ID_mix);


        // Assert
        cb.GetData(cbData);
        Paint result = cbData[2];

        Paint expected = new Paint();
        expected.Color = new Vector3(1 * Paint.UNIT, 1 * Paint.UNIT, 1 * Paint.UNIT);
        expected.Volume = 2 * Paint.UNIT;

        Assert.AreEqual(
            expected,
            result);

        cb.Dispose();
    }

    [Test]
    public void mix_negative_volume()
    {
        // Arrange
        Paint a = new Paint();
        a.Color = new Vector3(1, 1, 1);
        a.Volume = 1 * Paint.UNIT;
        Paint b = new Paint();
        b.Color = new Vector3(1, 1, 1);
        b.Volume = -1 * Paint.UNIT;
        Paint placeholder = new Paint(new Vector3(1, 2, 3), 5);
        Paint[] cbData = new Paint[] { b, a, placeholder };

        ComputeBuffer cb = new ComputeBuffer(3, Paint.SizeInBytes);
        cb.SetData(cbData);
        Attributes.Add(new CSComputeBuffer("Paint_A_B_Result", cb));


        // Act
        Execute(KERNEL_ID_mix);


        // Assert
        cb.GetData(cbData);
        Paint result = cbData[2];

        Paint expected = new Paint();
        expected.Color = new Vector3(1 * Paint.UNIT, 1 * Paint.UNIT, 1 * Paint.UNIT);
        expected.Volume = 1 * Paint.UNIT;

        Assert.AreEqual(
            expected,
            result);

        cb.Dispose();
    }
}