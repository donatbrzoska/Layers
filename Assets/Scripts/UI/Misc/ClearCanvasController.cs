
public class ClearCanvasController : ButtonController
{
    public void Start()
    {
        CheckEvaluateMode();
    }

    override public void OnClick()
    {
        OilPaintEngine.ClearCanvas();
    }
}
