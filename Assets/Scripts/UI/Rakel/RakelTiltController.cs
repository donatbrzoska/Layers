
public class RakelTiltController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.InputManager.RakelTilt);
    }

    public void Update()
    {
        if (!OilPaintEngine.RakelTiltLocked)
        {
            InputField.SetTextWithoutNotify("" + OilPaintEngine.InputManager.RakelTilt);
        }
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateRakelTilt(value);
    }
}