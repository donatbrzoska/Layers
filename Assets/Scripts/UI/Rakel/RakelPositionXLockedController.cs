
public class RakelPositionXLockedController : RakelControlToggleController
{
    void Start()
    {
        Toggle.SetIsOnWithoutNotify(InputManager.UsingScriptPositionX);
    }

    override public void OnValueChanged(bool locked)
    {
        InputManager.UpdateUsingScriptPositionX(locked);
    }
}