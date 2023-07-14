using UnityEngine;

public class MouseRakelPosition
{
    public static Vector3 Get()
    {
        return ScreenToWorld.Convert(Input.mousePosition);
    }
}
