﻿using System;
public class LayerThicknessController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Configuration.TransferConfiguration.LayerThickness_MAX);
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateLayerThickness_MAX(value);
    }
}