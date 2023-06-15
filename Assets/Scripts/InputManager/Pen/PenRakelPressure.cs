using UnityEngine;
using UnityEngine.InputSystem;

public class PenRakelPressure : FloatValueSource
{
    public override void Update()
    {
        Value = Mathf.Clamp01(Pen.current.pressure.ReadValue());
    }
}
