
public class RakelPositionXLockedController : ToggleController
{
    void Start()
    {
        Toggle.SetIsOnWithoutNotify(OilPaintEngine.RakelPositionXLocked);
    }

    override public void OnValueChanged(bool locked)
    {
        OilPaintEngine.UpdateRakelPositionXLocked(locked);
    }
}