using System.Collections.Generic;
using UnityEngine;

public class AutoRotation
{
    private Vector2 PreviousPosition;
    private Queue<Vector2> PreviousPositions;
    private const int ATTENTION_LENGTH = 20;

    public float Value;

    public AutoRotation()
    {
        PreviousPositions = new Queue<Vector2>();
    }

    public void Update(Vector2 currentPosition)
    {
        bool enoughDifference = (PreviousPosition - currentPosition).magnitude > 1;
        if (enoughDifference)
        {
            PreviousPositions.Enqueue(currentPosition);
            if (PreviousPositions.Count > ATTENTION_LENGTH)
            {
                PreviousPositions.Dequeue();
            }

            Vector2 direction = Vector2.zero;
            foreach (Vector2 p in PreviousPositions)
            {
                Vector2 currentDirection = currentPosition - p;
                direction += currentDirection / PreviousPositions.Count;
            }
            Value = MathUtil.Angle360(Vector2.right, direction);

            PreviousPosition = currentPosition;
        }
    }
}
