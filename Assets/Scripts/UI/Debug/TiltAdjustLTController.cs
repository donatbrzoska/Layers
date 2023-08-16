
public class TiltAdjustLTController : ToggleController
{
    void Start()
    {
        Toggle.SetIsOnWithoutNotify(OilPaintEngine.Config.TransferConfig.TiltAdjustLayerThickness);
        CheckEvaluateMode();
    }

    override public void OnValueChanged(bool value)
    {
        OilPaintEngine.UpdateTiltAdjustLayerThickness(value);
    }
}
