using UnityEngine;

public class KeyboardRakelPressure : FloatValueSource
{
    private const float PRESSURE_STEP = 0.01f;

    public override void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            Value -= PRESSURE_STEP;
            Value = Mathf.Clamp01(Value);
        }
        if (Input.GetKey(KeyCode.W))
        {
            Value += PRESSURE_STEP;
            Value = Mathf.Clamp01(Value);
        }
    }
}
