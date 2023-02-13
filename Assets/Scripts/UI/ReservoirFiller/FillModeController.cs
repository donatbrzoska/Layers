
public class FillModeController : DropdownController
{
    new public void Awake()
    {
        base.Awake();
        InitializeElements(typeof(FillMode));
    }

    public void Start()
    {
        Dropdown.SetValueWithoutNotify((int)OilPaintEngine.Configuration.FillConfiguration.Mode);
    }

    override public void OnValueChanged(int value)
    {
        OilPaintEngine.UpdateFillMode((FillMode)value);
    }
}
