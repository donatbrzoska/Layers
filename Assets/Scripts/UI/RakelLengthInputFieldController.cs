using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class RakelLengthInputFieldController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.RakelLength);
    }

    override public void OnValueChanged(string arg0)
    {
        float value = float.Parse(arg0);
        OilPaintEngine.UpdateRakelLength(value);
    }
}