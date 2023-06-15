using System;
using System.Collections.Generic;
using UnityEngine;

public class MouseRakelRotation : FloatValueSource
{
    private Vector2 PreviousMousePosition;
    private Queue<Vector2> PreviousMousePositions;
    private const int ATTENTION_LENGTH = 20;

    public MouseRakelRotation()
    {
        PreviousMousePositions = new Queue<Vector2>();
    }

    public override void Update()
    {
        Vector2 currentMousePosition = Input.mousePosition;
        bool enoughDifference = (PreviousMousePosition - currentMousePosition).magnitude > 1;
        if (enoughDifference)
        {
            PreviousMousePositions.Enqueue(currentMousePosition);
            if (PreviousMousePositions.Count > ATTENTION_LENGTH)
            {
                PreviousMousePositions.Dequeue();
            }

            Vector2 direction = Vector2.zero;
            foreach (Vector2 position in PreviousMousePositions)
            {
                Vector2 currentDirection = currentMousePosition - position;
                direction += currentDirection / PreviousMousePositions.Count;
            }
            Value = MathUtil.Angle360(Vector2.right, direction);

            PreviousMousePosition = currentMousePosition;
        }
    }
}
