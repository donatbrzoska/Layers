
public class RakelPositionYController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.InputManager.RakelPositionY);
    }

    public void Update()
    {
        if (!OilPaintEngine.RakelPositionYLocked)
        {
            InputField.SetTextWithoutNotify("" + OilPaintEngine.InputManager.RakelPositionY);
        }
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateRakelPositionY(value);
    }
}