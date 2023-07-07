using UnityEngine;

public class PenRakelPositionY : FloatValueSource
{
    public override void Update()
    {
        Vector3 hit = PenRakelPositionRaycaster.Raycast();
        Value = hit.y;
    }
}
