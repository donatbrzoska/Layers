using UnityEngine;

public class InputInterpolator
{
    private TransferEngine TransferEngine;
    private Rakel Rakel;
    private Canvas_ Canvas;

    private Vector3 PreviousRakelPosition;
    private float PreviousRakelPressure;
    private float PreviousRakelRotation;
    private float PreviousRakelTilt;

    private float NO_VALUE = float.NaN;
    private Vector3 NO_POSITION = Vector3.negativeInfinity;

    public InputInterpolator()
    {
    }

    public void SetTransferEngine(TransferEngine transferEngine)
    {
        TransferEngine = transferEngine;
    }

    public void SetRakel(Rakel rakel)
    {
        Rakel = rakel;
    }

    public void SetCanvas(Canvas_ canvas)
    {
        Canvas = canvas;
    }

    public void NewStroke(bool tiltNoiseEnabled, float tiltNoiseFrequency, float tiltNoiseAmplitude, float floatingZLength, bool csbEnabled)
    {
        PreviousRakelPosition = NO_POSITION;
        PreviousRakelPressure = NO_VALUE;
        PreviousRakelRotation = NO_VALUE;
        PreviousRakelTilt = NO_VALUE;

        TransferEngine.NewStroke(tiltNoiseEnabled, tiltNoiseFrequency, tiltNoiseAmplitude, floatingZLength, csbEnabled);
    }

    public void AddNode(Vector3 rakelPosition, bool autoZEnabled, float rakelPressure, float rakelRotation, float rakelTilt, TransferConfiguration transferConfig, int interpolationResolution)
    {
        // only reapply if there are changes
        if (!rakelPosition.Equals(PreviousRakelPosition)
            || !rakelPressure.Equals(PreviousRakelPressure)
            || !rakelRotation.Equals(PreviousRakelRotation)
            || !rakelTilt.Equals(PreviousRakelTilt))
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            bool isFirstNodeOfStroke = PreviousRakelPosition.Equals(NO_POSITION)
                && PreviousRakelPressure.Equals(NO_VALUE)
                && PreviousRakelRotation.Equals(NO_VALUE)
                && PreviousRakelTilt.Equals(NO_VALUE);
            if (isFirstNodeOfStroke)
            {
                TransferEngine.EnqueueOrRun(
                    rakelPosition,
                    autoZEnabled,
                    rakelPressure,
                    rakelRotation,
                    rakelTilt,
                    transferConfig
                );
            }
            else
            {
                // 1. determine differences and steps
                Vector3 dp = rakelPosition - PreviousRakelPosition;
                //float dpLength = dp.magnitude;
                Vector2 dp_ = Canvas.MapToPixel(rakelPosition) - Canvas.MapToPixel(PreviousRakelPosition);
                float dpLength = dp_.magnitude;
                int positionSteps = (int)(dpLength * interpolationResolution); // don't add 1 because the first one is already done when isFirstNodeOfStroke

                float dpr = rakelPressure - PreviousRakelPressure;

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
                float arcLength = Mathf.PI * (Rakel.Info.Length / 2) * (Mathf.Abs(dr)/180);
                int rotationSteps = (int)(arcLength * interpolationResolution);

                float dt = rakelTilt - PreviousRakelTilt;
                arcLength = Mathf.PI * Rakel.Info.Width * (Mathf.Abs(dt)/180);
                int tiltSteps = (int)(arcLength * interpolationResolution);

                int steps = Mathf.Max(1, Mathf.Max(Mathf.Max(positionSteps, rotationSteps), tiltSteps));


                // 2. interpolate
                Vector3 previousPosition = PreviousRakelPosition;
                float previousPressure = PreviousRakelPressure;
                float previousRotation = PreviousRakelRotation;
                float previousTilt = PreviousRakelTilt;

                for (int i=0; i<steps; i++)
                {
                    // first one is skipped, because that was already done when isFirstNodeOfStroke

                    Vector3 currentPosition = previousPosition + dp / steps;
                    //Vector3 currentPosition = PreviousRakelPosition + (i+1) * (dp / steps); // doesn't seem to make a difference

                    float currentPressure = previousPressure + dpr / steps;

                    float currentRotation = previousRotation + dr / steps;
                    if (currentRotation >= 360) { // fix turnover case 1
                        currentRotation = currentRotation % 360;
                    }
                    if (currentRotation < 0) { // fix turnover case 2
                        currentRotation = 360 + currentRotation;
                    }

                    float currentTilt = previousTilt + dt / steps;

                    TransferEngine.EnqueueOrRun(
                        currentPosition,
                        autoZEnabled,
                        currentPressure,
                        currentRotation,
                        currentTilt,
                        transferConfig
                    );

                    previousPosition = currentPosition;
                    previousRotation = currentRotation;
                    previousTilt = currentTilt;
                }
            }

            PreviousRakelPosition = rakelPosition;
            PreviousRakelPressure = rakelPressure;
            PreviousRakelRotation = rakelRotation;
            PreviousRakelTilt = rakelTilt;
            //if (logTime)
            //    UnityEngine.Debug.Log("UpdatePosition took " + sw.ElapsedMilliseconds + "ms");
        }
    }
}
