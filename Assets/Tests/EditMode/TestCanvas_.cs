using NUnit.Framework;
using UnityEngine;

public class TestCanvas_
{
    private Canvas_ CreateCentered3x3Canvas()
    {
        return new Canvas_(3, 3, new Vector3(0, 0, 0), 1, 0, 0);
    }

    private Canvas_ CreateShiftedLL3x3Canvas()
    {
        return new Canvas_(3, 3, new Vector3(-1, -1, 0), 1, 0, 0);
    }

    [Test]
    public void PixelsXY()
    {
        Canvas_ canvas = new Canvas_(3, 2, new Vector3(0, 0, 0), 4, 0, 0);

        Assert.AreEqual(
            12,
            canvas.TextureSize.x
        );

        Assert.AreEqual(
            8,
            canvas.TextureSize.y
        );
    }

    [Test]
    public void MapToPixel_Centered_Center()
    {
        Canvas_ canvas = CreateCentered3x3Canvas();

        Vector2Int pixel = canvas.MapToPixel(new Vector3(0, 0, 0));

        Assert.AreEqual(
            new Vector2Int(1, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Centered_Center_HigherResolution()
    {
        Canvas_ canvas = new Canvas_(3, 3, new Vector3(0, 0, 0), 3, 0, 0);

        Vector2Int pixel = canvas.MapToPixel(new Vector3(0, 0, 0));

        Assert.AreEqual(
            new Vector2Int(4, 4),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Centered_Left_Inner()
    {
        Canvas_ canvas = CreateCentered3x3Canvas();

        Vector2Int pixel = canvas.MapToPixel(new Vector3(-0.499f, 0, 0));

        Assert.AreEqual(
            new Vector2Int(1, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Centered_Left_Outer()
    {
        Canvas_ canvas = CreateCentered3x3Canvas();

        Vector2Int pixel = canvas.MapToPixel(new Vector3(-0.501f, 0, 0));

        Assert.AreEqual(
            new Vector2Int(0, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Centered_Right_Inner()
    {
        Canvas_ canvas = CreateCentered3x3Canvas();

        Vector2Int pixel = canvas.MapToPixel(new Vector3(0.5f, 0, 0));

        Assert.AreEqual(
            new Vector2Int(1, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Centered_Right_Outer()
    {
        Canvas_ canvas = CreateCentered3x3Canvas();

        Vector2Int pixel = canvas.MapToPixel(new Vector3(0.501f, 0, 0));

        Assert.AreEqual(
            new Vector2Int(2, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Centered_Top_Inner()
    {
        Canvas_ canvas = CreateCentered3x3Canvas();

        Vector2Int pixel = canvas.MapToPixel(new Vector3(0, 0.5f, 0));

        Assert.AreEqual(
            new Vector2Int(1, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Centered_Top_Outer()
    {
        Canvas_ canvas = CreateCentered3x3Canvas();

        Vector2Int pixel = canvas.MapToPixel(new Vector3(0, 0.501f, 0));

        Assert.AreEqual(
            new Vector2Int(1, 2),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Centered_Bottom_Inner()
    {
        Canvas_ canvas = CreateCentered3x3Canvas();

        Vector2Int pixel = canvas.MapToPixel(new Vector3(0, -0.499f, 0));

        Assert.AreEqual(
            new Vector2Int(1, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Centered_Bottom_Outer()
    {
        Canvas_ canvas = CreateCentered3x3Canvas();

        Vector2Int pixel = canvas.MapToPixel(new Vector3(0, -0.5f, 0));

        Assert.AreEqual(
            new Vector2Int(1, 0),
            pixel
        );
    }

    [Test]
    public void MapToPixel_EvenNumberOfPixels_Centered_Center()
    {
        Canvas_ canvas = new Canvas_(2, 2, new Vector3(0, 0, 0), 1, 0, 0);

        Vector2Int pixel = canvas.MapToPixel(new Vector3(0, 0, 0));

        Assert.AreEqual(
            new Vector2Int(0, 0),
            pixel
        );
    }

    // TODO Uneven should be tested more

    [Test]
    public void MapToPixel_OutOfRange_Left_Inner()
    {
        Canvas_ canvas = CreateCentered3x3Canvas();

        Vector2Int pixel = canvas.MapToPixel(new Vector3(-1.499f, 0, 0));

        Assert.AreEqual(
            new Vector2Int(0, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_OutOfRange_Left_Outer()
    {
        Canvas_ canvas = CreateCentered3x3Canvas();

        Vector2Int pixel = canvas.MapToPixel(new Vector3(-1.5f, 0, 0));

        Assert.AreEqual(
            new Vector2Int(-1, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_OutOfRange_Right_Inner()
    {
        Canvas_ canvas = CreateCentered3x3Canvas();

        Vector2Int pixel = canvas.MapToPixel(new Vector3(1.5f, 0, 0));

        Assert.AreEqual(
            new Vector2Int(2, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_OutOfRange_Right_Outer()
    {
        Canvas_ canvas = CreateCentered3x3Canvas();

        Vector2Int pixel = canvas.MapToPixel(new Vector3(1.501f, 0, 0));

        Assert.AreEqual(
            new Vector2Int(3, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_OutOfRange_Top_Inner()
    {
        Canvas_ canvas = CreateCentered3x3Canvas();

        Vector2Int pixel = canvas.MapToPixel(new Vector3(0, 1.5f, 0));

        Assert.AreEqual(
            new Vector2Int(1, 2),
            pixel
        );
    }

    [Test]
    public void MapToPixel_OutOfRange_Top_Outer()
    {
        Canvas_ canvas = CreateCentered3x3Canvas();

        Vector2Int pixel = canvas.MapToPixel(new Vector3(0, 1.501f, 0));

        Assert.AreEqual(
            new Vector2Int(1, 3),
            pixel
        );
    }

    [Test]
    public void MapToPixel_OutOfRange_Bottom_Inner()
    {
        Canvas_ canvas = CreateCentered3x3Canvas();

        Vector2Int pixel = canvas.MapToPixel(new Vector3(0, -1.499f, 0));

        Assert.AreEqual(
            new Vector2Int(1, 0),
            pixel
        );
    }

    [Test]
    public void MapToPixel_OutOfRange_Bottom_Outer()
    {
        Canvas_ canvas = CreateCentered3x3Canvas();

        Vector2Int pixel = canvas.MapToPixel(new Vector3(0, -1.5f, 0));

        Assert.AreEqual(
            new Vector2Int(1, -1),
            pixel
        );
    }

    // [Test]
    // public void MapToPixel_Centeredsss_UpperRight_Center()
    // {
    //     Canvas_ canvas = new Canvas_(3, 3, 3, new Vector3(1, 1, 0));

    //     Vector2Int pixel = canvas.MapToPixel(new Vector3(1, 1, 0));

    //     Assert.AreEqual(
    //         new Vector2Int(4, 4),
    //         pixel
    //     );
    // }

    [Test]
    public void MapToPixel_Shifted_UpperRight_Center()
    {
        Canvas_ canvas = new Canvas_(3, 3, new Vector3(1, 1, 0), 1, 0, 0);

        Vector2Int pixel = canvas.MapToPixel(new Vector3(1, 1, 0));

        Assert.AreEqual(
            new Vector2Int(1, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Shifted_LowerLeft_LowerLeftCorner_Inner()
    {
        Canvas_ canvas = CreateShiftedLL3x3Canvas();

        Vector2Int pixel = canvas.MapToPixel(new Vector3(-2.499f, -2.499f, 0));

        Assert.AreEqual(
            new Vector2Int(0, 0),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Shifted_LowerLeft_LowerLeftCorner_Outer()
    {
        Canvas_ canvas = CreateShiftedLL3x3Canvas();

        Vector2Int pixel = canvas.MapToPixel(new Vector3(-2.5f, -2.5f, 0));

        Assert.AreEqual(
            new Vector2Int(-1, -1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Shifted_LowerLeft_UpperRightCorner_Inner()
    {
        Canvas_ canvas = CreateShiftedLL3x3Canvas();

        Vector2Int pixel = canvas.MapToPixel(new Vector3(0.5f, 0.5f, 0));

        Assert.AreEqual(
            new Vector2Int(2, 2),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Shifted_LowerLeft_UpperRightCorner_Outer()
    {
        Canvas_ canvas = CreateShiftedLL3x3Canvas();

        Vector2Int pixel = canvas.MapToPixel(new Vector3(0.501f, 0.501f, 0));

        Assert.AreEqual(
            new Vector2Int(3, 3),
            pixel
        );
    }

    [Test]
    public void MapToPixelInRange_Top()
    {
        Canvas_ canvas = CreateCentered3x3Canvas();

        Vector2Int pixel = canvas.MapToPixelInRange(new Vector3(0, 1.5f, 0));

        Assert.AreEqual(
            new Vector2Int(1, 2),
            pixel
        );
    }

    [Test]
    public void MapToPixelInRange_Bottom()
    {
        Canvas_ canvas = CreateCentered3x3Canvas();

        Vector2Int pixel = canvas.MapToPixelInRange(new Vector3(0, -1.5001f, 0));

        Assert.AreEqual(
            new Vector2Int(1, 0),
            pixel
        );
    }

    [Test]
    public void MapToPixelInRange_Left()
    {
        Canvas_ canvas = CreateCentered3x3Canvas();

        Vector2Int pixel = canvas.MapToPixelInRange(new Vector3(-1.5001f, 0, 0));

        Assert.AreEqual(
            new Vector2Int(0, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixelInRange_Right()
    {
        Canvas_ canvas = CreateCentered3x3Canvas();

        Vector2Int pixel = canvas.MapToPixelInRange(new Vector3(1.5f, 0, 0));

        Assert.AreEqual(
            new Vector2Int(2, 1),
            pixel
        );
    }
}
