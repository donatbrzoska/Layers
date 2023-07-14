using System;
using System.Collections.Generic;

public class FillColorController : DropdownController
{
    new public void Awake()
    {
        base.Awake();
        //InitializeElements(typeof(_Color));
        List<string> colorNames = new List<string>();
        foreach (Color_ c in Enum.GetValues(typeof(Color_)))
        {
            colorNames.Add(Colors.GetName(c));
        }
        Dropdown.AddOptions(colorNames);
    }

    public void Start()
    {
        Dropdown.SetValueWithoutNotify((int)OilPaintEngine.Config.FillConfig.Color);
    }

    override public void OnValueChanged(int value)
    {
        OilPaintEngine.UpdateFillColor((Color_)value);
    }
}
