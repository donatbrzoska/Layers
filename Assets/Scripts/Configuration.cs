using UnityEngine;

public class Configuration
{
    public int TextureResolution;
    public ColorSpace ColorSpace;

    public InputConfiguration InputConfig;
    public CanvasConfiguration CanvasConfig;
    public RakelConfiguration RakelConfig;
    public FillConfiguration FillConfig;
    public TransferConfiguration TransferConfig;

    public Configuration()
    {
        TextureResolution = 40;
        ColorSpace = ColorSpace.RGB;

        InputConfig = new InputConfiguration();
        CanvasConfig = new CanvasConfiguration();
        RakelConfig = new RakelConfiguration();
        FillConfig = new FillConfiguration();
        TransferConfig = new TransferConfiguration();
    }

    public void LoadDebug()
    {
        TextureResolution = 1;

        RakelConfig.Length = 4;
        RakelConfig.Width = 2;

        FillConfig.Volume = 1;
        FillConfig.VolumeMode = VolumeMode.Flat;
        FillConfig.ColorMode = ColorMode.Colorful;

        TransferConfig.EmitVolumeApplicationReservoirRate = 1; // unused
        TransferConfig.EmitVolumePickupReservoirRate = 0; // unused
        TransferConfig.PickupVolume_MAX = 0;
    }

    public void LoadBenchmark()
    {
        TextureResolution = 50;

        RakelConfig.Length = 1;
        RakelConfig.Width = 1;
    }

    public void LoadPixelMapping()
    {
        TextureResolution = 80;

        InputConfig.RakelRotation.Source = InputSourceType.Mouse;

        TransferConfig.EmitVolumePickupReservoirRate = 0;
        TransferConfig.PickupVolume_MAX = 0;

        FillConfig.Color = Color_.CadmiumGreen;
        FillConfig.Volume = 300;
        FillConfig.VolumeMode = VolumeMode.Flat;
    }

    public void LoadPresentation()
    {
        TextureResolution = 80;

        InputConfig.RakelRotation.Source = InputSourceType.Mouse;

        FillConfig.Color = Color_.DarkRed;
        FillConfig.Volume = 600;
        FillConfig.VolumeMode = VolumeMode.Perlin;

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
        FillConfig.Volume = 40;
        FillConfig.VolumeMode = VolumeMode.Flat;
    }
}

public class InputConfiguration
{
    public InputValue RakelPositionX;
    public InputValue RakelPositionY;
    public InputValue RakelPositionZ;
    public InputValue RakelPressure;

    public InputValue RakelRotation;
    public InputValue RakelTilt;

    public InputSourceType StrokeStateSource;

    public InputConfiguration()
    {
        RakelPositionX = new InputValue() { Source = InputSourceType.Mouse, Value = 0 };
        RakelPositionY = new InputValue() { Source = InputSourceType.Mouse, Value = 0 };
        //RakelPositionZ = new InputValue() { Source = InputSourceType.Text, Value = -0.004f };
        RakelPositionZ = new InputValue() { Source = InputSourceType.Auto, Value = 0 };
        RakelPressure = new InputValue() { Source = InputSourceType.Keyboard, Value = 0 };

        RakelRotation = new InputValue() { Source = InputSourceType.Text, Value = 0 };
        RakelTilt = new InputValue() { Source = InputSourceType.Keyboard, Value = 0 };

        StrokeStateSource = InputSourceType.Mouse;
    }
}

public class CanvasConfiguration
{
    public float NormalScale;
    public float CellVolume;
    public int DiffuseDepth;
    public float DiffuseRatio;

    public CanvasConfiguration()
    {
        NormalScale = 0.015f;
        CellVolume = 1;
        DiffuseDepth = 0;
        DiffuseRatio = 0.2f;
    }
}

public class RakelConfiguration
{
    public float Length;
    public float Width;
    public float CellVolume;
    public int DiffuseDepth;
    public float DiffuseRatio;

    public RakelConfiguration()
    {
        Length = 2.5f;
        Width = 0.5f;
        CellVolume = 4;
        DiffuseDepth = 0;
        DiffuseRatio = 0.2f;
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
        Color = Color_.UltramarineBlueRAL;
        ColorMode = ColorMode.Flat;
        Volume = 60;
        VolumeMode = VolumeMode.Perlin;
    }

}

public class TransferConfiguration
{
    public float EmitDistance_MAX;
    public float PickupDistance_MAX;
    public float EmitVolume_MIN;
    public float EmitVolume_MAX;
    public float EmitVolumeApplicationReservoirRate;
    public float EmitVolumePickupReservoirRate;
    public float PickupVolume_MIN;
    public float PickupVolume_MAX;
    public float LayerThickness_MAX;

    public float BaseSink_MAX_Ratio;
    public float TiltSink_MAX;

    public TransferConfiguration()
    {
        EmitDistance_MAX = 0.1f;
        PickupDistance_MAX = EmitDistance_MAX;

        EmitVolume_MIN = 0.1f;
        EmitVolume_MAX = 1;

        EmitVolumeApplicationReservoirRate = 1;
        EmitVolumePickupReservoirRate = 1.2f;
        PickupVolume_MIN = 0.1f;// 0.3f;
        PickupVolume_MAX = 0.65f;

        BaseSink_MAX_Ratio = 1;
        LayerThickness_MAX = 4 * Paint.VOLUME_THICKNESS;

        TiltSink_MAX = 4 * Paint.VOLUME_THICKNESS;
    }
}