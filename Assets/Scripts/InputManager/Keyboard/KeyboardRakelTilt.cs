using UnityEngine;

public class KeyboardRakelTilt : FloatValueSource
{
    private float TILT_STEP = 0.1f;

    public override void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            Value += TILT_STEP;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Value -= TILT_STEP;
        }

        Value = Rakel.ClampTilt(Value);
    }
}
