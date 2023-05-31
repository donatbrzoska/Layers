
public class RakelPositionXLockedController : ToggleController
{
    void Start()
    {
        Toggle.SetIsOnWithoutNotify(OilPaintEngine.Configuration.RakelConfiguration.PositionLocked.x);
    }

    override public void OnValueChanged(bool locked)
    {
        OilPaintEngine.UpdateRakelPositionXLocked(locked);
    }
}