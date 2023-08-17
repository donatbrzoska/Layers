
public class MacroController : ButtonController
{
    public void Start()
    {
        MakeNonInteractableInEvaluateMode();
    }

    override public void OnClick()
    {
        OilPaintEngine.DoMacroAction();
    }
}