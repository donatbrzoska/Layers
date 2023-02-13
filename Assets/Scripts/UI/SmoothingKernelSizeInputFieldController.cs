using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class SmoothingKernelSizeInputFieldController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Configuration.ReservoirSmoothingKernelSize);
    }

    override public void OnValueChanged(string arg0)
    {
        int value = int.Parse(arg0);
        OilPaintEngine.UpdateReservoirSmoothingKernelSize(value);
    }
}