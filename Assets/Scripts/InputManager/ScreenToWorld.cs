using UnityEngine;

public class ScreenToWorld
{
    private static Camera Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    //private static GameObject Canvas = GameObject.Find("Canvas");

    public static Vector3 Convert(Vector3 screenPosition)
    {
        // HACK:
        // Need to fetch Canvas every time. This is because for some reason,
        // changing its transform.localScale in OilPaintEngine does not have an effect,
        // so we need to recreate it every time
        GameObject Canvas = GameObject.FindGameObjectWithTag("Canvas");
        return Camera.ScreenToWorldPoint(
            new Vector3(
                screenPosition.x,
                screenPosition.y,
                (Canvas.transform.position - Camera.transform.position).magnitude));
    }
}
