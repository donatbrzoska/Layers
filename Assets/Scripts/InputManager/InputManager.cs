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
    Auto
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
    private FloatValueSource RakelPressureSource;
    private FloatValueSource RakelRotationSource;
    private FloatValueSource RakelTiltSource;

    private StrokeStateSource StrokeStateSource;

    private InputConfiguration InputConfig;

    public InputManager(InputConfiguration inputConfig)
    {
        InputConfig = inputConfig;

        switch (inputConfig.RakelPositionX.Source)
        {
            case InputSourceType.Text:
                RakelPositionXSource = new TextRakelPositionX();
                break;
            case InputSourceType.Mouse:
                RakelPositionXSource = new MouseRakelPositionX();
                break;
            case InputSourceType.Pen:
                RakelPositionXSource = new PenRakelPositionX();
                break;
            default:
                Debug.LogError(string.Format("Unsupported InputSourceType '{0}' for RakelPositionXSource", inputConfig.RakelPositionX.Source.ToString())); 
                break;
        }
        switch (inputConfig.RakelPositionY.Source)
        {
            case InputSourceType.Text:
                RakelPositionYSource = new TextRakelPositionY();
                break;
            case InputSourceType.Mouse:
                RakelPositionYSource = new MouseRakelPositionY();
                break;
            case InputSourceType.Pen:
                RakelPositionYSource = new PenRakelPositionY();
                break;
            default:
                Debug.LogError(string.Format("Unsupported InputSourceType '{0}' for RakelPositionYSource", inputConfig.RakelPositionY.Source.ToString()));
                break;
        }
        switch (inputConfig.RakelPositionZ.Source)
        {
            case InputSourceType.Text:
                RakelPositionZSource = new TextRakelPositionZ();
                break;
            case InputSourceType.Auto:
                RakelPositionZSource = new AutoRakelPositionZ();
                break;
            default:
                Debug.LogError(string.Format("Unsupported InputSourceType '{0}' for RakelPositionZSource", inputConfig.RakelPositionZ.Source.ToString()));
                break;
        }
        switch (inputConfig.RakelPressure.Source)
        {
            case InputSourceType.Text:
                RakelPressureSource = new TextRakelPressure();
                break;
            case InputSourceType.Keyboard:
                RakelPressureSource = new KeyboardRakelPressure();
                break;
            case InputSourceType.Pen:
                RakelPressureSource = new PenRakelPressure();
                break;
            default:
                Debug.LogError(string.Format("Unsupported InputSourceType '{0}' for RakelPressureSource", inputConfig.RakelPressure.Source.ToString()));
                break;
        }
        switch (inputConfig.RakelRotation.Source)
        {
            case InputSourceType.Text:
                RakelRotationSource = new TextRakelRotation();
                break;
            case InputSourceType.Mouse:
                RakelRotationSource = new MouseRakelRotation();
                break;
            case InputSourceType.Pen:
                RakelRotationSource = new PenRakelRotation();
                break;
            default:
                Debug.LogError(string.Format("Unsupported InputSourceType '{0}' for RakelRotationSource", inputConfig.RakelRotation.Source.ToString()));
                break;
        }
        switch (inputConfig.RakelTilt.Source)
        {
            case InputSourceType.Text:
                RakelTiltSource = new TextRakelTilt();
                break;
            case InputSourceType.Keyboard:
                RakelTiltSource = new KeyboardRakelTilt();
                break;
            default:
                Debug.LogError(string.Format("Unsupported InputSourceType '{0}' for RakelTiltSource", inputConfig.RakelTilt.Source.ToString()));
                break;
        }

        // HACK This is necessary for keeping the complete current configuration
        // Ideally we wouldn't need to reinitialize the entire InputManager every
        // time a value changes, but it works and there is no time to fix it
        RakelPositionXSource.Value = inputConfig.RakelPositionX.Value;
        RakelPositionYSource.Value = inputConfig.RakelPositionY.Value;
        RakelPositionZSource.Value = inputConfig.RakelPositionZ.Value;
        RakelPressureSource.Value = inputConfig.RakelPressure.Value;
        RakelRotationSource.Value = inputConfig.RakelRotation.Value;
        RakelTiltSource.Value = inputConfig.RakelTilt.Value;

        switch (inputConfig.StrokeStateSource)
        {
            case InputSourceType.Mouse:
                StrokeStateSource = new MouseStrokeState();
                break;
            case InputSourceType.Pen:
                StrokeStateSource = new PenStrokeState();
                break;
            default:
                Debug.LogError(string.Format("Unsupported InputSourceType '{0}' for StrokeStateSource", inputConfig.StrokeStateSource.ToString()));
                break;
        }
    }

    public void Update()
    {
        RakelPositionXSource.Update();
        InputConfig.RakelPositionX.Value = RakelPositionXSource.Value;
        RakelPositionYSource.Update();
        InputConfig.RakelPositionY.Value = RakelPositionYSource.Value;
        RakelPositionZSource.Update();
        InputConfig.RakelPositionZ.Value = RakelPositionZSource.Value;
        RakelPressureSource.Update();
        InputConfig.RakelPressure.Value = RakelPressureSource.Value;
        RakelRotationSource.Update();
        InputConfig.RakelRotation.Value = RakelRotationSource.Value;
        RakelTiltSource.Update();
        InputConfig.RakelTilt.Value = RakelTiltSource.Value;

        StrokeStateSource.Update();
    }

    public float RakelPositionX { get { return RakelPositionXSource.Value; } }
    public float RakelPositionY { get { return RakelPositionYSource.Value; } }
    public float RakelPositionZ { get { return RakelPositionZSource.Value; } }
    public float RakelPressure { get { return RakelPressureSource.Value; } }
    public float RakelRotation { get { return RakelRotationSource.Value; } }
    public float RakelTilt { get { return RakelTiltSource.Value; } }

    public bool DrawingEnabled { get { return StrokeStateSource.DrawingEnabled; } }
    public bool StrokeBegin { get { return StrokeStateSource.StrokeBegin; } }
    public bool InStroke { get { return StrokeStateSource.InStroke; } }
}
