
public class Macro3Controller : ButtonController
{
    public void Start()
    {
        CheckEvaluateMode();
    }

    override public void OnClick()
    {
        OilPaintEngine.DoMacro3Action();
    }
}