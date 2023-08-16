
public class Macro4Controller : ButtonController
{
    public void Start()
    {
        CheckEvaluateMode();
    }

    override public void OnClick()
    {
        OilPaintEngine.DoMacro4Action();
    }
}