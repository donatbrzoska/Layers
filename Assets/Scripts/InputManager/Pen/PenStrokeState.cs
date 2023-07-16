using UnityEngine;
using UnityEngine.InputSystem;

public class PenStrokeState : StrokeStateSource
{
    private GraphicsRaycaster GraphicsRaycaster;

    public PenStrokeState()
    {
        GraphicsRaycaster = GameObject.Find("UI").GetComponent<GraphicsRaycaster>(); ;
    }

    public override void Update()
    {
        DrawingEnabled = Pen.current.pressure.ReadValue() > 0;


        if (DrawingEnabled)
        {
            StrokeBegin = Pen.current.press.wasPressedThisFrame && !GraphicsRaycaster.UIBlocking(Pen.current.position.ReadValue());
            if (StrokeBegin)
            {
                InStroke = true;
            }
        }


        if (Pen.current.press.wasReleasedThisFrame)
        {
            InStroke = false;
        }
    }
}
