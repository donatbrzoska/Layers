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
            wsc.PixelsX
        );

        Assert.AreEqual(
            8,
            wsc.PixelsY
        );
    }

    [Test]
    public void MapToPixel_OutOfRange_Left()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(-1.5001f, 0, 0));

        Assert.AreEqual(
            new Vector2Int(int.MinValue, int.MinValue),
            pixel
        );
    }

    [Test]
    public void MapToPixel_OutOfRange_Right()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(1.5001f, 0, 0));

        Assert.AreEqual(
            new Vector2Int(int.MinValue, int.MinValue),
            pixel
        );
    }

    [Test]
    public void MapToPixel_OutOfRange_Top()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(0, 1.5001f, 0));

        Assert.AreEqual(
            new Vector2Int(int.MinValue, int.MinValue),
            pixel
        );
    }

    [Test]
    public void MapToPixel_OutOfRange_Bottom()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(0, -1.5001f, 0));

        Assert.AreEqual(
            new Vector2Int(int.MinValue, int.MinValue),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Centered_EvenNumberOfPixels_Center()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(2, 2, 2, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(0, 0, 0));

        Assert.AreEqual(
            new Vector2Int(2, 2),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Centered_UnevenNumberOfPixels_Center()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(1, 1, 3, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(0, 0, 0));

        Assert.AreEqual(
            new Vector2Int(1, 1),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Centered_LargerUnevenNumberOfPixels_Center()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 3, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(0, 0, 0));

        Assert.AreEqual(
            new Vector2Int(4, 4),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Shifted_UpperRight_Center()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 3, new Vector3(1, 1, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(1, 1, 0));

        Assert.AreEqual(
            new Vector2Int(4, 4),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Shifted_LowerLeft_UpperLeftCorner()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 3, new Vector3(-1, -1, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(-2.5f, 0.5f, 0));

        Assert.AreEqual(
            new Vector2Int(0, 8),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Shifted_LowerLeft_UpperRightCorner()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 3, new Vector3(-1, -1, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(0.5f, 0.5f, 0));

        Assert.AreEqual(
            new Vector2Int(8, 8),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Shifted_LowerLeft_LowerLeftCorner()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 3, new Vector3(-1, -1, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(-2.5f, -2.5f, 0));

        Assert.AreEqual(
            new Vector2Int(0, 0),
            pixel
        );
    }

    [Test]
    public void MapToPixel_Shifted_LowerLeft_LowerRightCorner()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 3, new Vector3(-1, -1, 0));

        Vector2Int pixel = wsc.MapToPixel(new Vector3(0.5f, -2.5f, 0));

        Assert.AreEqual(
            new Vector2Int(8, 0),
            pixel
        );
    }

    [Test]
    public void MapToPixelInRange_Top()
    {
        WorldSpaceCanvas wsc = new WorldSpaceCanvas(3, 3, 1, new Vector3(0, 0, 0));

        Vector2Int pixel = wsc.MapToPixelInRange(new Vector3(0, 1.5001f, 0));

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

        Vector2Int pixel = wsc.MapToPixelInRange(new Vector3(1.5001f, 0, 0));

        Assert.AreEqual(
            new Vector2Int(2, 1),
            pixel
        );
    }
}
