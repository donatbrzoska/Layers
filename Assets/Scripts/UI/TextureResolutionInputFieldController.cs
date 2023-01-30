using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class TextureResolutionInputFieldController : InputFieldController
{
    public void Start()
    {
        InputField.text = "" + OilPaintEngine.TextureResolution;
    }

    override public void OnValueChanged(string arg0)
    {
        int value = int.Parse(arg0);
        OilPaintEngine.UpdateTextureResolution(value);
    }
}