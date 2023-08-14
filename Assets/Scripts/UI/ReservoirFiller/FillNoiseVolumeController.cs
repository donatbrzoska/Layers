
public class FillNoiseVolumeController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Config.FillConfig.NoiseVolume);
    }

    override public void OnValueChanged(string arg0)
    {
        int value = int.Parse(arg0);
        OilPaintEngine.UpdateFillNoiseVolume(value);
    }
}