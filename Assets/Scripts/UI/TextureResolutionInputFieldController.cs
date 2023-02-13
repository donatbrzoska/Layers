using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class TextureResolutionInputFieldController : InputFieldController
{
    public void Start()
    {
        InputField.SetTextWithoutNotify("" + OilPaintEngine.Configuration.CanvasResolution);
    }

    override public void OnValueChanged(string arg0)
    {
        int value = int.Parse(arg0);
        OilPaintEngine.UpdateTextureResolution(value);
    }
}