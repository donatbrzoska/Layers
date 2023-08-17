
public class RakelVolumeReduceFunctionController : DropdownController
{
    new public void Awake()
    {
        base.Awake();
        InitializeElements(typeof(ReduceFunction));
    }

    public void Start()
    {
        Dropdown.SetValueWithoutNotify((int)OilPaintEngine.Config.TransferConfig.RakelVolumeReduceFunction);
        MakeNonInteractableInEvaluateMode();
    }

    override public void OnValueChanged(int value)
    {
        OilPaintEngine.UpdateRakelVolumeReduceFunction((ReduceFunction)value);
    }
}
