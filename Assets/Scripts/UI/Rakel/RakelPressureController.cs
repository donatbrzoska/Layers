
public class RakelPressureController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.InputManager.RakelPressure);
    }

    public void Update()
    {
        if (!OilPaintEngine.RakelPressureLocked)
        {
            InputField.SetTextWithoutNotify("" + OilPaintEngine.InputManager.RakelPressure);
        }
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateRakelPressure(value);
    }
}