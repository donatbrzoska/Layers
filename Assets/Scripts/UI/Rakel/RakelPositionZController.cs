
public class RakelPositionZController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.RakelPositionZ);
    }

    public void Update()
    {
        if (!OilPaintEngine.Configuration.RakelConfiguration.PositionLocked.z)
        {
            InputField.SetTextWithoutNotify("" + OilPaintEngine.RakelPositionZ);
        }
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateRakelPositionZ(value);
    }
}