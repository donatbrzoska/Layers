
public class PickupVolumeController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Configuration.TransferConfiguration.PickupVolume_MAX);
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdatePickupVolume(value);
    }
}