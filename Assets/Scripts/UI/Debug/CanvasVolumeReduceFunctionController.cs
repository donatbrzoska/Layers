
public class CanvasVolumeReduceFunctionController : DropdownController
{
    new public void Awake()
    {
        base.Awake();
        InitializeElements(typeof(ReduceFunction));
    }

    public void Start()
    {
        Dropdown.SetValueWithoutNotify((int)OilPaintEngine.Config.TransferConfig.CanvasVolumeReduceFunction);
        MakeNonInteractableInEvaluateMode();
    }

    override public void OnValueChanged(int value)
    {
        OilPaintEngine.UpdateCanvasVolumeReduceFunction((ReduceFunction)value);
    }
}
