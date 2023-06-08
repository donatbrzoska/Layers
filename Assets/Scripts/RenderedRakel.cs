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
        Vector3 scale = new Vector3(OilPaintEngine.Rakel.Width, transform.localScale.y, OilPaintEngine.Rakel.Length);
        transform.localScale = scale;

        Vector3 position = new Vector3(
            OilPaintEngine.InputManager.RakelPositionX,
            OilPaintEngine.InputManager.RakelPositionY,
            OilPaintEngine.InputManager.RakelPositionZ);
        transform.position = position - Quaternion.AngleAxis(
            OilPaintEngine.InputManager.RakelRotation,
            Vector3.back) * OilPaintEngine.Rakel.Anchor;

        // Rotations have to be transformed, because the rendered rakel model has a different base orientation (flat in xz-plane)
        transform.rotation = BaseRotation * Quaternion.Euler(new Vector3(
            0,
            OilPaintEngine.InputManager.RakelRotation,
            -OilPaintEngine.InputManager.RakelTilt));
    }
}
