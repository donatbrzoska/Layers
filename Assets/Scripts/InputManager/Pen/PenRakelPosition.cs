using UnityEngine;
using UnityEngine.InputSystem;

public class PenRakelPosition
{
    public static Vector3 Get()
    {
        return ScreenToWorld.Convert(Pen.current.position.ReadValue());
    }
}
