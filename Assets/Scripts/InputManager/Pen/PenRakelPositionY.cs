
public class PenRakelPositionY : FloatValueSource
{
    public override void Update()
    {
        Value = PenRakelPosition.Get().y;
    }
}
