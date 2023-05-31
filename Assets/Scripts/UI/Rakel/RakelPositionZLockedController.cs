
public class RakelPositionZLockedController : ToggleController
{
    void Start()
    {
        Toggle.SetIsOnWithoutNotify(OilPaintEngine.Configuration.RakelConfiguration.PositionLocked.z);
    }

    override public void OnValueChanged(bool locked)
    {
        OilPaintEngine.UpdateRakelPositionZLocked(locked);
    }
}