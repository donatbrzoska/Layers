
public class RakelPressureLockedController : ToggleController
{
    void Start()
    {
        Toggle.SetIsOnWithoutNotify(OilPaintEngine.RakelPressureLocked);
    }

    override public void OnValueChanged(bool locked)
    {
        OilPaintEngine.UpdateRakelPressureLocked(locked);
    }
}