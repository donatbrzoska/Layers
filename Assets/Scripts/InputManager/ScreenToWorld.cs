using UnityEngine;

public class ScreenToWorld
{
    private static Camera Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    private static GameObject Canvas = GameObject.Find("Canvas");

    public static Vector3 Convert(Vector3 screenPosition)
    {
        return Camera.ScreenToWorldPoint(
            new Vector3(
                screenPosition.x,
                screenPosition.y,
                (Canvas.transform.position - Camera.transform.position).magnitude));
    }
}
