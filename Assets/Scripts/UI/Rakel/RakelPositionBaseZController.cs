
public class RakelPositionBaseZController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.InputManager.RakelPositionBaseZ);
    }

    public void Update()
    {
        if (!OilPaintEngine.RakelPositionBaseZLocked)
        {
            InputField.SetTextWithoutNotify("" + OilPaintEngine.InputManager.RakelPositionBaseZ);
        }
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateRakelPositionBaseZ(value);
    }
}