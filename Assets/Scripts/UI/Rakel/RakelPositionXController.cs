
public class RakelPositionXController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.InputManager.RakelPositionX);
    }

    public void Update()
    {
        if (!OilPaintEngine.RakelPositionXLocked)
        {
            InputField.SetTextWithoutNotify("" + OilPaintEngine.InputManager.RakelPositionX);
        }
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateRakelPositionX(value);
    }
}