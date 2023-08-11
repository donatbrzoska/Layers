using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestPaint_alpha_blend
{
    private const int KERNEL_ID_alpha_blend = 1;

    List<CSAttribute> Attributes;

    [SetUp]
    public void Setup()
    {
        Attributes = new List<CSAttribute>();
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
    public void keep_volume()
    {
        // Arrange
        Paint a = new Paint();
        a.Color = new Vector3(1, 1, 1);
        a.Volume = 0.5f;
        Paint[] cbData = new Paint[] { a, new Paint() };

        ComputeBuffer cb = new ComputeBuffer(2, Paint.SizeInBytes);
        cb.SetData(cbData);
        Attributes.Add(new CSComputeBuffer("Paint_A_Result", cb));
        Attributes.Add(new CSFloat3("BackgroundColor", new Vector3(0, 0, 0))); // not relevant


        // Act
        Execute(KERNEL_ID_alpha_blend);


        // Assert
        cb.GetData(cbData);
        Paint result = cbData[1];

        Assert.AreEqual(
            0.5f,
            result.Volume);

        cb.Dispose();
    }

    [Test]
    public void zero_volume()
    {
        Vector3 BACKGROUND_COLOR = new Vector3(1, 1, 1);

        // Arrange
        Paint a = new Paint();
        a.Color = new Vector3(0.2f, 0.3f, 0.4f);
        a.Volume = 0;
        Paint[] cbData = new Paint[] { a, new Paint() };

        ComputeBuffer cb = new ComputeBuffer(2, Paint.SizeInBytes);
        cb.SetData(cbData);
        Attributes.Add(new CSComputeBuffer("Paint_A_Result", cb));
        Attributes.Add(new CSFloat3("BackgroundColor", BACKGROUND_COLOR));


        // Act
        Execute(KERNEL_ID_alpha_blend);


        // Assert
        cb.GetData(cbData);
        Paint result = cbData[1];

        Assert.AreEqual(
            BACKGROUND_COLOR,
            result.Color);

        cb.Dispose();
    }

    [Test]
    public void one_volume()
    {
        Vector3 PAINT_COLOR = new Vector3(0.2f, 0.3f, 0.4f);

        // Arrange
        Paint a = new Paint();
        a.Color = PAINT_COLOR;
        a.Volume = 1;
        Paint[] cbData = new Paint[] { a, new Paint() };

        ComputeBuffer cb = new ComputeBuffer(2, Paint.SizeInBytes);
        cb.SetData(cbData);
        Attributes.Add(new CSComputeBuffer("Paint_A_Result", cb));
        Attributes.Add(new CSFloat3("BackgroundColor", new Vector3(1, 1, 1)));


        // Act
        Execute(KERNEL_ID_alpha_blend);


        // Assert
        cb.GetData(cbData);
        Paint result = cbData[1];

        Assert.AreEqual(
            PAINT_COLOR,
            result.Color);

        cb.Dispose();
    }
}