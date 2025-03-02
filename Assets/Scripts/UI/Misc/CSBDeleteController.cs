
public class CSBDeleteController : ConfigToggleController
{
    void Start()
    {
        Toggle.SetIsOnWithoutNotify(OilPaintEngine.Config.TransferConfig.DeletePickedUpFromCSB);
    }

    override public void OnValueChanged(bool value)
    {
        OilPaintEngine.UpdateDeletePickedUpFromCSB(value);
    }
}
