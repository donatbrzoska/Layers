using UnityEngine;

public class Configuration
{
    public int TextureResolution;

    public float RakelRotation;
    public bool RakelRotationLocked;

    public RakelConfiguration RakelConfiguration;
    public FillConfiguration FillConfiguration;
    public TransferConfiguration TransferConfiguration;

    public Configuration()
    {
        TextureResolution = 20;

        RakelRotation = 0;
        RakelRotationLocked = true;

        RakelConfiguration = new RakelConfiguration();
        FillConfiguration = new FillConfiguration();
        TransferConfiguration = new TransferConfiguration();
    }

    public void LoadDebug()
    {
        TextureResolution = 1;

        RakelConfiguration.Length = 4;
        RakelConfiguration.Width = 2;

        FillConfiguration.Volume = 1;
        FillConfiguration.VolumeMode = VolumeMode.Flat;
        FillConfiguration.ColorMode = ColorMode.Colorful;

        TransferConfiguration.MapMode = TransferMapMode.PolygonClipping;
        TransferConfiguration.EmitVolumeApplicationReservoir = 1;
        TransferConfiguration.EmitVolumePickupReservoir = 0;
        TransferConfiguration.PickupVolume = 0;
    }

    public void LoadBenchmark()
    {
        TextureResolution = 50;

        RakelConfiguration.Length = 1;
        RakelConfiguration.Width = 1;

        TransferConfiguration.MapMode = TransferMapMode.NearestNeighbour;
    }

    public void LoadPixelMapping()
    {
        TextureResolution = 80;

        RakelRotationLocked = false;

        TransferConfiguration.MapMode = TransferMapMode.NearestNeighbour;
        TransferConfiguration.EmitVolumePickupReservoir = 0;
        TransferConfiguration.PickupVolume = 0;

        FillConfiguration.Color = Color_.CadmiumGreen;
        FillConfiguration.Volume = 300;
        FillConfiguration.VolumeMode = VolumeMode.Flat;
    }

    public void LoadPresentation()
    {
        TextureResolution = 80;

        RakelRotationLocked = false;

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
        TextureResolution = 80;
        FillConfiguration.Volume = 40;
        FillConfiguration.VolumeMode = VolumeMode.Flat;
    }
}

public class RakelConfiguration
{
    public float Length;
    public float Width;

    public RakelConfiguration()
    {
        Length = 2.5f;
        Width = 0.5f;
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