
public class CanvasDiffuseDepthController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Config.TransferConfig.CanvasDiffuseDepth);
        CheckEvaluateMode();
    }

    override public void OnValueChanged(string arg0)
    {
        int value = int.Parse(arg0);
        OilPaintEngine.UpdateCanvasDiffuseDepth(value);
    }
}