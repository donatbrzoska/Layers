
public class RakelPositionXController : InputFieldController
{
    public void Update()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.RakelPositionX);
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateRakelPositionX(value);
    }
}