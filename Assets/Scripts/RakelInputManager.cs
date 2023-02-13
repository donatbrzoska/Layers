using UnityEngine;

public class RakelInputManager
{
    private Camera Camera;
    private GraphicsRaycaster GraphicsRaycaster;

    private int[] ColliderIDs;

    public RakelInputManager(params int[] colliderIDs)
    {
        Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        GraphicsRaycaster = GameObject.Find("UI").GetComponent<GraphicsRaycaster>();

        ColliderIDs = colliderIDs;
    }

    public Vector3 GetPosition()
    {
        if (!GraphicsRaycaster.UIBlocking() && Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hit);

            foreach (int id in ColliderIDs)
            {
                if (hit.colliderInstanceID == id)
                    return hit.point;
            }
        }
        return Vector3.negativeInfinity;
    }
}
