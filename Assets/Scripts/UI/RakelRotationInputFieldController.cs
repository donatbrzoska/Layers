using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class RakelRotationInputFieldController : InputFieldController
{
    public void Start()
    {
        InputField.text = "" + MathUtil.Angle360(Vector2.right, OilPaintEngine.RakelNormal);
    }

    override public void OnValueChanged(string arg0)
    {
        int value = int.Parse(arg0);
        Vector2 normal = MathUtil.RotateAroundOrigin(Vector2.right, value);
        OilPaintEngine.UpdateRakelNormal(normal);
    }
}