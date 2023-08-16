
public class TrueVolumeMINController : ToggleController
{
    void Start()
    {
        Toggle.SetIsOnWithoutNotify(OilPaintEngine.Config.TransferConfig.TrueVolume_MIN_Calculation);
        CheckEvaluateMode();
    }

    override public void OnValueChanged(bool value)
    {
        OilPaintEngine.UpdateTrueVolume_MIN(value);
    }
}
