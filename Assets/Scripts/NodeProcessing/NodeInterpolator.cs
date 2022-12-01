using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

//public struct Node
//{
//    public Vector3 Position { get; }
//    public float Rotation { get; }
//    public float Tilt { get; }

//    public Node(Vector3 position, float rotation, float tilt) : this()
//    {
//        Position = position;
//        Rotation = rotation;
//        Tilt = tilt;
//    }
//}

public class NodeInterpolator
{
    private RenderTexture DrawingTarget;
    private IRakel Rakel;

    private Vector3 PreviousRakelPosition;
    private float PreviousRakelRotation;
    private float PreviousRakelTilt;

    private float NO_ANGLE = float.NaN;
    private Vector3 NO_POSITION = Vector3.negativeInfinity;

    public NodeInterpolator(IRakel rakel, RenderTexture drawingTarget)
    {
        Rakel = rakel;
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
                //NodeSink.Add(new Node(rakelPosition, rakelRotation, rakelTilt));
                Rakel.Apply(rakelPosition, rakelRotation, rakelTilt, DrawingTarget);
            }
            else
            {
                // 1. determine steps
                Vector3 dp = rakelPosition - PreviousRakelPosition;
                float dpLength = dp.magnitude;
                int positionSteps = (int)(dpLength * interpolationResolution);

                float dr = Mathf.Abs(PreviousRakelRotation - rakelRotation);
                int rotationSteps = (int)(dr * interpolationResolution); // TODO maybe adjust

                float dt = Mathf.Abs(PreviousRakelTilt - rakelTilt);
                int tiltSteps = (int)(dt * interpolationResolution); // TODO maybe adjust

                int steps = Mathf.Max(Mathf.Max(positionSteps, rotationSteps), tiltSteps);


                // 2. interpolate
                Vector3 previousPosition = PreviousRakelPosition;
                float previousRotation = PreviousRakelRotation;
                float previousTilt = PreviousRakelTilt;

                for (int i=0; i<steps; i++)
                {
                    // skip first one, because that was already added with new stroke call
                    Vector3 currentPosition = previousPosition + dp / steps;
                    float currentRotation = previousRotation + dr / steps;
                    float currentTilt = previousTilt + dt / steps;

                    Rakel.Apply(currentPosition, previousRotation, previousTilt, DrawingTarget);
                    //NodeSink.Add(new Node(currentPosition, currentRotation, currentTilt));

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
