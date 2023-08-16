
public class Macro6Controller : ButtonController
{
    public void Start()
    {
        CheckEvaluateMode();
    }

    override public void OnClick()
    {
        OilPaintEngine.DoMacro6Action();
    }
}