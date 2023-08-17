
public class ClearCanvasController : ButtonController
{
    public void Start()
    {
        MakeNonInteractableInEvaluateMode();
    }

    override public void OnClick()
    {
        OilPaintEngine.ClearCanvas();
    }
}
