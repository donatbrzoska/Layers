using UnityEngine;

public class KeyboardControl : MonoBehaviour,
    PressureSource, TiltSource
{
    public float Pressure { get; private set; }
    public float Tilt { get; private set; }

    private FrameStopwatch FrameStopwatch;
    private float PRESSURE_STEP_PER_SECOND = 1;
    private float TILT_STEP_PER_SECOND = 50;

    void Start()
    {
        FrameStopwatch = new FrameStopwatch();
    }

    void Update()
    {
        FrameStopwatch.Update();

        if (Input.GetKey(KeyCode.Q))
        {
            Pressure -= FrameStopwatch.SecondsSinceLastFrame * PRESSURE_STEP_PER_SECOND;
        }
        if (Input.GetKey(KeyCode.W))
        {
            Pressure += FrameStopwatch.SecondsSinceLastFrame * PRESSURE_STEP_PER_SECOND;
        }
        Pressure = Mathf.Clamp01(Pressure);

        if (Input.GetKey(KeyCode.A))
        {
            Tilt += FrameStopwatch.SecondsSinceLastFrame * TILT_STEP_PER_SECOND;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Tilt -= FrameStopwatch.SecondsSinceLastFrame * TILT_STEP_PER_SECOND;
        }
        Tilt = Rakel.ClampTilt(Tilt);
    }
}
