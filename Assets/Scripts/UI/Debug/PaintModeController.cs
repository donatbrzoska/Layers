
using System.Collections.Generic;
using System;

public class PaintModeController : DropdownController
{
    new public void Awake()
    {
        base.Awake();
        //InitializeElements(typeof(PaintMode));
        List<string> paintModeNames = new List<string>();
        foreach (PaintMode p in Enum.GetValues(typeof(PaintMode)))
        {
            paintModeNames.Add(PaintModes.GetName(p));
        }
        Dropdown.AddOptions(paintModeNames);
    }

    public void Start()
    {
        Dropdown.SetValueWithoutNotify((int)OilPaintEngine.PaintMode);
    }

    override public void OnValueChanged(int value)
    {
        OilPaintEngine.UpdatePaintMode((PaintMode)value);
    }
}
