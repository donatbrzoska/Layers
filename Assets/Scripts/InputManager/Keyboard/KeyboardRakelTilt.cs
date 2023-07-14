using UnityEngine;

public class KeyboardRakelTilt : FloatValueSource
{
    private FrameStopwatch FrameStopwatch;
    private float TILT_STEP_PER_SECOND = 50;

    public KeyboardRakelTilt()
    {
        FrameStopwatch = new FrameStopwatch();
    }

    public override void Update()
    {
        FrameStopwatch.Update();

        if (Input.GetKey(KeyCode.A))
        {
            Value += FrameStopwatch.SecondsSinceLastFrame * TILT_STEP_PER_SECOND;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Value -= FrameStopwatch.SecondsSinceLastFrame * TILT_STEP_PER_SECOND;
        }

        Value = Rakel.ClampTilt(Value);
    }
}
