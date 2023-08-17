
public class TiltAdjustLTController : ToggleController
{
    void Start()
    {
        Toggle.SetIsOnWithoutNotify(OilPaintEngine.Config.TransferConfig.TiltAdjustLayerThickness);
        MakeNonInteractableInEvaluateMode();
    }

    override public void OnValueChanged(bool value)
    {
        OilPaintEngine.UpdateTiltAdjustLayerThickness(value);
    }
}
