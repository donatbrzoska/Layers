
public class PenRakelPositionX : FloatValueSource
{
    public override void Update()
    {
        Value = PenRakelPosition.Get().x;
    }
}
