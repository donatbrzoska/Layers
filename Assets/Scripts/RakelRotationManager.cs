using System;
using UnityEngine;

public class RakelRotationManager
{
    public bool RotationLocked { get; set; }

    private bool PreviousMousePositionInitialized;
    private Vector2 PreviousMousePosition;
    private float CurrentRotation;

    public RakelRotationManager()
    {
        PreviousMousePositionInitialized = false;
    }

    public float GetRotation()
    {
        if (!RotationLocked)
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
                    CurrentRotation = angle;

                    PreviousMousePosition = currentMousePosition;
                }
            }
        }
        return CurrentRotation;
    }
}
