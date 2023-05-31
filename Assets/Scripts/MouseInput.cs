using UnityEngine;

public class MouseInput
{
    private Camera Camera;
    private GraphicsRaycaster GraphicsRaycaster;

    private int[] ColliderIDs;

    public MouseInput(params int[] colliderIDs)
    {
        Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        GraphicsRaycaster = GameObject.Find("UI").GetComponent<GraphicsRaycaster>();

        ColliderIDs = colliderIDs;
    }

    public void Update()
    {
        UpdatePosition();
        UpdateCanvasHit();
        UpdateStrokeBegin();
        UpdateRotation();
    }


    // only valid if CanvasHit is true
    public Vector3 Position { get; private set; }

    private void UpdatePosition()
    {
        Position = Vector3.negativeInfinity;
        if (!GraphicsRaycaster.UIBlocking() && Input.GetMouseButton(0))
        {
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
        }
    }


    public bool CanvasHit { get; private set; }

    private void UpdateCanvasHit()
    {
        if (!Position.Equals(Vector3.negativeInfinity))
        {
            CanvasHit = true;
        }
    }


    public bool StrokeBegin { get; private set; }

    private void UpdateStrokeBegin()
    {
        if (CanvasHit)
        {
            StrokeBegin = Input.GetMouseButtonDown(0);
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
}
