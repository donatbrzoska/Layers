
public class FillNoiseVolumeFrequencyYController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Config.FillConfig.NoiseVolumeFrequencyY);
    }

    override public void OnValueChanged(string arg0)
    {
        int value = int.Parse(arg0);
        OilPaintEngine.UpdateFillNoiseVolumeFrequencyY(value);
    }
}