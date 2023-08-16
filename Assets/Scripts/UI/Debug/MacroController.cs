
public class MacroController : ButtonController
{
    public void Start()
    {
        CheckEvaluateMode();
    }

    override public void OnClick()
    {
        OilPaintEngine.DoMacroAction();
    }
}