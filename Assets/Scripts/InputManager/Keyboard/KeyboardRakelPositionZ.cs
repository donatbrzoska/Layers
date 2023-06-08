using UnityEngine;

public class KeyboardRakelPositionZ : FloatValueSource
{
    private const float POSITION_Z_STEP = 0.1f;

    public override void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            Value += POSITION_Z_STEP;
        }
        if (Input.GetKey(KeyCode.W))
        {
            Value -= POSITION_Z_STEP;
        }
    }
}
