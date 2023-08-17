
public class Macro8Controller : ButtonController
{
    public void Start()
    {
        MakeNonInteractableInEvaluateMode();
    }

    override public void OnClick()
    {
        OilPaintEngine.DoMacro8Action();
    }
}