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
        FillConfiguration.VolumeMode = VolumeMode.Flat;
        FillConfiguration.ColorMode = ColorMode.Colorful;
    }

    public void LoadBenchmark()
    {
        CanvasResolution = 50;

        RakelConfiguration.Resolution = 50;
        RakelConfiguration.Length = 1;
        RakelConfiguration.Width = 1;

        TransferConfiguration.MapMode = TransferMapMode.NearestNeighbour;
    }

    public void LoadPixelMapping()
    {
        CanvasResolution = 80;

        RakelRotationLocked = false;
        RakelConfiguration.Resolution = 80;

        TransferConfiguration.MapMode = TransferMapMode.NearestNeighbour;
        TransferConfiguration.EmitVolumePickupReservoir = 0;
        TransferConfiguration.PickupVolume = 0;

        FillConfiguration.Color = Color_.CadmiumGreen;
        FillConfiguration.Volume = 300;
        FillConfiguration.VolumeMode = VolumeMode.Flat;
    }

    public void LoadPresentation()
    {
        CanvasResolution = 80;

        RakelRotationLocked = false;
        RakelConfiguration.Resolution = 80;

        TransferConfiguration.MapMode = TransferMapMode.PolygonClipping;

        FillConfiguration.Color = Color_.DarkRed;
        FillConfiguration.Volume = 600;
        FillConfiguration.VolumeMode = VolumeMode.Perlin;

        /*
         * Curve
         * > Clear Rakel
         * > Fill with Cadmium Yellow
         * Back and forth
         * 
         * > FillConfiguration.Volume = 200;
         * > FillConfiguration.Mode = FillMode.PerlinNoiseColored
         * Line and back
         * Smear from left to right and from right to left
         */
    }

    public void LoadMappingResults()
    {
        CanvasResolution = 80;
        RakelConfiguration.Resolution = 80;
        FillConfiguration.Volume = 40;
        FillConfiguration.VolumeMode = VolumeMode.Flat;
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
    public ColorMode ColorMode;
    public int Volume;
    public VolumeMode VolumeMode;

    public FillConfiguration()
    {
        Color = Color_.CadmiumGreen;
        ColorMode = ColorMode.Colorful;
        Volume = 120;
        VolumeMode = VolumeMode.Perlin;
    }

}

public class TransferConfiguration
{
    public TransferMapMode MapMode;
    public int SuperSamplingSteps;
    public int ReservoirSmoothingKernelSize;
    public int ReservoirDiscardVolumeThreshold;
    public float EmitVolumeApplicationReservoir;
    public float EmitVolumePickupReservoir;
    public float PickupVolume;

    public TransferConfiguration()
    {
        MapMode = TransferMapMode.Bilinear;
        SuperSamplingSteps = 11;
        ReservoirSmoothingKernelSize = 1;
        ReservoirDiscardVolumeThreshold = 10;
        EmitVolumeApplicationReservoir = 1;
        EmitVolumePickupReservoir = 1.2f;
        PickupVolume = 1.3f;
    }
}