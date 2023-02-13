
public class SmoothingKernelSizeController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Configuration.TransferConfiguration.ReservoirSmoothingKernelSize);
    }

    override public void OnValueChanged(string arg0)
    {
        int value = int.Parse(arg0);
        OilPaintEngine.UpdateReservoirSmoothingKernelSize(value);
    }
}