
public class ReadjustZToRakelVolumeController : ConfigToggleController
{
    void Start()
    {
        Toggle.SetIsOnWithoutNotify(OilPaintEngine.Config.TransferConfig.ReadjustZToRakelVolume);
        MakeNonInteractableInEvaluateMode();
    }

    override public void OnValueChanged(bool enabled)
    {
        OilPaintEngine.UpdateReadjustZToRakelVolume(enabled);
    }
}
