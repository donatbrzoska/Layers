
public class ColorModeController : DropdownController
{
    new public void Awake()
    {
        base.Awake();
        InitializeElements(typeof(ColorMode));
    }

    public void Start()
    {
        Dropdown.SetValueWithoutNotify((int)OilPaintEngine.Config.FillConfig.ColorMode);
    }

    override public void OnValueChanged(int value)
    {
        OilPaintEngine.UpdateColorMode((ColorMode)value);
    }
}
