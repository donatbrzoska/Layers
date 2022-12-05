using UnityEngine;

public class RakelSnapshot
{
    public Vector3 Position { get; private set; }
    public float Rotation { get; private set; }
    public Vector3 UpperLeft { get; private set; }
    public Vector3 UpperRight { get; private set; }
    public Vector3 LowerLeft { get; private set; }
    public Vector3 LowerRight { get; private set; }
    public Vector2 OriginBoundaries { get; private set; }

    public RakelSnapshot(float length, float width, Vector3 anchor, Vector3 position, float rotation, float tilt){
        Position = position;
        Rotation = rotation;

        Vector3 ulOrigin = new Vector3(0, length, 0);
        Vector3 urOrigin = new Vector3(width, length, 0);
        Vector3 llOrigin = new Vector3(0, 0, 0);
        Vector3 lrOrigin = new Vector3(width, 0, 0);

        Quaternion tiltQuaternion = Quaternion.AngleAxis(tilt, Vector3.up);
        Vector3 urTilted = tiltQuaternion * urOrigin;
        Vector3 lrTilted = tiltQuaternion * lrOrigin;

        Quaternion rotationQuaternion = Quaternion.AngleAxis(rotation, Vector3.back);
        Vector3 ulRotated = rotationQuaternion * (ulOrigin - anchor) + anchor; // rotate around anchor
        Vector3 urRotated = rotationQuaternion * (urTilted - anchor) + anchor; // rotate around anchor
        Vector3 llRotated = rotationQuaternion * (llOrigin - anchor) + anchor; // rotate around anchor
        Vector3 lrRotated = rotationQuaternion * (lrTilted - anchor) + anchor; // rotate around anchor

        Vector3 positionTranslation = position - anchor;
        UpperLeft = ulRotated + positionTranslation;
        UpperRight = urRotated + positionTranslation;
        LowerLeft = llRotated + positionTranslation;
        LowerRight = lrRotated + positionTranslation;

        OriginBoundaries = new Vector2(urTilted.x, urTilted.y);
    }
}