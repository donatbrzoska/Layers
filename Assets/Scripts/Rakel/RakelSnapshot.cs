using UnityEngine;

public class RakelSnapshot
{
    public Vector3 Position { get; private set; }
    public float Rotation { get; private set; }

    public Vector3 UpperLeft { get; private set; }
    public Vector3 UpperRight { get; private set; }
    public Vector3 LowerLeft { get; private set; }
    public Vector3 LowerRight { get; private set; }

    public Vector3 ulTilted { get; private set; }
    public Vector3 urTilted { get; private set; }
    public Vector3 llTilted { get; private set; }
    public Vector3 lrTilted { get; private set; }

    public RakelSnapshot(float length, float width, Vector3 anchor, Vector3 position, float rotation, float tilt){
        Position = position;
        Rotation = rotation;

        Vector3 ulOrigin = new Vector3(0, length, 0);
        Vector3 urOrigin = new Vector3(width, length, 0);
        Vector3 llOrigin = new Vector3(0, 0, 0);
        Vector3 lrOrigin = new Vector3(width, 0, 0);

        Quaternion tiltQuaternion = Quaternion.AngleAxis(tilt, Vector3.up);
        ulTilted = tiltQuaternion * (ulOrigin - anchor) + anchor; // tilt around anchor
        urTilted = tiltQuaternion * (urOrigin - anchor) + anchor; // tilt around anchor
        llTilted = tiltQuaternion * (llOrigin - anchor) + anchor; // tilt around anchor
        lrTilted = tiltQuaternion * (lrOrigin - anchor) + anchor; // tilt around anchor

        Quaternion rotationQuaternion = Quaternion.AngleAxis(rotation, Vector3.back);
        Vector3 ulRotated = rotationQuaternion * (ulTilted - anchor) + anchor; // rotate around anchor
        Vector3 urRotated = rotationQuaternion * (urTilted - anchor) + anchor; // rotate around anchor
        Vector3 llRotated = rotationQuaternion * (llTilted - anchor) + anchor; // rotate around anchor
        Vector3 lrRotated = rotationQuaternion * (lrTilted - anchor) + anchor; // rotate around anchor

        Vector3 positionTranslation = position - anchor;
        UpperLeft = ulRotated + positionTranslation;
        UpperRight = urRotated + positionTranslation;
        LowerLeft = llRotated + positionTranslation;
        LowerRight = lrRotated + positionTranslation;
    }
}