using UnityEngine;

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
    Pen,
}

public struct InputValue
{
    public InputSourceType Source;
    public float Default;
}


public class InputManager
{
    private FloatValueSource RakelPositionXSource;
    private FloatValueSource RakelPositionYSource;
    private FloatValueSource RakelPositionZSource;
    private FloatValueSource RakelRotationSource;
    private FloatValueSource RakelTiltSource;

    private StrokeStateSource StrokeStateSource;

    public InputManager(InputConfiguration inputConfiguration, float RakelPositionZ_MIN, float RakelPositionZ_MAX)
    {
        switch (inputConfiguration.RakelPositionX.Source)
        {
            case InputSourceType.Text:
                RakelPositionXSource = new TextRakelPositionX(inputConfiguration.RakelPositionX.Default);
                break;
            case InputSourceType.Mouse:
                RakelPositionXSource = new MouseRakelPositionX();
                break;
            default:
                Debug.LogError(string.Format("Unsupported InputSourceType '{0}' for RakelPositionXSource", inputConfiguration.RakelPositionX.Source.ToString())); 
                break;
        }
        switch (inputConfiguration.RakelPositionY.Source)
        {
            case InputSourceType.Text:
                RakelPositionYSource = new TextRakelPositionY(inputConfiguration.RakelPositionY.Default);
                break;
            case InputSourceType.Mouse:
                RakelPositionYSource = new MouseRakelPositionY();
                break;
            default:
                Debug.LogError(string.Format("Unsupported InputSourceType '{0}' for RakelPositionYSource", inputConfiguration.RakelPositionY.Source.ToString()));
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
            case InputSourceType.Pen:
                RakelPositionZSource = new PenRakelPositionZ(RakelPositionZ_MIN, RakelPositionZ_MAX);
                break;
            default:
                Debug.LogError(string.Format("Unsupported InputSourceType '{0}' for RakelPositionZSource", inputConfiguration.RakelPositionZ.Source.ToString()));
                break;
        }
        switch (inputConfiguration.RakelRotation.Source)
        {
            case InputSourceType.Text:
                RakelRotationSource = new TextRakelRotation(inputConfiguration.RakelRotation.Default);
                break;
            case InputSourceType.Mouse:
                RakelRotationSource = new MouseRakelRotation();
                break;
            default:
                Debug.LogError(string.Format("Unsupported InputSourceType '{0}' for RakelRotationSource", inputConfiguration.RakelRotation.Source.ToString()));
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
            default:
                Debug.LogError(string.Format("Unsupported InputSourceType '{0}' for RakelTiltSource", inputConfiguration.RakelTilt.Source.ToString()));
                break;
        }

        switch (inputConfiguration.StrokeStateSource)
        {
            case InputSourceType.Mouse:
                StrokeStateSource = new MouseStrokeState();
                break;
            default:
                Debug.LogError(string.Format("Unsupported InputSourceType '{0}' for StrokeStateSource", inputConfiguration.StrokeStateSource.ToString()));
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

    public float RakelPositionX { get { return RakelPositionXSource.Value; } }
    public float RakelPositionY { get { return RakelPositionYSource.Value; } }
    public float RakelPositionZ { get { return RakelPositionZSource.Value; } }
    public float RakelRotation { get { return RakelRotationSource.Value; } }
    public float RakelTilt { get { return RakelTiltSource.Value; } }

    public bool DrawingEnabled { get { return StrokeStateSource.DrawingEnabled; } }
    public bool StrokeBegin { get { return StrokeStateSource.StrokeBegin; } }
    public bool InStroke { get { return StrokeStateSource.InStroke; } }
}
