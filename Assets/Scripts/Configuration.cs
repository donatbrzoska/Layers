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
    float MAX_SUPPORTED_LENGTH = 15;
    float MAX_SUPPORTED_WIDTH = 2;

    private float length;
    public float Length
    {
        get
        {
            return length;
        }
        set
        {
            length = Mathf.Min(value, MAX_SUPPORTED_LENGTH);
        }
    }

    private float width;
    public float Width
    {
        get
        {
            return width;
        }
        set
        {
            width = Mathf.Min(value, MAX_SUPPORTED_WIDTH);
        }
    }
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
        Color = Color_.CadmiumLightGreen;
        ColorMode = ColorMode.Flat;
        WidthPart = 0.6f;
        Volume = 60;
        VolumeMode = VolumeMode.Perlin;
    }

}

public class TransferConfiguration
{
    public ReduceFunction CanvasVolumeReduceFunction;
    public ReduceFunction RakelVolumeReduceFunction;
    public bool ReadjustZToRakelVolume;
    public bool ReadjustZToCanvasVolume;
    public float FloatingZLength;

    public bool CanvasSnapshotBufferEnabled;
    public bool DeletePickedUpFromCSB;

    public float EmitDistance_MAX;
    public float PickupDistance_MAX;

    private float emitVolume_MIN_NoCSB;
    private float emitVolume_MIN_CSB;
    private float emitVolume_MIN_CSBDelete;
    public float EmitVolume_MIN
    {
        get
        {
            if (CanvasSnapshotBufferEnabled)
            {
                if (DeletePickedUpFromCSB)
                {
                    return emitVolume_MIN_CSBDelete;
                }
                else
                {
                    return emitVolume_MIN_CSB;
                }
            }
            else
            {
                return emitVolume_MIN_NoCSB;
            }
        }
        set
        {
            if (CanvasSnapshotBufferEnabled)
            {
                if (DeletePickedUpFromCSB)
                {
                    emitVolume_MIN_CSBDelete = value;
                }
                else
                {
                    emitVolume_MIN_CSB = value;
                }
            }
            else
            {
                emitVolume_MIN_NoCSB = value;
            }
        }
    }
    private float pickupVolume_MIN_NoCSB;
    private float pickupVolume_MIN_CSB;
    private float pickupVolume_MIN_CSBDelete;
    public float PickupVolume_MIN
    {
        get
        {
            if (CanvasSnapshotBufferEnabled)
            {
                if (DeletePickedUpFromCSB)
                {
                    return pickupVolume_MIN_CSBDelete;
                }
                else
                {
                    return pickupVolume_MIN_CSB;
                }
            }
            else
            {
                return pickupVolume_MIN_NoCSB;
            }
        }
        set
        {
            if (CanvasSnapshotBufferEnabled)
            {
                if (DeletePickedUpFromCSB)
                {
                    pickupVolume_MIN_CSBDelete = value;
                }
                else
                {
                    pickupVolume_MIN_CSB = value;
                }
            }
            else
            {
                pickupVolume_MIN_NoCSB = value;
            }
        }
    }

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
        CanvasVolumeReduceFunction = ReduceFunction.Avg;
        RakelVolumeReduceFunction = ReduceFunction.Avg;
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

        emitVolume_MIN_NoCSB = 0.1f;
        pickupVolume_MIN_NoCSB = 0.1f;
        emitVolume_MIN_CSB = 0.1f;
        pickupVolume_MIN_CSB = 0;
        emitVolume_MIN_CSBDelete = 0.2f;
        pickupVolume_MIN_CSBDelete = 0;

        PaintDoesPickup = true;

        RakelDiffuseDepth = 0;
        RakelDiffuseRatio = 0.2f;
        CanvasDiffuseDepth = 0;
        CanvasDiffuseRatio = 0.2f;

        LayerThickness_MAX_Volume = 4;
        TiltAdjustLayerThickness = true;

        BaseSink_MAX_Volume = 1;
        LayerSink_MAX_Ratio = 0.9f;
        TiltSink_MAX_Volume = 6;
    }
}