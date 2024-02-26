using UnityEngine;
using UnityEngine.InputSystem;

public class PenControl : MonoBehaviour,
    PositionXSource, PositionYSource, RotationSource, PressureSource, StrokeStateSource
{
    public bool Active;
    public bool ChangeDetected;

    public float PositionX { get; private set; }
    public float PositionY { get; private set; }
    public float Pressure { get; private set; }
    public float Rotation { get; private set; }
    public bool StrokeBegin { get; private set; }
    public bool InStroke { get; private set; }

    private Vector3 PreviousPosition;

    private AutoRotation AutoRotation;
    private GraphicsRaycaster GraphicsRaycaster;

    void Start()
    {
        AutoRotation = new AutoRotation();
        GraphicsRaycaster = GameObject.Find("UI").GetComponent<GraphicsRaycaster>();
    }

    void Update()
    {
        if (Pen.current == null)
        {
            return;
        }

        Vector3 penPosition = ScreenToWorld.Convert(Pen.current.position.ReadValue());

        ChangeDetected = penPosition != PreviousPosition;
        PreviousPosition = penPosition;

        if (Active)
        {
            PositionX = penPosition.x;
            PositionY = penPosition.y;

            Pressure = Mathf.Clamp01(Pen.current.pressure.ReadValue());

            AutoRotation.Update(penPosition);
            Rotation = AutoRotation.Rotation;

            if (Pen.current.pressure.ReadValue() > 0)
            {
                StrokeBegin = Pen.current.press.wasPressedThisFrame &&
                              !GraphicsRaycaster.UIBlocking(Pen.current.position.ReadValue());
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
}
