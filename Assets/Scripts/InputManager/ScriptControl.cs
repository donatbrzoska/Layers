using UnityEngine;

public class ScriptControl : MonoBehaviour,
    PositionXSink, PositionYSink, PositionBaseZSink, PressureSink, RotationSink, TiltSink,
    PositionXSource, PositionYSource, PositionBaseZSource, PressureSource, RotationSource, TiltSource
{
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float PositionBaseZ { get; set; }
    private float _Pressure;
    public float Pressure
    {
        get
        {
            return _Pressure;
        }

        set
        {
            _Pressure = Mathf.Clamp01(value);
        }
    }
    public float Rotation { get; set; }
    private float _Tilt;
    public float Tilt
    {
        get
        {
            return _Tilt;
        }

        set
        {
            _Tilt = Rakel.ClampTilt(value);
        }
    }

    public void Start()
    {
        PositionBaseZ = -4 * Paint.VOLUME_THICKNESS;
    }
}