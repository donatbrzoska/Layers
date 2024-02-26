
public class RakelPositionYLockedController : RakelControlToggleController
{
    void Start()
    {
        Toggle.SetIsOnWithoutNotify(InputManager.UsingScriptPositionY);
    }

    override public void OnValueChanged(bool locked)
    {
        InputManager.UpdateUsingScriptPositionY(locked);
    }
}