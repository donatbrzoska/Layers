using UnityEngine;

public class TextRakelPressure : FloatValueSource
{
    public override void Update()
    {
        Value = Mathf.Clamp01(Value);
    }
}
