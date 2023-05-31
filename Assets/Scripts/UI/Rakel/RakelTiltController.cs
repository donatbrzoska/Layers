
public class RakelTiltController : InputFieldController
{
    public void Update()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.RakelTilt);
    }

    override public void OnValueChanged(string arg0)
    {
        int value = int.Parse(arg0);
        OilPaintEngine.UpdateRakelTilt(value);
    }
}