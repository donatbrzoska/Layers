
public class CSBEnabledController: ConfigToggleController
{
    void Start()
    {
        Toggle.SetIsOnWithoutNotify(OilPaintEngine.Config.TransferConfig.CanvasSnapshotBufferEnabled);
    }

    override public void OnValueChanged(bool locked)
    {
        OilPaintEngine.UpdateCanvasSnapshotBufferEnabled(locked);
    }
}
