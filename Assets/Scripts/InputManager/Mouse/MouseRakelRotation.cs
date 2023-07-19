﻿using UnityEngine;

public class MouseRakelRotation : FloatValueSource
{
    private AutoRotation AutoRotation;

    public MouseRakelRotation()
    {
        AutoRotation = new AutoRotation();
    }

    public override void Update()
    {
        AutoRotation.Update(Input.mousePosition);
        Value = AutoRotation.Value;
    }
}
