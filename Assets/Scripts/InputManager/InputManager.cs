using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public GameObject _ScriptControl;
    public GameObject _KeyboardControl;
    public GameObject _MouseControl;
    public GameObject _PenControl;

    private ScriptControl ScriptControl;
    private KeyboardControl KeyboardControl;
    private MouseControl MouseControl;
    private PenControl PenControl;

    private PositionXSource PositionXSource;
    private PositionYSource PositionYSource;
    private PositionBaseZSource PositionBaseZSource;
    private PressureSource PressureSource;
    private RotationSource RotationSource;
    private TiltSource TiltSource;
    private StrokeStateSource StrokeStateSource;

    private const int EMPTY_POSITION_BASE_Z = 0;

    void Awake()
    {
        ScriptControl = _ScriptControl.GetComponent<ScriptControl>();
        KeyboardControl = _KeyboardControl.GetComponent<KeyboardControl>();
        MouseControl = _MouseControl.GetComponent<MouseControl>();
        PenControl = _PenControl.GetComponent<PenControl>();

        PositionXSource = MouseControl;
        PositionYSource = MouseControl;
        PositionBaseZSource = null; // = auto
        PressureSource = KeyboardControl;
        RotationSource = ScriptControl;
        TiltSource = KeyboardControl;
        StrokeStateSource = MouseControl;
    }

    void Update()
    {
        if (PenControl.ChangeDetected && !PenControl.Active)
        {
            MouseControl.Active = false;
            PenControl.Active = true;
            PositionXSource = PenControl;
            PositionYSource = PenControl;
            PressureSource = PenControl;
            if (ReferenceEquals(RotationSource, MouseControl))
            {
                RotationSource = PenControl;
            }
            StrokeStateSource = PenControl;
        }

        if (MouseControl.ChangeDetected && !MouseControl.Active)
        {
            PenControl.Active = false;
            MouseControl.Active = true;
            PositionXSource = MouseControl;
            PositionYSource = MouseControl;
            PressureSource = KeyboardControl;
            if (ReferenceEquals(RotationSource, PenControl))
            {
                RotationSource = MouseControl;
            }
            StrokeStateSource = MouseControl;
        }
    }

    public float PositionX { get { return PositionXSource.PositionX; } }
    public float PositionY { get { return PositionYSource.PositionY; } }
    public bool PositionAutoBaseZEnabled { get { return PositionBaseZSource == null; } }
    public float PositionBaseZ { get { return PositionAutoBaseZEnabled ? EMPTY_POSITION_BASE_Z : PositionBaseZSource.PositionBaseZ; } }
    public float Pressure { get { return PressureSource.Pressure; } }
    public float Rotation { get { return RotationSource.Rotation; } }
    public float Tilt { get { return TiltSource.Tilt; } }
    public bool StrokeBegin { get { return StrokeStateSource.StrokeBegin; } }
    public bool InStroke { get { return StrokeStateSource.InStroke; } }

    // ****************************************************************************************
    // ***                             UI AND MAIN SCRIPTING API                            ***
    // ****************************************************************************************

    public bool UsingScriptPositionX { get { return ReferenceEquals(PositionXSource, ScriptControl); } }
    public bool UsingScriptPositionY { get { return ReferenceEquals(PositionYSource, ScriptControl); } }
    public bool UsingScriptPositionBaseZ { get { return ReferenceEquals(PositionBaseZSource, ScriptControl); } }
    public bool UsingScriptPressure { get { return ReferenceEquals(PressureSource, ScriptControl); } }
    public bool UsingScriptRotation { get { return ReferenceEquals(RotationSource, ScriptControl); } }
    public bool UsingScriptTilt { get { return ReferenceEquals(TiltSource, ScriptControl); } }

    public void UpdateUsingScriptPositionX(bool usingScriptPositionX)
    {
        if (usingScriptPositionX)
        {
            ScriptControl.PositionX = PositionX;
            PositionXSource = ScriptControl;
        }
        else if (PenControl.Active)
        {
            PositionXSource = PenControl;
        }
        else
        {
            PositionXSource = MouseControl;
        }
    }

    public void UpdateUsingScriptPositionY(bool usingScriptPositionY)
    {
        if (usingScriptPositionY)
        {
            ScriptControl.PositionY = PositionY;
            PositionYSource = ScriptControl;
        }
        else if (PenControl.Active)
        {
            PositionYSource = PenControl;
        }
        else
        {
            PositionYSource = MouseControl;
        }
    }

    public void UpdateUsingScriptPositionBaseZ(bool usingScriptPositionBaseZ)
    {
        if (usingScriptPositionBaseZ)
        {
            ScriptControl.PositionBaseZ = PositionBaseZ;
            PositionBaseZSource = ScriptControl;
        }
        else
        {
            PositionBaseZSource = null;
        }
    }

    public void UpdateUsingScriptPressure(bool usingScriptPressure)
    {
        if (usingScriptPressure)
        {
            ScriptControl.Pressure = Pressure;
            PressureSource = ScriptControl;
        }
        else if (PenControl.Active)
        {
            PressureSource = PenControl;
        }
        else
        {
            PressureSource = KeyboardControl;
        }
    }

    public void UpdateUsingScriptRotation(bool usingScriptRotation)
    {
        if (usingScriptRotation)
        {
            ScriptControl.Rotation = Rotation;
            RotationSource = ScriptControl;
        }
        else if (PenControl.Active)
        {
            RotationSource = PenControl;
        }
        else
        {
            RotationSource = MouseControl;
        }
    }

    public void UpdateUsingScriptTilt(bool usingScriptTilt)
    {
        if (usingScriptTilt)
        {
            ScriptControl.Tilt = Tilt;
            TiltSource = ScriptControl;
        }
        else
        {
            TiltSource = KeyboardControl;
        }
    }
}