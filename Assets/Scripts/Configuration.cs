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
        //RakelPositionZ = new InputValue() { Source = InputSourceType.Text, Value = -4 * Paint.VOLUME_THICKNESS };
        RakelPositionZ = new InputValue() { Source = InputSourceType.Auto, Value = -4 * Paint.VOLUME_THICKNESS };
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

    public CanvasConfiguration()
    {
        FormatA = 3;
        FormatB = 2;

        NormalScale = 0.015f;
        CellVolume = 1;
    }
}

public class RakelConfiguration
{
    public float Length;
    public float Width;
    public float CellVolume;

    public bool TiltNoiseEnabled;
    public float TiltNoiseFrequency;
    public float TiltNoiseAmplitude;

    public RakelConfiguration()
    {
        Length = 2f;
        Width = 0.8f;
        CellVolume = 2;

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
    public float FloatingZLength;

    public bool CanvasSnapshotBufferEnabled;
    public bool DeletePickedUpFromCSB;

    public float EmitDistance_MAX;
    public float PickupDistance_MAX;

    public float EmitVolume_MIN;
    public float PickupVolume_MIN;

    public bool PaintDoesPickup;

    public int RakelDiffuseDepth;
    public float RakelDiffuseRatio;
    public int CanvasDiffuseDepth;
    public float CanvasDiffuseRatio;

    public float LayerThickness_MAX_Volume;
    public float LayerThickness_MAX
    {
        get
        {
            return LayerThickness_MAX_Volume * Paint.VOLUME_THICKNESS;
        }
    }
    public bool TiltAdjustLayerThickness;

    public float BaseSink_MAX_Volume;
    public float BaseSink_MAX
    {
        get
        {
            return BaseSink_MAX_Volume * Paint.VOLUME_THICKNESS;
        }
    }
    public float LayerSink_MAX_Ratio;
    public float TiltSink_MAX_Volume;
    public float TiltSink_MAX
    {
        get
        {
            return TiltSink_MAX_Volume * Paint.VOLUME_THICKNESS;
        }
    }

    public TransferConfiguration()
    {
        ReadjustZToRakelVolume = true;
        ReadjustZToCanvasVolume = true;
        FloatingZLength = 0.5f;

        CanvasSnapshotBufferEnabled = true;
        DeletePickedUpFromCSB = false;

        // This is basically 1:1 the max distance at which the paint "gravity" acts.
        // (Paint being transferred, even though surfaces don't really touch. For details,
        // see comment in VolumeToPickup.compute)
        // 0.1 is the value so that there is pickup, even with resolution 10 and 79° tilt
        PickupDistance_MAX = 0.1f;
        EmitDistance_MAX = 0;

        EmitVolume_MIN = 0.1f;
        PickupVolume_MIN = 0;

        PaintDoesPickup = false;

        RakelDiffuseDepth = 0;
        RakelDiffuseRatio = 0.2f;
        CanvasDiffuseDepth = 0;
        CanvasDiffuseRatio = 0.2f;

        LayerThickness_MAX_Volume = 4;
        TiltAdjustLayerThickness = true;

        BaseSink_MAX_Volume = 6;
        LayerSink_MAX_Ratio = 0.9f;
        TiltSink_MAX_Volume = 6;
    }
}