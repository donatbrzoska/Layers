
public class FillNoiseVolumeFrequencyXController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Config.FillConfig.NoiseVolumeFrequencyX);
    }

    override public void OnValueChanged(string arg0)
    {
        int value = int.Parse(arg0);
        OilPaintEngine.UpdateFillNoiseVolumeFrequencyX(value);
    }
}