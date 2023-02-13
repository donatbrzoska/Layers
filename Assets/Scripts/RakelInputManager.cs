using UnityEngine;

public class RakelInputManager
{
    // Position attributes
    public bool YPositionLocked { get; set; }

    private Camera Camera;
    private GraphicsRaycaster GraphicsRaycaster;

    private int[] ColliderIDs;

    // Rotation attributes
    public bool RotationLocked { get; set; }

    private bool PreviousMousePositionInitialized;
    private Vector2 PreviousMousePosition;
    private float CurrentRotation;

    public RakelInputManager(params int[] colliderIDs)
    {
        Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        GraphicsRaycaster = GameObject.Find("UI").GetComponent<GraphicsRaycaster>();

        ColliderIDs = colliderIDs;
    }

    public Vector3 GetPosition()
    {
        Vector3 result = Vector3.negativeInfinity;
        if (!GraphicsRaycaster.UIBlocking() && Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hit);

            foreach (int id in ColliderIDs)
            {
                if (hit.colliderInstanceID == id)
                {
                    result = hit.point;
                    if (YPositionLocked)
                    {
                        result.y = 0;
                    }
                    break;
                }
            }
        }
        return result;
    }

    public float GetRotation()
    {
        if (!RotationLocked)
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
                    CurrentRotation = angle;

                    PreviousMousePosition = currentMousePosition;
                }
            }
        }
        return CurrentRotation;
    }
}
