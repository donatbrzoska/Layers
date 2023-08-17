
public class RakelCellVolumeController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Config.RakelConfig.CellVolume);
        MakeNonInteractableInEvaluateMode();
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateRakelCellVolume(value);
    }
}