
public class RakelPositionYController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.RakelPositionY);
    }

    public void Update()
    {
        if (!OilPaintEngine.Configuration.RakelConfiguration.PositionLocked.y)
        {
            InputField.SetTextWithoutNotify("" + OilPaintEngine.RakelPositionY);
        }
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateRakelPositionY(value);
    }
}