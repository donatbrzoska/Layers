using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class RakelResolutionInputFieldController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Configuration.RakelResolution);
    }

    override public void OnValueChanged(string arg0)
    {
        int value = int.Parse(arg0);
        OilPaintEngine.UpdateRakelResolution(value);
    }
}