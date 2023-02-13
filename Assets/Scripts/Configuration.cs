public class Configuration
{
    public int CanvasResolution;

    public float RakelRotation;
    public bool RakelRotationLocked;
    public RakelConfiguration RakelConfiguration;

    public Paint FillPaint;
    public FillMode FillMode;

    public TransferConfiguration TransferConfiguration;

    public Configuration()
    {
        CanvasResolution = 80;

        RakelRotation = 0;
        RakelRotationLocked = true;
        RakelConfiguration = new RakelConfiguration();

        FillPaint = new Paint(Colors.GetColor(_Color.CadmiumGreen), 240);
        FillMode = FillMode.PerlinColored;

        TransferConfiguration = new TransferConfiguration();
    }
}

public class RakelConfiguration
{
    public float Length;
    public float Width;
    public int Resolution;

    public RakelConfiguration()
    {
        Length = 2.5f;
        Width = 0.5f;
        Resolution = 80;
    }
}

public class TransferConfiguration
{
    public TransferMapMode MapMode;
    public int ReservoirSmoothingKernelSize;
    public int ReservoirDiscardVolumeThreshold;

    public TransferConfiguration()
    {
        MapMode = TransferMapMode.Bilinear;
        ReservoirSmoothingKernelSize = 1;
        ReservoirDiscardVolumeThreshold = 10;
    }
}