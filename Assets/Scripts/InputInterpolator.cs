using UnityEngine;
using System.Collections.Generic;

public class InputInterpolator : InputStateSource
{
    public Queue<InputState> LeftInputStates;

    private Rakel Rakel;
    private Canvas_ Canvas;

    private InputState PreviousInputState;

    public InputInterpolator()
    {
        LeftInputStates = new();
    }

    public void SetRakel(Rakel rakel)
    {
        Rakel = rakel;
    }

    public void SetCanvas(Canvas_ canvas)
    {
        Canvas = canvas;
    }

    public void AddNode(InputState inputState, bool isFirstNodeOfStroke, int interpolationResolution)
    {
        if (inputState.Equals(PreviousInputState))
        {
            return;
        }

        if (isFirstNodeOfStroke)
        {
            LeftInputStates.Enqueue(inputState);
            PreviousInputState = inputState;
            return;
        }

        // 1. determine differences and steps
        Vector3 dp = inputState.Position - PreviousInputState.Position;
        //float dpLength = dp.magnitude;
        Vector2 dp_ = Canvas.MapToPixel(inputState.Position) - Canvas.MapToPixel(PreviousInputState.Position);
        float dpLength = dp_.magnitude;
        int positionSteps = (int)(dpLength * interpolationResolution); // don't add 1 because the first one is already done

        float dpr = inputState.Pressure - PreviousInputState.Pressure;

        float dr = inputState.Rotation - PreviousInputState.Rotation;
        if (Mathf.Abs(dr) >= 300)
        {
            if (inputState.Rotation < PreviousInputState.Rotation)
            {
                // turn over case 1: from 360 to 0
                // -> dr in in this case is something like -345
                // -> needs to be positive and small though because we want to rotate further over
                dr = 360 + dr;
            }
            else
            {
                // turn over case 2: from 0 to 360
                // -> dr in this case is something like 345
                // -> needs to be negative negative and small though because we want to rotate further over
                dr = dr - 360;
            }
        }
        float arcLength = Mathf.PI * (Rakel.Info.Length / 2) * (Mathf.Abs(dr) / 180);
        int rotationSteps = (int)(arcLength * interpolationResolution);

        float dt = inputState.Tilt - PreviousInputState.Tilt;
        arcLength = Mathf.PI * Rakel.Info.Width * (Mathf.Abs(dt) / 180);
        int tiltSteps = (int)(arcLength * interpolationResolution);

        int steps = Mathf.Max(1, Mathf.Max(Mathf.Max(positionSteps, rotationSteps), tiltSteps));


        // 2. interpolate
        Vector3 previousPosition = PreviousInputState.Position;
        float previousPressure = PreviousInputState.Pressure;
        float previousRotation = PreviousInputState.Rotation;
        float previousTilt = PreviousInputState.Tilt;

        for (int i = 0; i < steps; i++)
        {
            // first one is skipped, because that was already done when isFirstNodeOfStroke

            Vector3 currentPosition = previousPosition + dp / steps;
            //Vector3 currentPosition = PreviousRakelPosition + (i+1) * (dp / steps); // doesn't seem to make a difference

            float currentPressure = previousPressure + dpr / steps;

            float currentRotation = previousRotation + dr / steps;
            if (currentRotation >= 360)
            { // fix turnover case 1
                currentRotation = currentRotation % 360;
            }
            if (currentRotation < 0)
            { // fix turnover case 2
                currentRotation = 360 + currentRotation;
            }

            float currentTilt = previousTilt + dt / steps;

            LeftInputStates.Enqueue(
                new InputState(
                    currentPosition,
                    inputState.PositionAutoBaseZEnabled,
                    currentPressure,
                    currentRotation,
                    currentTilt));

            previousPosition = currentPosition;
            previousRotation = currentRotation;
            previousTilt = currentTilt;
        }

        PreviousInputState = inputState;
    }

    public bool HasNext()
    {
        return LeftInputStates.Count > 0;
    }

    public InputState Next()
    {
        return LeftInputStates.Dequeue();
    }
}
