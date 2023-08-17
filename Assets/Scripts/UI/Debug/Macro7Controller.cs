
public class Macro7Controller : ButtonController
{
    public void Start()
    {
        MakeNonInteractableInEvaluateMode();
    }

    override public void OnClick()
    {
        OilPaintEngine.DoMacro7Action();
    }
}