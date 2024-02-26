using UnityEngine;

public class MouseControl : MonoBehaviour,
    PositionXSource, PositionYSource, RotationSource, StrokeStateSource
{
    public float PositionX { get; private set; }
    public float PositionY { get; private set; }
    public float Rotation { get; private set; }
    public bool StrokeBegin { get; private set; }
    public bool InStroke { get; private set; }

    private AutoRotation AutoRotation;
    private GraphicsRaycaster GraphicsRaycaster;

    void Start()
    {
        AutoRotation = new AutoRotation();
        GraphicsRaycaster = GameObject.Find("UI").GetComponent<GraphicsRaycaster>();
    }

    void Update()
    {
        Vector3 mousePosition = ScreenToWorld.Convert(Input.mousePosition);
        PositionX = mousePosition.x;
        PositionY = mousePosition.y;

        AutoRotation.Update(mousePosition);
        Rotation = AutoRotation.Rotation;

        if (Input.GetMouseButton(0))
        {
            StrokeBegin = Input.GetMouseButtonDown(0) && !GraphicsRaycaster.UIBlocking(Input.mousePosition);
            if (StrokeBegin)
            {
                InStroke = true;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            InStroke = false;
        }
    }
}
