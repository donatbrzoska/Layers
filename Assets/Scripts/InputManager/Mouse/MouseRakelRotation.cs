using System;
using UnityEngine;

public class MouseRakelRotation : FloatValueSource
{
    private bool PreviousMousePositionInitialized;
    private Vector2 PreviousMousePosition;

    public override void Update()
    {
        Vector2 currentMousePosition = Input.mousePosition;
        if (!PreviousMousePositionInitialized)
        {
            PreviousMousePosition = currentMousePosition;
            PreviousMousePositionInitialized = true;
        }
        else
        {
            Vector2 direction = currentMousePosition - PreviousMousePosition;

            if (direction.magnitude > 8)
            {
                float angle = MathUtil.Angle360(Vector2.right, direction);
                Value = angle;

                PreviousMousePosition = currentMousePosition;
            }
        }
    }
}
