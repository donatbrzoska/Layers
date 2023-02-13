public class Configuration
{
    public int CanvasResolution;

    public float RakelRotation;
    public bool RakelRotationLocked;
    public float RakelLength;
    public float RakelWidth;
    public int RakelResolution;

    public Paint FillPaint;
    public FillMode FillMode;

    public TransferMapMode TransferMapMode;
    public int ReservoirSmoothingKernelSize;
    public int ReservoirDiscardVolumeThreshold;

    public Configuration()
    {
        CanvasResolution = 80;

        RakelRotation = 0;
        RakelRotationLocked = true;
        RakelLength = 2.5f;
        RakelWidth = 0.5f;
        RakelResolution = CanvasResolution;

        FillPaint = new Paint(Colors.GetColor(_Color.CadmiumGreen), 240);
        FillMode = FillMode.PerlinColored;

        TransferMapMode = TransferMapMode.Bilinear;
        ReservoirSmoothingKernelSize = 1;
        ReservoirDiscardVolumeThreshold = 10;
    }
}