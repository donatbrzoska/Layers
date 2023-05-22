using NUnit.Framework;
using UnityEngine;

public class TestWorldSpaceCanvas
{
    [Test]
    public void PixelsXY()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(2, 3, 4, new Vector3(0, 0, 0));

        Assert.AreEqual(
            12,
            wsc.TextureSize.x
        );

        Assert.AreEqual(
            8,
            wsc.TextureSize.y
        );
    }

    [Test]
    public void MapToPixel_Centered_Center()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(0, 0, 0));

        Assert.AreEqual(
            new Vector2Int(1, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Centered_Center_HigherResolution()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 3, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(0, 0, 0));

        Assert.AreEqual(
            new Vector2Int(4, 4),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Centered_Left_Inner()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(-0.499f, 0, 0));

        Assert.AreEqual(
            new Vector2Int(1, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Centered_Left_Outer()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(-0.501f, 0, 0));

        Assert.AreEqual(
            new Vector2Int(0, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Centered_Right_Inner()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(0.5f, 0, 0));

        Assert.AreEqual(
            new Vector2Int(1, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Centered_Right_Outer()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(0.501f, 0, 0));

        Assert.AreEqual(
            new Vector2Int(2, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Centered_Top_Inner()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(0, 0.5f, 0));

        Assert.AreEqual(
            new Vector2Int(1, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Centered_Top_Outer()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(0, 0.501f, 0));

        Assert.AreEqual(
            new Vector2Int(1, 2),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Centered_Bottom_Inner()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(0, -0.499f, 0));

        Assert.AreEqual(
            new Vector2Int(1, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Centered_Bottom_Outer()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(0, -0.5f, 0));

        Assert.AreEqual(
            new Vector2Int(1, 0),
            pixel
        );
    }

    [Test]
    public void MapToPixel_EvenNumberOfPixels_Centered_Center()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(2, 2, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(0, 0, 0));

        Assert.AreEqual(
            new Vector2Int(0, 0),
            pixel
        );
    }

    // TODO Uneven should be tested more

    [Test]
    public void MapToPixel_OutOfRange_Left_Inner()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(-1.499f, 0, 0));

        Assert.AreEqual(
            new Vector2Int(0, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_OutOfRange_Left_Outer()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(-1.5f, 0, 0));

        Assert.AreEqual(
            new Vector2Int(-1, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_OutOfRange_Right_Inner()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(1.5f, 0, 0));

        Assert.AreEqual(
            new Vector2Int(2, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_OutOfRange_Right_Outer()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(1.501f, 0, 0));

        Assert.AreEqual(
            new Vector2Int(3, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_OutOfRange_Top_Inner()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(0, 1.5f, 0));

        Assert.AreEqual(
            new Vector2Int(1, 2),
            pixel
        );
    }

    [Test]
    public void MapToPixel_OutOfRange_Top_Outer()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(0, 1.501f, 0));

        Assert.AreEqual(
            new Vector2Int(1, 3),
            pixel
        );
    }

    [Test]
    public void MapToPixel_OutOfRange_Bottom_Inner()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(0, -1.499f, 0));

        Assert.AreEqual(
            new Vector2Int(1, 0),
            pixel
        );
    }

    [Test]
    public void MapToPixel_OutOfRange_Bottom_Outer()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(0, -1.5f, 0));

        Assert.AreEqual(
            new Vector2Int(1, -1),
            pixel
        );
    }

    // [Test]
    // public void MapToPixel_Centeredsss_UpperRight_Center()
    // {
    //     WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 3, new Vector3(1, 1, 0));

    //     Vector2Int pixel = wsc.MapToPixel(new Vector3(1, 1, 0));

    //     Assert.AreEqual(
    //         new Vector2Int(4, 4),
    //         pixel
    //     );
    // }

    [Test]
    public void MapToPixel_Shifted_UpperRight_Center()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(1, 1, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(1, 1, 0));

        Assert.AreEqual(
            new Vector2Int(1, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Shifted_LowerLeft_LowerLeftCorner_Inner()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(-1, -1, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(-2.499f, -2.499f, 0));

        Assert.AreEqual(
            new Vector2Int(0, 0),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Shifted_LowerLeft_LowerLeftCorner_Outer()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(-1, -1, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(-2.5f, -2.5f, 0));

        Assert.AreEqual(
            new Vector2Int(-1, -1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Shifted_LowerLeft_UpperRightCorner_Inner()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(-1, -1, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(0.5f, 0.5f, 0));

        Assert.AreEqual(
            new Vector2Int(2, 2),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Shifted_LowerLeft_UpperRightCorner_Outer()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(-1, -1, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(0.501f, 0.501f, 0));

        Assert.AreEqual(
            new Vector2Int(3, 3),
            pixel
        );
    }

    [Test]
    public void MapToPixelInRange_Top()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixelInRange(new Vector3(0, 1.5f, 0));

        Assert.AreEqual(
            new Vector2Int(1, 2),
            pixel
        );
    }

    [Test]
    public void MapToPixelInRange_Bottom()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixelInRange(new Vector3(0, -1.5001f, 0));

        Assert.AreEqual(
            new Vector2Int(1, 0),
            pixel
        );
    }

    [Test]
    public void MapToPixelInRange_Left()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixelInRange(new Vector3(-1.5001f, 0, 0));

        Assert.AreEqual(
            new Vector2Int(0, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixelInRange_Right()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixelInRange(new Vector3(1.5f, 0, 0));

        Assert.AreEqual(
            new Vector2Int(2, 1),
            pixel
        );
    }
}
