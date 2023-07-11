using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PenRakelPositionRaycaster
{
    private static Camera Camera = GameObject.Find("Main Camera").GetComponent<Camera>();

    private static int WallColliderID = GameObject.Find("Wall").GetComponent<MeshCollider>().GetInstanceID();
    private static int CanvasColliderID = GameObject.Find("Canvas").GetComponent<MeshCollider>().GetInstanceID();

    public static Vector3 Raycast()
    {
        // determine mouse position on canvas
        RaycastHit hit;
        Ray ray = Camera.ScreenPointToRay(Pen.current.position.ReadValue());
        Physics.Raycast(ray, out hit);

        Vector3 result = Vector3.zero;

        if (hit.colliderInstanceID == WallColliderID || hit.colliderInstanceID == CanvasColliderID)
        {
            result = hit.point;
        }

        return result;
    }
}
