using System.Collections.Generic;
using UnityEngine;

public class AutoRotation
{
    private Vector2 PreviousPosition;
    private Queue<Vector2> PreviousDirections;
    private const int ATTENTION_LENGTH = 20;

    public float Value;

    public AutoRotation()
    {
        PreviousDirections = new Queue<Vector2>();
    }

    public void Update(Vector2 currentPosition)
    {
        bool enoughDifference = (PreviousPosition - currentPosition).magnitude > 0.05;
        if (enoughDifference)
        {
            Vector2 currentDirection = currentPosition - PreviousPosition;

            PreviousDirections.Enqueue(currentDirection);
            if (PreviousDirections.Count > ATTENTION_LENGTH)
            {
                PreviousDirections.Dequeue();
            }

            Vector2 direction = Vector2.zero;
            foreach (Vector2 d in PreviousDirections)
            {
                direction += d;
            }
            Value = MathUtil.Angle360(Vector2.right, direction);

            PreviousPosition = currentPosition;
        }
    }
}
