﻿using System;
public class TextRakelPositionZ : FloatValueSource
{
    public TextRakelPositionZ(float defaultValue)
    {
        Value = defaultValue;
    }

    public override void Update()
    {
        // For consistency, we could fetch the value from the UI element here.
        // However, this feels pretty dirty, since we would have to hard code
        // all the UI element names in these text update methods
    }
}
