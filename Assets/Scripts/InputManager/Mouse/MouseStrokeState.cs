using UnityEngine;

public class MouseStrokeState : StrokeStateSource
{
    private GraphicsRaycaster GraphicsRaycaster;

    public MouseStrokeState()
    {
        GraphicsRaycaster = GameObject.Find("UI").GetComponent<GraphicsRaycaster>(); ;
    }

    public override void Update()
    {
        DrawingEnabled = Input.GetMouseButton(0);


        if (DrawingEnabled)
        {
            StrokeBegin = Input.GetMouseButtonDown(0) && !GraphicsRaycaster.UIBlocking();
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
