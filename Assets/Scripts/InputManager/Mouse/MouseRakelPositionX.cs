
public class MouseRakelPositionX : FloatValueSource
{
    public override void Update()
    {
        Value = MouseRakelPosition.Get().x;
    }
}
