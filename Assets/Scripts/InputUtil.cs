using UnityEngine;

public class InputUtil
{
    private InputUtil()
    {
    }

    public static Vector3 GetMouseHit(Camera camera, int colliderID)
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hit);

            if (hit.colliderInstanceID == colliderID)
            {
                return hit.point;
            }
        }
        return Vector3.negativeInfinity;
    }
}
