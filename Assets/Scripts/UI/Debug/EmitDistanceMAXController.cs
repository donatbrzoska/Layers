
public class EmitDistanceMAXController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Config.TransferConfig.EmitDistance_MAX);
        CheckEvaluateMode();
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateEmitDistance_MAX(value);
    }
}