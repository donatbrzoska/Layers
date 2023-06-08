using UnityEngine;

public class MouseRakelPositionX : FloatValueSource
{
    public override void Update()
    {
        Vector3 hit = MouseRakelPositionRaycaster.Raycast();
        Value = hit.x;
    }
}
