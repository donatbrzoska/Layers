
public class SmoothZLengthController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Config.TransferConfig.FloatingZLength);
        MakeNonInteractableInEvaluateMode();
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateSmoothZLength(value);
    }
}