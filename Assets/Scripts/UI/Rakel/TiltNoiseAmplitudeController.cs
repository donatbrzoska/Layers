
public class TiltNoiseAmplitudeController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Config.RakelConfig.TiltNoiseAmplitude);
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateRakelTiltNoiseAmplitude(value);
    }
}