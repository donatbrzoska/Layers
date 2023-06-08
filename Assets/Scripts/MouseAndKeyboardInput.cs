using UnityEngine;

public class MouseAndKeyboardInput
{
    private Camera Camera;
    private GraphicsRaycaster GraphicsRaycaster;

    private int[] ColliderIDs;

    public MouseAndKeyboardInput(params int[] colliderIDs)
    {
        Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        GraphicsRaycaster = GameObject.Find("UI").GetComponent<GraphicsRaycaster>();

        ColliderIDs = colliderIDs;
    }

    public void Update()
    {
        UpdatePosition();
        UpdateDrawingEnabled();
        UpdateStrokeBegin();
        UpdateInStroke();
        UpdateRotation();
        UpdateTilt();
    }


    public Vector3 Position { get; private set; }

    private const float POSITION_Z_STEP = 0.1f;
    private float positionZ;

    private void UpdatePosition()
    {
        // determine mouse position on canvas
        RaycastHit hit;
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit);

        foreach (int id in ColliderIDs)
        {
            if (hit.colliderInstanceID == id)
            {
                Position = hit.point;
                break;
            }
        }

        // process keyboard input
        if (Input.GetKey(KeyCode.Q))
        {
            positionZ += POSITION_Z_STEP;
        }
        if (Input.GetKey(KeyCode.W))
        {
            positionZ -= POSITION_Z_STEP;
        }

        Vector3 p = Position;
        p.z = positionZ;
        Position = p;
    }


    // NOTE: Real haptic input controllers probably won't need this and should always return true
    public bool DrawingEnabled { get; private set; }

    private void UpdateDrawingEnabled()
    {
        DrawingEnabled = Input.GetMouseButton(0);
    }


    public bool StrokeBegin { get; private set; }

    private void UpdateStrokeBegin()
    {
        if (DrawingEnabled)
        {
            StrokeBegin = Input.GetMouseButtonDown(0) && !GraphicsRaycaster.UIBlocking();
            if (StrokeBegin)
            {
                InStroke = true;
            }
        }
    }


    public bool InStroke { get; private set; }

    private void UpdateInStroke()
    {
        if (Input.GetMouseButtonUp(0))
        {
            InStroke = false;
        }
    }


    public float Rotation { get; private set; }

    private bool PreviousMousePositionInitialized;
    private Vector2 PreviousMousePosition;

    private void UpdateRotation()
    {
        Vector2 currentMousePosition = Input.mousePosition;
        if (!PreviousMousePositionInitialized)
        {
            PreviousMousePosition = currentMousePosition;
            PreviousMousePositionInitialized = true;
        }
        else
        {
            Vector2 direction = currentMousePosition - PreviousMousePosition;

            if (direction.magnitude > 8)
            {
                float angle = MathUtil.Angle360(Vector2.right, direction);
                Rotation = angle;

                PreviousMousePosition = currentMousePosition;
            }
        }
    }


    public float Tilt { get; private set; }

    private float TILT_STEP = 0.5f;

    private void UpdateTilt()
    {
        if (Input.GetKey(KeyCode.A))
        {
            Tilt += TILT_STEP;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Tilt -= TILT_STEP;
        }

        Tilt = Rakel.ClampTilt(Tilt);
    }
}
