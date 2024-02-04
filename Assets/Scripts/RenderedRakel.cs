using UnityEngine;

public class RenderedRakel : MonoBehaviour
{
    private OilPaintEngine OilPaintEngine;

    private Quaternion BaseRotation;

    void Start()
    {
        OilPaintEngine = GameObject.Find("OilPaintEngine").GetComponent<OilPaintEngine>();

        BaseRotation = transform.rotation;
    }

    void Update()
    {
        Vector3 scale = new Vector3(OilPaintEngine.Rakel.Info.Width, transform.localScale.y, OilPaintEngine.Rakel.Info.Length);
        transform.localScale = scale;

        bool inStroke = !OilPaintEngine.TransferEngine.IsDone();
        float positionX = inStroke ? OilPaintEngine.Rakel.Info.Position.x : OilPaintEngine.InputManager.RakelPositionX;
        float positionY = inStroke ? OilPaintEngine.Rakel.Info.Position.y : OilPaintEngine.InputManager.RakelPositionY;
        float positionZ = OilPaintEngine.InputManager.RakelPositionBaseZ;
        float rotation = inStroke ? OilPaintEngine.Rakel.Info.Rotation : OilPaintEngine.InputManager.RakelRotation;
        float tilt = inStroke ? OilPaintEngine.Rakel.Info.Tilt : OilPaintEngine.InputManager.RakelTilt;

        Vector3 position = new Vector3(positionX, positionY, positionZ);
        transform.position = position - Quaternion.AngleAxis(rotation, Vector3.back) * OilPaintEngine.Rakel.Info.Anchor;

        // Rotations have to be transformed, because the rendered rakel model has a different base orientation (flat in xz-plane)
        transform.rotation = BaseRotation * Quaternion.Euler(new Vector3(0, rotation, -tilt));
    }
}
