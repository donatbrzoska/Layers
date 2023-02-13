using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class RakelRotationInputFieldController : InputFieldController
{
    public void Update()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.RakelInputManager.Rotation);
    }

    override public void OnValueChanged(string arg0)
    {
        int value = int.Parse(arg0);
        OilPaintEngine.UpdateRakelRotation(value);
    }
}