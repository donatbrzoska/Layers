
public class RakelTiltLockedController : RakelControlToggleController
{
    void Start()
    {
        Toggle.SetIsOnWithoutNotify(InputManager.UsingScriptTilt);
    }

    override public void OnValueChanged(bool locked)
    {
        InputManager.UpdateUsingScriptTilt(locked);
    }
}
