using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class DiscardVolumeInputFieldController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Configuration.ReservoirDiscardVolumeThreshold);
    }

    override public void OnValueChanged(string arg0)
    {
        int value = int.Parse(arg0);
        OilPaintEngine.UpdateReservoirDiscardVolumeThreshold(value);
    }
}