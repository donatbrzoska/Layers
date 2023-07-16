
public class TiltNoiseFrequencyController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Config.RakelConfig.TiltNoiseFrequency);
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateRakelTiltNoiseFrequency(value);
    }
}