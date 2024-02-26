
public class RakelPressureLockedController : RakelControlToggleController
{
    void Start()
    {
        Toggle.SetIsOnWithoutNotify(InputManager.UsingScriptPressure);
    }

    override public void OnValueChanged(bool locked)
    {
        InputManager.UpdateUsingScriptPressure(locked);
    }
}