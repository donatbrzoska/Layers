
public class RakelPositionXController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.RakelPositionX);
    }

    public void Update()
    {
        if (!OilPaintEngine.Configuration.RakelConfiguration.PositionLocked.x)
        {
            InputField.SetTextWithoutNotify("" + OilPaintEngine.RakelPositionX);
        }
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateRakelPositionX(value);
    }
}