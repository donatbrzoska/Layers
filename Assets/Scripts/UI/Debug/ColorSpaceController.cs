
public class ColorSpaceController : DropdownController
{
    new public void Awake()
    {
        base.Awake();
        InitializeElements(typeof(ColorSpace));
    }

    public void Start()
    {
        Dropdown.SetValueWithoutNotify((int)OilPaintEngine.Config.ColorSpace);
        MakeNonInteractableInEvaluateMode();
    }

    override public void OnValueChanged(int value)
    {
        OilPaintEngine.UpdateColorSpace((ColorSpace)value);
    }
}
