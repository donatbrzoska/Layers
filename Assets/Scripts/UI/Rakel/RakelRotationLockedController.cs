
public class RakelRotationLockedController : RakelControlToggleController
{
    void Start()
    {
        Toggle.SetIsOnWithoutNotify(InputManager.UsingScriptRotation);
    }

    override public void OnValueChanged(bool locked)
    {
        InputManager.UpdateUsingScriptRotation(locked);
    }
}
