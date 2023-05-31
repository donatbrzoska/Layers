
public class RakelPositionYController : InputFieldController
{
    public void Update()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.RakelPositionY);
    }

    override public void OnValueChanged(string arg0)
    {
        int value = int.Parse(arg0);
        OilPaintEngine.UpdateRakelPositionY(value);
    }
}