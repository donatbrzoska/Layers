
public class RakelRotationController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.InputManager.RakelRotation);
    }

    public void Update()
    {
        if (!OilPaintEngine.RakelRotationLocked)
        {
            InputField.SetTextWithoutNotify("" + OilPaintEngine.InputManager.RakelRotation);
        }
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateRakelRotation(value);
    }
}