using UnityEngine;
using UnityEngine.InputSystem;

public class PenRakelPositionZ : FloatValueSource
{
    private float Z_MIN;
    private float Z_MAX;

    public PenRakelPositionZ(float z_MIN, float z_MAX)
    {
        Z_MIN = z_MIN;
        Z_MAX = z_MAX;
    }

    public override void Update()
    {
        float pressure = Mathf.Clamp01(Pen.current.pressure.ReadValue());
        Value = Z_MIN + (1 - pressure) * (Z_MAX - Z_MIN);
    }
}
