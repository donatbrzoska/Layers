using UnityEngine.InputSystem;

public class PenRakelRotation : FloatValueSource
{
    private AutoRotation AutoRotation;

    public PenRakelRotation()
    {
        AutoRotation = new AutoRotation();
    }

    public override void Update()
    {
        AutoRotation.Update(Pen.current.position.ReadValue());
        Value = AutoRotation.Value;
    }
}
