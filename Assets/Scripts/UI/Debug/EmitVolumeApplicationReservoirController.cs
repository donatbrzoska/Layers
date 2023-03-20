
public class EmitVolumeApplicationReservoirController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Configuration.TransferConfiguration.EmitVolumeApplicationReservoir);
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateEmitVolumeApplicationReservoir(value);
    }
}