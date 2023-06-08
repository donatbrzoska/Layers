
public abstract class FloatValueSource
{
    public abstract void Update();
    public float Value { get; set; }
}


public abstract class StrokeStateSource
{
    public abstract void Update();
    public bool DrawingEnabled { get; protected set; }
    public bool StrokeBegin { get; protected set; }
    public bool InStroke { get; protected set; }
}


public enum InputSourceType
{
    Text,
    Mouse,
    Keyboard,
}

public struct InputValue
{
    public InputSourceType Source;
    public float Default;
}


public class InputManager
{
    public FloatValueSource RakelPositionXSource;
    public FloatValueSource RakelPositionYSource;
    public FloatValueSource RakelPositionZSource;
    public FloatValueSource RakelRotationSource;
    public FloatValueSource RakelTiltSource;

    public StrokeStateSource StrokeStateSource;


    public InputManager(InputConfiguration inputConfiguration)
    {
        switch (inputConfiguration.RakelPositionX.Source)
        {
            case InputSourceType.Text:
                RakelPositionXSource = new TextRakelPositionX(inputConfiguration.RakelPositionX.Default);
                break;
            case InputSourceType.Keyboard:
                break;
            case InputSourceType.Mouse:
                RakelPositionXSource = new MouseRakelPositionX();
                break;
            default:
                break;
        }
        switch (inputConfiguration.RakelPositionY.Source)
        {
            case InputSourceType.Text:
                RakelPositionYSource = new TextRakelPositionY(inputConfiguration.RakelPositionY.Default);
                break;
            case InputSourceType.Keyboard:
                break;
            case InputSourceType.Mouse:
                RakelPositionYSource = new MouseRakelPositionY();
                break;
            default:
                break;
        }
        switch (inputConfiguration.RakelPositionZ.Source)
        {
            case InputSourceType.Text:
                RakelPositionZSource = new TextRakelPositionZ(inputConfiguration.RakelPositionZ.Default);
                break;
            case InputSourceType.Keyboard:
                RakelPositionZSource = new KeyboardRakelPositionZ();
                break;
            case InputSourceType.Mouse:
                break;
            default:
                break;
        }
        switch (inputConfiguration.RakelRotation.Source)
        {
            case InputSourceType.Text:
                RakelRotationSource = new TextRakelRotation(inputConfiguration.RakelRotation.Default);
                break;
            case InputSourceType.Keyboard:
                break;
            case InputSourceType.Mouse:
                RakelRotationSource = new MouseRakelRotation();
                break;
            default:
                break;
        }
        switch (inputConfiguration.RakelTilt.Source)
        {
            case InputSourceType.Text:
                RakelTiltSource = new TextRakelTilt(inputConfiguration.RakelTilt.Default);
                break;
            case InputSourceType.Keyboard:
                RakelTiltSource = new KeyboardRakelTilt();
                break;
            case InputSourceType.Mouse:
                break;
            default:
                break;
        }

        switch (inputConfiguration.StrokeStateSource)
        {
            case InputSourceType.Text:
                break;
            case InputSourceType.Keyboard:
                break;
            case InputSourceType.Mouse:
                StrokeStateSource = new MouseStrokeState();
                break;
            default:
                break;
        }
    }


    public void Update()
    {
        RakelPositionXSource.Update();
        RakelPositionYSource.Update();
        RakelPositionZSource.Update();
        RakelRotationSource.Update();
        RakelTiltSource.Update();

        StrokeStateSource.Update();
    }

    public float RakelPositionX
    {
        get
        {
            return RakelPositionXSource.Value;
        }

        // this is only really used for text source
        set
        {
            RakelPositionXSource.Value = value;
        }
    }

    public float RakelPositionY
    {
        get
        {
            return RakelPositionYSource.Value;
        }

        // this is only really used for text source
        set
        {
            RakelPositionYSource.Value = value;
        }
    }

    public float RakelPositionZ
    {
        get
        {
            return RakelPositionZSource.Value;
        }

        // this is only really used for text source
        set
        {
            RakelPositionZSource.Value = value;
        }
    }

    public float RakelRotation
    {
        get
        {
            return RakelRotationSource.Value;
        }

        // this is only really used for text source
        set
        {
            RakelRotationSource.Value = value;
        }
    }

    public float RakelTilt
    {
        get
        {
            return RakelTiltSource.Value;
        }

        // this is only really used for text source
        set
        {
            RakelTiltSource.Value = value;
        }
    }


    public bool DrawingEnabled
    {
        get
        {
            return StrokeStateSource.DrawingEnabled;
        }
    }

    public bool StrokeBegin
    {
        get
        {
            return StrokeStateSource.StrokeBegin;
        }
    }

    public bool InStroke
    {
        get
        {
            return StrokeStateSource.InStroke;
        }
    }
}
