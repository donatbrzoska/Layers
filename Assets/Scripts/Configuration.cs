using UnityEngine;

public class Configuration
{
    public int TextureResolution;
    public float NormalScale;

    public InputConfiguration InputConfiguration;
    public RakelConfiguration RakelConfiguration;
    public FillConfiguration FillConfiguration;
    public TransferConfiguration TransferConfiguration;

    public Configuration()
    {
        TextureResolution = 40;
        NormalScale = 0.015f;

        InputConfiguration = new InputConfiguration();
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

        TransferConfiguration.EmitVolumeApplicationReservoir_MAX = 1;
        TransferConfiguration.EmitVolumePickupReservoir_MAX = 0;
        TransferConfiguration.PickupVolume_MAX = 0;
    }

    public void LoadBenchmark()
    {
        TextureResolution = 50;

        RakelConfiguration.Length = 1;
        RakelConfiguration.Width = 1;
    }

    public void LoadPixelMapping()
    {
        TextureResolution = 80;

        InputConfiguration.RakelRotation.Source = InputSourceType.Mouse;

        TransferConfiguration.EmitVolumePickupReservoir_MAX = 0;
        TransferConfiguration.PickupVolume_MAX = 0;

        FillConfiguration.Color = Color_.CadmiumGreen;
        FillConfiguration.Volume = 300;
        FillConfiguration.VolumeMode = VolumeMode.Flat;
    }

    public void LoadPresentation()
    {
        TextureResolution = 80;

        InputConfiguration.RakelRotation.Source = InputSourceType.Mouse;

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

public class InputConfiguration
{
    public InputValue RakelPositionX;
    public InputValue RakelPositionY;
    public InputValue RakelPositionZ;

    public InputValue RakelRotation;
    public InputValue RakelTilt;

    public InputSourceType StrokeStateSource;

    public InputConfiguration()
    {
        RakelPositionX = new InputValue() { Source = InputSourceType.Mouse, Default = 0 };
        RakelPositionY = new InputValue() { Source = InputSourceType.Mouse, Default = 0 };
        RakelPositionZ = new InputValue() { Source = InputSourceType.Text, Default = -0.05f };

        RakelRotation = new InputValue() { Source = InputSourceType.Text, Default = 0 };
        RakelTilt = new InputValue() { Source = InputSourceType.Keyboard, Default = 0 };

        StrokeStateSource = InputSourceType.Mouse;
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
    public float EmitDistance_MAX;
    public float PickupDistance_MAX;
    public float EmitVolumeApplicationReservoir_MIN;
    public float EmitVolumeApplicationReservoir_MAX;
    public float EmitVolumePickupReservoir_MIN;
    public float EmitVolumePickupReservoir_MAX;
    public float PickupVolume_MIN;
    public float PickupVolume_MAX;

    public TransferConfiguration()
    {
        EmitDistance_MAX = 0.1f;
        PickupDistance_MAX = EmitDistance_MAX;

        EmitVolumeApplicationReservoir_MAX = 1;
        EmitVolumePickupReservoir_MAX = 1.2f;
        PickupVolume_MAX = 1.3f;
    }
}