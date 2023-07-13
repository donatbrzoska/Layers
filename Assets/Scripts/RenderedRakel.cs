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

        bool inStroke = !OilPaintEngine.TransferEngine.Done();
        Vector3 position = new Vector3(
            inStroke ? OilPaintEngine.Rakel.Info.Position.x : OilPaintEngine.InputManager.RakelPositionX,
            inStroke ? OilPaintEngine.Rakel.Info.Position.y : OilPaintEngine.InputManager.RakelPositionY,
            OilPaintEngine.InputManager.RakelPositionZ);
        transform.position = position - Quaternion.AngleAxis(
            inStroke ? OilPaintEngine.Rakel.Info.Rotation : OilPaintEngine.InputManager.RakelRotation,
            Vector3.back) * OilPaintEngine.Rakel.Info.Anchor;

        // Rotations have to be transformed, because the rendered rakel model has a different base orientation (flat in xz-plane)
        transform.rotation = BaseRotation * Quaternion.Euler(new Vector3(
            0,
            inStroke ? OilPaintEngine.Rakel.Info.Rotation : OilPaintEngine.InputManager.RakelRotation,
            inStroke ? -OilPaintEngine.Rakel.Info.Tilt : -OilPaintEngine.InputManager.RakelTilt));
    }
}
