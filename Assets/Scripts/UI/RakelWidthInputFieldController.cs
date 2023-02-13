using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class RakelWidthInputFieldController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Configuration.RakelWidth);
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateRakelWidth(value);
    }
}