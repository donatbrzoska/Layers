using System;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class RakelEmitModeDropdownController : MonoBehaviour
{
    OilPaintEngine OilPaintEngine;

    TMP_Dropdown EmitModeDropdown;

    public void Awake()
    {
        OilPaintEngine = GameObject.Find("OilPaintEngine").GetComponent<OilPaintEngine>();

        EmitModeDropdown = GameObject.Find("Rakel Emit Mode Dropdown").GetComponent<TMP_Dropdown>();
        EmitModeDropdown.AddOptions(Enum.GetNames(typeof(EmitMode)).ToList());
        EmitModeDropdown.onValueChanged.AddListener(OnValueChanged);
    }

    public void Start()
    {
        EmitModeDropdown.SetValueWithoutNotify((int)OilPaintEngine.RakelEmitMode);
    }

    public void OnValueChanged(int value)
    {
        OilPaintEngine.UpdateRakelEmitMode((EmitMode)value);
    }
}
