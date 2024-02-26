using UnityEngine;

public class RenderedRakel : MonoBehaviour
{
    public GameObject _InputManager;

    private OilPaintEngine OilPaintEngine;
    private InputManager InputManager;

    private Quaternion BaseRotation;

    void Start()
    {
        OilPaintEngine = GameObject.Find("OilPaintEngine").GetComponent<OilPaintEngine>();
        InputManager = _InputManager.GetComponent<InputManager>();

        BaseRotation = transform.rotation;
    }

    void Update()
    {
        Vector3 scale = new Vector3(OilPaintEngine.Rakel.Info.Width, transform.localScale.y, OilPaintEngine.Rakel.Info.Length);
        transform.localScale = scale;

        bool inStroke = !OilPaintEngine.TransferEngine.IsDone();
        float positionX = inStroke ? OilPaintEngine.Rakel.Info.Position.x : InputManager.PositionX;
        float positionY = inStroke ? OilPaintEngine.Rakel.Info.Position.y : InputManager.PositionY;
        float positionZ = InputManager.PositionBaseZ;
        float rotation = inStroke ? OilPaintEngine.Rakel.Info.Rotation : InputManager.Rotation;
        float tilt = inStroke ? OilPaintEngine.Rakel.Info.Tilt : InputManager.Tilt;

        Vector3 position = new Vector3(positionX, positionY, positionZ);
        transform.position = position - Quaternion.AngleAxis(rotation, Vector3.back) * OilPaintEngine.Rakel.Info.Anchor;

        // Rotations have to be transformed, because the rendered rakel model has a different base orientation (flat in xz-plane)
        transform.rotation = BaseRotation * Quaternion.Euler(new Vector3(0, rotation, -tilt));
    }
}
