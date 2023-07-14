using UnityEngine;

public class KeyboardRakelPressure : FloatValueSource
{
    private FrameStopwatch FrameStopwatch;
    private float PRESSURE_STEP_PER_SECOND = 1;

    public KeyboardRakelPressure()
    {
        FrameStopwatch = new FrameStopwatch();
    }

    public override void Update()
    {
        FrameStopwatch.Update();

        if (Input.GetKey(KeyCode.Q))
        {
            Value -= FrameStopwatch.SecondsSinceLastFrame * PRESSURE_STEP_PER_SECOND;
            Value = Mathf.Clamp01(Value);
        }
        if (Input.GetKey(KeyCode.W))
        {
            Value += FrameStopwatch.SecondsSinceLastFrame * PRESSURE_STEP_PER_SECOND;
            Value = Mathf.Clamp01(Value);
        }
    }
}
