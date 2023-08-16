
public class Macro2Controller : ButtonController
{
    public void Start()
    {
        CheckEvaluateMode();
    }

    override public void OnClick()
    {
        OilPaintEngine.DoMacro2Action();
    }
}