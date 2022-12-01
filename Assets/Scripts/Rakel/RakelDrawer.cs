using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class RakelDrawer
{
    //IRakel Rakel;

    //private Vector3 PreviousRakelPosition;
    //private float PreviousRakelRotation;
    //private float PreviousRakelTilt;

    //private float NO_ROTATION = float.NaN;
    //private float NO_TILT = float.NaN;
    //private Vector3 NO_POSITION = Vector3.negativeInfinity;

    //public RakelDrawer(IRakel rakel)
    //{
    //    Rakel = rakel;
    //}

    //public void NewStroke()
    //{
    //    PreviousRakelPosition = NO_POSITION;
    //    PreviousRakelRotation = NO_ROTATION;
    //    PreviousRakelTilt = NO_TILT;
    //}

    //public void AddNode(IOilPaintSurface oilPaintSurface, Vector2Int position, Vector2 normal, bool logTime = false)
    //{
    //    Stopwatch sw = new Stopwatch();
    //    sw.Start();

    //    // only reapply if there are changes
    //    if (!position.Equals(PreviousRakelPosition) || !normal.Equals(PreviousRakelNormal))
    //    {
    //        bool isFirstNodeOfStroke = PreviousRakelPosition.Equals(NO_POSITION) && PreviousRakelNormal.Equals(NO_NORMAL);
    //        if (isFirstNodeOfStroke)
    //        {
    //            Rakel.UpdateNormal(normal, logTime);
    //            Rakel.ApplyAt(oilPaintSurface, position, logTime);
    //        }
    //        else
    //        {
    //            // 1. interpolate positions
    //            List<Vector2Int> positions;
    //            positions = Bresenham(PreviousRakelPosition.x, PreviousRakelPosition.y, position.x, position.y);

    //            if (positions.Count > 1) // in this case, the same position was used but with a different normal
    //            {
    //                // prevent apply overlap on interpolation nodes, because Bresenham also adds the start position
    //                positions.RemoveAt(0); // TODO optimize this to O(1), LinkedList may be enough even
    //            }

    //            // 2. interpolate normals
    //            List<Vector2> normals = new List<Vector2>();
    //            int n_neededInterpolatedNormals = positions.Count - 1; // first one is left out, last one will always be added
    //            if (n_neededInterpolatedNormals > 0) // this is just for readability though
    //            {
    //                float targetAngle = MathUtil.Angle360(PreviousRakelNormal, normal);
    //                float angleStepSize = targetAngle / (n_neededInterpolatedNormals + 1);

    //                float currentAngle = 0;
    //                for (int i = 0; i < n_neededInterpolatedNormals; i++)
    //                {
    //                    currentAngle += angleStepSize;
    //                    Vector2 interpolatedNormal = MathUtil.RotateAroundOrigin(PreviousRakelNormal, currentAngle);
    //                    normals.Add(interpolatedNormal);
    //                }
    //            }
    //            normals.Add(normal); // just add the target normal to prevent rounding errors

    //            // 3. do stroke
    //            for (int i = 0; i < positions.Count; i++)
    //            {
    //                Rakel.UpdateNormal(normals[i], logTime);
    //                Rakel.ApplyAt(oilPaintSurface, positions[i], logTime);
    //            }
    //        }

    //        PreviousRakelNormal = normal;
    //        PreviousRakelPosition = position;
    //        if (logTime)
    //            UnityEngine.Debug.Log("UpdatePosition took " + sw.ElapsedMilliseconds + "ms");
    //    }
    //}

    //// TODO do one function for both mask applicator and RakelDrawer
    //// Bresenham from https://stackoverflow.com/a/11683720
    //private List<Vector2Int> Bresenham(int x1, int y1, int x2, int y2)
    //{
    //    List<Vector2Int> coordinates = new List<Vector2Int>();

    //    int w = x2 - x1;
    //    int h = y2 - y1;
    //    int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
    //    if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
    //    if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
    //    if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
    //    int longest = Mathf.Abs(w);
    //    int shortest = Mathf.Abs(h);
    //    if (!(longest > shortest))
    //    {
    //        longest = Mathf.Abs(h);
    //        shortest = Mathf.Abs(w);
    //        if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
    //        dx2 = 0;
    //    }
    //    int numerator = longest >> 1;
    //    for (int i = 0; i <= longest; i++)
    //    {
    //        coordinates.Add(new Vector2Int(x1, y1));

    //        numerator += shortest;
    //        if (!(numerator < longest))
    //        {
    //            numerator -= longest;
    //            x1 += dx1;
    //            y1 += dy1;
    //        }
    //        else
    //        {
    //            x1 += dx2;
    //            y1 += dy2;
    //        }
    //    }

    //    return coordinates;
    //}
}
