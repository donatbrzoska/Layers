using UnityEngine;

public class InputState
{
    public Vector3 Position;
    public bool PositionAutoBaseZEnabled;
    public float Pressure;
    public float Rotation;
    public float Tilt;

    public InputState(
        Vector3 position,
        bool positionAutoBaseZEnabled,
        float pressure,
        float rotation,
        float tilt)
    {
        Position = position;
        PositionAutoBaseZEnabled = positionAutoBaseZEnabled;
        Pressure = pressure;
        Rotation = rotation;
        Tilt = tilt;
    }
}