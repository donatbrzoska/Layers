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
        CanvasResolution = 20;

        RakelRotation = 0;
        RakelRotationLocked = true;

        RakelConfiguration = new RakelConfiguration();
        FillConfiguration = new FillConfiguration();
        TransferConfiguration = new TransferConfiguration();
    }

    public void LoadDebug()
    {
        CanvasResolution = 1;

        RakelConfiguration.Resolution = 1;
        RakelConfiguration.Length = 2;
        RakelConfiguration.Width = 1;

        FillConfiguration.Volume = 1;
        FillConfiguration.Mode = FillMode.FlatColored;
    }

    public void LoadBenchmark()
    {
        CanvasResolution = 50;

        RakelConfiguration.Resolution = 50;
        RakelConfiguration.Length = 1;
        RakelConfiguration.Width = 1;

        TransferConfiguration.MapMode = TransferMapMode.NearestNeighbour;
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
        Resolution = 20;
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
        Volume = 120;
        Mode = FillMode.PerlinColored;
    }

}

public class TransferConfiguration
{
    public TransferMapMode MapMode;
    public int ReservoirSmoothingKernelSize;
    public int ReservoirDiscardVolumeThreshold;
    public float EmitVolumeApplicationReservoir;
    public float EmitVolumePickupReservoir;
    public float PickupVolume;

    public TransferConfiguration()
    {
        MapMode = TransferMapMode.Bilinear;
        ReservoirSmoothingKernelSize = 1;
        ReservoirDiscardVolumeThreshold = 10;
        EmitVolumeApplicationReservoir = 1;
        EmitVolumePickupReservoir = 1.2f;
        PickupVolume = 1.3f;
    }
}