
public class RakelTiltLockedController : ToggleController
{
    void Start()
    {
        Toggle.SetIsOnWithoutNotify(OilPaintEngine.Configuration.RakelConfiguration.TiltLocked);
    }

    override public void OnValueChanged(bool locked)
    {
        OilPaintEngine.UpdateRakelTiltLocked(locked);
    }
}
