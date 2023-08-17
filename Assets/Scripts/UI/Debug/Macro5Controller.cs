
public class Macro5Controller : ButtonController
{
    public void Start()
    {
        MakeNonInteractableInEvaluateMode();
    }

    override public void OnClick()
    {
        OilPaintEngine.DoMacro5Action();
    }
}