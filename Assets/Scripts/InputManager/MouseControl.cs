using UnityEngine;
using UnityEngine.InputSystem;

public class MouseControl : MonoBehaviour,
    PositionXSource, PositionYSource, RotationSource, StrokeStateSource
{
    public bool Active;
    public bool ChangeDetected;

    public float PositionX { get; private set; }
    public float PositionY { get; private set; }
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
        Vector3 mousePosition = ScreenToWorld.Convert(Mouse.current.position.ReadValue());

        ChangeDetected = mousePosition != PreviousPosition;
        PreviousPosition = mousePosition;

        if (Active)
        {
            PositionX = mousePosition.x;
            PositionY = mousePosition.y;

            AutoRotation.Update(mousePosition);
            Rotation = AutoRotation.Rotation;
            
            if (Mouse.current.leftButton.isPressed)
            {
                StrokeBegin = Mouse.current.leftButton.wasPressedThisFrame &&
                              !GraphicsRaycaster.UIBlocking(Mouse.current.position.ReadValue());
                if (StrokeBegin)
                {
                    InStroke = true;
                }
            }
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                InStroke = false;
            }
        }
    }
}
