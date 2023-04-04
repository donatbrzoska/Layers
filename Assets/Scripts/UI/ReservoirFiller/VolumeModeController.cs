﻿
public class VolumeModeController : DropdownController
{
    new public void Awake()
    {
        base.Awake();
        InitializeElements(typeof(VolumeMode));
    }

    public void Start()
    {
        Dropdown.SetValueWithoutNotify((int)OilPaintEngine.Configuration.FillConfiguration.VolumeMode);
    }

    override public void OnValueChanged(int value)
    {
        OilPaintEngine.UpdateVolumeMode((VolumeMode)value);
    }
}
