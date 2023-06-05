using UnityEngine;

public class Configuration
{
    public int TextureResolution;
    public float NormalScale;

    public RakelConfiguration RakelConfiguration;
    public FillConfiguration FillConfiguration;
    public TransferConfiguration TransferConfiguration;

    public Configuration()
    {
        TextureResolution = 20;
        NormalScale = 0.015f;

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

        RakelConfiguration.RotationLocked = false;

        TransferConfiguration.EmitVolumePickupReservoir_MAX = 0;
        TransferConfiguration.PickupVolume_MAX = 0;

        FillConfiguration.Color = Color_.CadmiumGreen;
        FillConfiguration.Volume = 300;
        FillConfiguration.VolumeMode = VolumeMode.Flat;
    }

    public void LoadPresentation()
    {
        TextureResolution = 80;

        RakelConfiguration.RotationLocked = false;

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
    public class Bool3
    {
        public bool x;
        public bool y;
        public bool z;

        public override string ToString()
        {
            return base.ToString() + string.Format(": ({0}, {1}, {2})", x, y, z);
        }
    }

    public float Length;
    public float Width;

    public Vector3 Position;
    public Bool3 PositionLocked;

    public float Rotation;
    public bool RotationLocked;

    public float Tilt;
    public bool TiltLocked;

    public RakelConfiguration()
    {
        Position = Vector3.zero;
        PositionLocked = new Bool3();
        Position.z = 0.05f;
        PositionLocked.z = true;

        Rotation = 0;
        RotationLocked = true;

        Tilt = 0;
        TiltLocked = false;

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