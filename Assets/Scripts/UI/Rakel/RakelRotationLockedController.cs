
public class RakelRotationLockedController : ToggleController
{
    void Start()
    {
        Toggle.SetIsOnWithoutNotify(OilPaintEngine.Configuration.RakelConfiguration.RotationLocked);
    }

    override public void OnValueChanged(bool locked)
    {
        OilPaintEngine.UpdateRakelRotationLocked(locked);
    }
}
