
public class RakelPositionZController : InputFieldController
{
    public void Update()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.RakelPositionZ);
    }

    override public void OnValueChanged(string arg0)
    {
        int value = int.Parse(arg0);
        OilPaintEngine.UpdateRakelPositionZ(value);
    }
}