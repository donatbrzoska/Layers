
public class RakelDiffuseRatioController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Config.TransferConfig.RakelDiffuseRatio);
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateRakelDiffuseRatio(value);
    }
}