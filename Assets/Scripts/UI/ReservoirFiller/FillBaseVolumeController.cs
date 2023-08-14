
public class FillBaseVolumeController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Config.FillConfig.BaseVolume);
    }

    override public void OnValueChanged(string arg0)
    {
        int value = int.Parse(arg0);
        OilPaintEngine.UpdateFillBaseVolume(value);
    }
}