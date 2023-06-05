
public class EmitVolumePickupReservoirController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Configuration.TransferConfiguration.EmitVolumePickupReservoir_MAX);
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateEmitVolumePickupReservoir(value);
    }
}