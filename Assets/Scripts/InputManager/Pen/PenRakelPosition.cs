using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PenRakelPosition
{
    public static Vector3 Get()
    {
        return ScreenToWorld.Convert(Pen.current.position.ReadValue());
    }
}
