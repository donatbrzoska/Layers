
public class ReadjustZToCanvasVolumeController : ToggleController
{
    void Start()
    {
        Toggle.SetIsOnWithoutNotify(OilPaintEngine.Config.TransferConfig.ReadjustZToCanvasVolume);
        MakeNonInteractableInEvaluateMode();
    }

    override public void OnValueChanged(bool enabled)
    {
        OilPaintEngine.UpdateReadjustZToCanvasVolume(enabled);
    }
}
