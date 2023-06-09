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
    public float Value;
}


public class InputManager
{
    private FloatValueSource RakelPositionXSource;
    private FloatValueSource RakelPositionYSource;
    private FloatValueSource RakelPositionZSource;
    private FloatValueSource RakelRotationSource;
    private FloatValueSource RakelTiltSource;

    private StrokeStateSource StrokeStateSource;

    private InputConfiguration InputConfiguration;

    public InputManager(InputConfiguration inputConfiguration, float RakelPositionZ_MIN, float RakelPositionZ_MAX)
    {
        InputConfiguration = inputConfiguration;

        switch (inputConfiguration.RakelPositionX.Source)
        {
            case InputSourceType.Text:
                RakelPositionXSource = new TextRakelPositionX();
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
                RakelPositionYSource = new TextRakelPositionY();
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
                RakelPositionZSource = new TextRakelPositionZ();
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
                RakelRotationSource = new TextRakelRotation();
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
                RakelTiltSource = new TextRakelTilt();
                break;
            case InputSourceType.Keyboard:
                RakelTiltSource = new KeyboardRakelTilt();
                break;
            default:
                Debug.LogError(string.Format("Unsupported InputSourceType '{0}' for RakelTiltSource", inputConfiguration.RakelTilt.Source.ToString()));
                break;
        }

        // HACK This is necessary for keeping the complete current configuration
        // Ideally we wouldn't need to reinitialize the entire InputManager every
        // time a value changes, but it works and there is no time to fix it
        RakelPositionXSource.Value = inputConfiguration.RakelPositionX.Value;
        RakelPositionYSource.Value = inputConfiguration.RakelPositionY.Value;
        RakelPositionZSource.Value = inputConfiguration.RakelPositionZ.Value;
        RakelRotationSource.Value = inputConfiguration.RakelRotation.Value;
        RakelTiltSource.Value = inputConfiguration.RakelTilt.Value;

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
        InputConfiguration.RakelPositionX.Value = RakelPositionXSource.Value;
        RakelPositionYSource.Update();
        InputConfiguration.RakelPositionY.Value = RakelPositionYSource.Value;
        RakelPositionZSource.Update();
        InputConfiguration.RakelPositionZ.Value = RakelPositionZSource.Value;
        RakelRotationSource.Update();
        InputConfiguration.RakelRotation.Value = RakelRotationSource.Value;
        RakelTiltSource.Update();
        InputConfiguration.RakelTilt.Value = RakelTiltSource.Value;

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
