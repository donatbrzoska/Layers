
public class TiltAdjustLTController : ConfigToggleController
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
