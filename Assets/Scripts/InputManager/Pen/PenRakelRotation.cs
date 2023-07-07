using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PenRakelRotation : FloatValueSource
{
    private Vector2 PreviousPenPosition;
    private Queue<Vector2> PreviousPenPositions;
    private const int ATTENTION_LENGTH = 20;

    public PenRakelRotation()
    {
        PreviousPenPositions = new Queue<Vector2>();
    }

    public override void Update()
    {
        Vector2 currentPenPosition = Pen.current.position.ReadValue();
        bool enoughDifference = (PreviousPenPosition - currentPenPosition).magnitude > 1;
        if (enoughDifference)
        {
            PreviousPenPositions.Enqueue(currentPenPosition);
            if (PreviousPenPositions.Count > ATTENTION_LENGTH)
            {
                PreviousPenPositions.Dequeue();
            }

            Vector2 direction = Vector2.zero;
            foreach (Vector2 position in PreviousPenPositions)
            {
                Vector2 currentDirection = currentPenPosition - position;
                direction += currentDirection / PreviousPenPositions.Count;
            }
            Value = MathUtil.Angle360(Vector2.right, direction);

            PreviousPenPosition = currentPenPosition;
        }
    }
}
