
public class FillWidthPartController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Config.FillConfig.WidthPart);
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateFillWidthPart(value);
    }
}