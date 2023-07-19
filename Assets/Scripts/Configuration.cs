using UnityEngine;

public class Configuration
{
    private int TextureResolution_;
    public int TextureResolution
    {
        get
        {
            return TextureResolution_;
        }
        set
        {
            TextureResolution_ = value;
            CanvasConfig = new CanvasConfiguration(TextureResolution_);
            RakelConfig = new RakelConfiguration(TextureResolution_);
        }
    }
    public ColorSpace ColorSpace;

    public InputConfiguration InputConfig;
    public CanvasConfiguration CanvasConfig;
    public RakelConfiguration RakelConfig;
    public FillConfiguration FillConfig;
    public TransferConfiguration TransferConfig;

    public Configuration()
    {
        TextureResolution_ = 40;
        ColorSpace = ColorSpace.RGB;

        InputConfig = new InputConfiguration();
        CanvasConfig = new CanvasConfiguration(TextureResolution_);
        RakelConfig = new RakelConfiguration(TextureResolution_);
        FillConfig = new FillConfiguration();
        TransferConfig = new TransferConfiguration();
    }

    public void LoadBenchmark()
    {
        TextureResolution = 50;

        RakelConfig.Length = 1;
        RakelConfig.Width = 1;
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
        RakelPositionZ = new InputValue() { Source = InputSourceType.Auto, Value = -0.004f };
        RakelPressure = new InputValue() { Source = InputSourceType.Keyboard, Value = 0 };

        RakelRotation = new InputValue() { Source = InputSourceType.Text, Value = 0 };
        RakelTilt = new InputValue() { Source = InputSourceType.Keyboard, Value = 0 };

        StrokeStateSource = InputSourceType.Mouse;
    }
}

public class CanvasConfiguration
{
    private const float MAX_WIDTH = 15;
    private const float MAX_HEIGHT = 10;

    public int FormatA;
    public int FormatB;
    private int Resolution;

    public float Width
    {
        get
        {
            float ratio = (float)FormatA / FormatB;
            if (ratio < MAX_WIDTH / MAX_HEIGHT)
            {
                float height = MAX_HEIGHT;
                float width = height * ratio;
                return width;
            }
            else
            {
                return MAX_WIDTH;
            }
        }
    }

    public float Height
    {
        get
        {
            float ratio = (float)FormatA / FormatB;
            if (ratio < MAX_WIDTH / MAX_HEIGHT)
            {
                return MAX_HEIGHT;
            }
            else
            {
                float width = MAX_WIDTH;
                float height = width / ratio;
                return height;
            }
        }
    }

    public float NormalScale;
    public float CellVolume;
    public int DiffuseDepth;
    public float DiffuseRatio;

    public CanvasConfiguration(int resolution)
    {
        FormatA = 3;
        FormatB = 2;
        Resolution = resolution;

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
    public int Resolution;
    public float CellVolume;
    public int DiffuseDepth;
    public float DiffuseRatio;

    public bool TiltNoiseEnabled;
    public float TiltNoiseFrequency;
    public float TiltNoiseAmplitude;

    public RakelConfiguration(int resolution)
    {
        Length = 2f;
        Width = 0.8f;
        Resolution = resolution;
        CellVolume = 2;
        DiffuseDepth = 0;
        DiffuseRatio = 0.2f;

        TiltNoiseEnabled = true;
        TiltNoiseFrequency = 45;
        TiltNoiseAmplitude = 1;
    }
}

public class FillConfiguration
{
    public Color_ Color;
    public ColorMode ColorMode;
    public float WidthPart;
    public int Volume;
    public VolumeMode VolumeMode;

    public FillConfiguration()
    {
        Color = Color_.LavenderLight;
        ColorMode = ColorMode.Flat;
        WidthPart = 0.6f;
        Volume = 60;
        VolumeMode = VolumeMode.Perlin;
    }

}

public class TransferConfiguration
{
    public bool ReadjustZToRakelVolume;
    public bool ReadjustZToCanvasVolume;

    public bool CanvasSnapshotBufferEnabled;
    public bool DeletePickedUpFromCSB;

    public float EmitDistance_MAX;
    public float PickupDistance_MAX;

    public float EmitVolume_MIN;
    public float PickupVolume_MIN;

    public bool PaintDoesPickup;

    public float LayerThickness_MAX;

    public float BaseSink_MAX;
    public float LayerSink_MAX_Ratio;
    public float TiltSink_MAX;

    public TransferConfiguration()
    {
        ReadjustZToRakelVolume = true;
        ReadjustZToCanvasVolume = true;

        CanvasSnapshotBufferEnabled = true;
        DeletePickedUpFromCSB = true;

        // This is basically 1:1 the max distance at which the paint "gravity" acts.
        // (Paint being transferred, even though surfaces don't really touch. For details,
        // see comment in VolumeToPickup.compute)
        // 0.1 is the value so that there is pickup, even with resolution 10 and 79° tilt
        PickupDistance_MAX = 0.1f;
        EmitDistance_MAX = 0;

        EmitVolume_MIN = 0.1f;
        PickupVolume_MIN = 0.1f;

        PaintDoesPickup = false;

        LayerThickness_MAX = 4 * Paint.VOLUME_THICKNESS;

        BaseSink_MAX = 1 * Paint.VOLUME_THICKNESS;
        LayerSink_MAX_Ratio = 1;
        TiltSink_MAX = 6 * Paint.VOLUME_THICKNESS;
    }
}