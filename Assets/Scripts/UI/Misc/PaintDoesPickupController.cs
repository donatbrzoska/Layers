
public class PaintDoesPickupController : ToggleController
{
    void Start()
    {
        Toggle.SetIsOnWithoutNotify(OilPaintEngine.Config.TransferConfig.PaintDoesPickup);
        MakeNonInteractableInEvaluateMode();
    }

    override public void OnValueChanged(bool value)
    {
        OilPaintEngine.UpdatePaintDoesPickup(value);
    }
}
