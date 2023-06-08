
public class RakelPositionZController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.InputManager.RakelPositionZ);
    }

    public void Update()
    {
        if (!OilPaintEngine.RakelPositionZLocked)
        {
            InputField.SetTextWithoutNotify("" + OilPaintEngine.InputManager.RakelPositionZ);
        }
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateRakelPositionZ(value);
    }
}