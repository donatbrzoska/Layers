
public class RakelPositionXController : InputFieldController
{
    public void Update()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.RakelPositionX);
    }

    override public void OnValueChanged(string arg0)
    {
        int value = int.Parse(arg0);
        OilPaintEngine.UpdateRakelPositionX(value);
    }
}