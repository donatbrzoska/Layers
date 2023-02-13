using UnityEngine;

public class Configuration
{
    public int CanvasResolution;

    public float RakelRotation;
    public bool RakelRotationLocked;

    public RakelConfiguration RakelConfiguration;
    public FillConfiguration FillConfiguration;
    public TransferConfiguration TransferConfiguration;

    public Configuration()
    {
        CanvasResolution = 80;

        RakelRotation = 0;
        RakelRotationLocked = true;

        RakelConfiguration = new RakelConfiguration();
        FillConfiguration = new FillConfiguration();
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

public class FillConfiguration
{
    public Color_ Color;
    public int Volume;
    public FillMode Mode;

    public FillConfiguration()
    {
        Color = Color_.CadmiumGreen;
        Volume = 240;
        Mode = FillMode.PerlinColored;
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