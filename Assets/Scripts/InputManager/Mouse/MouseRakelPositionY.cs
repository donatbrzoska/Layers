
public class MouseRakelPositionY : FloatValueSource
{
    public override void Update()
    {
        Value = MouseRakelPosition.Get().y;
    }
}
