
public class RakelPositionYLockedController : ToggleController
{
    void Start()
    {
        Toggle.SetIsOnWithoutNotify(OilPaintEngine.Configuration.RakelConfiguration.PositionLocked.y);
    }

    override public void OnValueChanged(bool locked)
    {
        OilPaintEngine.UpdateRakelPositionYLocked(locked);
    }
}