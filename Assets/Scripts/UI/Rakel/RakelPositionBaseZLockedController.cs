
public class RakelPositionBaseZLockedController : RakelControlToggleController
{
    void Start()
    {
        Toggle.SetIsOnWithoutNotify(InputManager.UsingScriptPositionBaseZ);
    }

    override public void OnValueChanged(bool locked)
    {
        InputManager.UpdateUsingScriptPositionBaseZ(locked);
    }
}