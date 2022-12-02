using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class RakelRotationInputFieldController : InputFieldController
{
    public void Start()
    {
        InputField.text = "" + OilPaintEngine.RakelRotation;
    }

    override public void OnValueChanged(string arg0)
    {
        int value = int.Parse(arg0);
        OilPaintEngine.UpdateRakelRotation(value);
    }
}