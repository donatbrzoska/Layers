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
