
public class BaseSinkMAXVolumeController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Config.TransferConfig.BaseSink_MAX_Volume);
        MakeNonInteractableInEvaluateMode();
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateBaseSink_MAX_Volume(value);
    }
}