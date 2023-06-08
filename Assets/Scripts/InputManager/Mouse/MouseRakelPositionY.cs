using UnityEngine;

public class MouseRakelPositionY : FloatValueSource
{
    public override void Update()
    {
        Vector3 hit = MouseRakelPositionRaycaster.Raycast();
        Value = hit.y;
    }
}
