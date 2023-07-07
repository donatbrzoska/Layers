using UnityEngine;

public class PenRakelPositionX : FloatValueSource
{
    public override void Update()
    {
        Vector3 hit = PenRakelPositionRaycaster.Raycast();
        Value = hit.x;
    }
}
