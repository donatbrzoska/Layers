
public class RakelYPositionLockedController : ToggleController
{
    void Start()
    {
        Toggle.SetIsOnWithoutNotify(OilPaintEngine.Configuration.RakelYPositionLocked);
    }

    override public void OnValueChanged(bool locked)
    {
        OilPaintEngine.UpdateRakelYPositionLocked(locked);
    }
}
