
public class RakelDiffuseRatioController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Config.TransferConfig.RakelDiffuseRatio);
        CheckEvaluateMode();
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateRakelDiffuseRatio(value);
    }
}