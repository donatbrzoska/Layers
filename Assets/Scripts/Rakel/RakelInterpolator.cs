using UnityEngine;

public class RakelInterpolator
{
    private RenderTexture DrawingTarget;
    private WorldSpaceCanvas WorldSpaceCanvas;
    private IRakel Rakel;

    private Vector3 PreviousRakelPosition;
    private float PreviousRakelRotation;
    private float PreviousRakelTilt;

    private float NO_ANGLE = float.NaN;
    private Vector3 NO_POSITION = Vector3.negativeInfinity;

    public RakelInterpolator(IRakel rakel, WorldSpaceCanvas wsc, RenderTexture drawingTarget)
    {
        Rakel = rakel;
        WorldSpaceCanvas = wsc;
        DrawingTarget = drawingTarget;
    }

    public void NewStroke()
    {
        PreviousRakelPosition = NO_POSITION;
        PreviousRakelRotation = NO_ANGLE;
        PreviousRakelTilt = NO_ANGLE;
    }

    public void AddNode(Vector3 rakelPosition, float rakelRotation, float rakelTilt, int interpolationResolution)
    {
        // only reapply if there are changes
        if (!rakelPosition.Equals(PreviousRakelPosition)
            || !rakelRotation.Equals(PreviousRakelRotation)
            || !rakelTilt.Equals(PreviousRakelTilt))
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            bool isFirstNodeOfStroke = PreviousRakelPosition.Equals(NO_POSITION)
                && PreviousRakelRotation.Equals(NO_ANGLE)
                && PreviousRakelTilt.Equals(NO_ANGLE);
            if (isFirstNodeOfStroke)
            {
                Rakel.Apply(
                    rakelPosition,
                    rakelRotation,
                    rakelTilt,
                    WorldSpaceCanvas.Position,
                    WorldSpaceCanvas.Size,
                    DrawingTarget);
            }
            else
            {
                // 1. determine differences and steps
                Vector3 dp = rakelPosition - PreviousRakelPosition;
                float dpLength = dp.magnitude;
                int positionSteps = (int)(dpLength * interpolationResolution);
                
                float dr = rakelRotation - PreviousRakelRotation;
                if (Mathf.Abs(dr) >= 300){
                    if (rakelRotation < PreviousRakelRotation) {
                        // turn over case 1: from 360 to 0
                        // -> dr in in this case is something like -345
                        // -> needs to be positive and small though because we want to rotate further over
                        dr = 360 + dr;
                    } else {
                        // turn over case 2: from 0 to 360
                        // -> dr in this case is something like 345
                        // -> needs to be negative negative and small though because we want to rotate further over
                        dr = dr - 360;
                    }
                }
                float arcLength = Mathf.PI * (Rakel.Length / 2) * (Mathf.Abs(dr)/180);
                int rotationSteps = (int)(arcLength * interpolationResolution);

                float dt = Mathf.Abs(PreviousRakelTilt - rakelTilt);
                arcLength = Mathf.PI * Rakel.Width * (Mathf.Abs(dt)/180);
                int tiltSteps = (int)(arcLength * interpolationResolution);

                int steps = Mathf.Max(1, Mathf.Max(Mathf.Max(positionSteps, rotationSteps), tiltSteps));


                // 2. interpolate
                Vector3 previousPosition = PreviousRakelPosition;
                float previousRotation = PreviousRakelRotation;
                float previousTilt = PreviousRakelTilt;

                for (int i=0; i<steps; i++)
                {
                    // first one is skipped, because that was already added with new stroke call

                    Vector3 currentPosition = previousPosition + dp / steps;

                    float currentRotation = previousRotation + dr / steps;
                    if (currentRotation >= 360) { // fix turnover case 1
                        currentRotation = currentRotation % 360;
                    }
                    if (currentRotation < 0) { // fix turnover case 2
                        currentRotation = 360 + currentRotation;
                    }

                    float currentTilt = previousTilt + dt / steps;

                    Rakel.Apply(
                        currentPosition,
                        currentRotation,
                        currentTilt,
                        WorldSpaceCanvas.Position,
                        WorldSpaceCanvas.Size,
                        DrawingTarget
                    );

                    previousPosition = currentPosition;
                    previousRotation = currentRotation;
                    previousTilt = currentTilt;
                }
            }

            PreviousRakelPosition = rakelPosition;
            PreviousRakelRotation = rakelRotation;
            PreviousRakelTilt = rakelTilt;
            //if (logTime)
            //    UnityEngine.Debug.Log("UpdatePosition took " + sw.ElapsedMilliseconds + "ms");
        }
    }
}
