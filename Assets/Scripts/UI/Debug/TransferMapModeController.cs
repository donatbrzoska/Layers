
public class TransferMapModeController : DropdownController
{
    new public void Awake()
    {
        base.Awake();
        InitializeElements(typeof(TransferMapMode));
    }

    public void Start()
    {
        Dropdown.SetValueWithoutNotify((int)OilPaintEngine.Configuration.TransferConfiguration.MapMode);
    }

    override public void OnValueChanged(int value)
    {
        OilPaintEngine.UpdateRakelEmitMode((TransferMapMode)value);
    }
}
