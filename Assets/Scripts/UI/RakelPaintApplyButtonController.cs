using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(Button))]
public class RakelPaintApplyButtonController : MonoBehaviour
{
    OilPaintEngine OilPaintEngine;
    Button Button;

    TMP_Dropdown ColorDropdown;
    TMP_InputField VolumeInputField;
    TMP_Dropdown FillModeDropdown;

    public void Awake()
    {
        OilPaintEngine = GameObject.Find("OilPaintEngine").GetComponent<OilPaintEngine>();

        Button = GetComponent<Button>();
        Button.onClick.AddListener(OnClick);

        ColorDropdown = GameObject.Find("Predefined Colors Dropdown").GetComponent<TMP_Dropdown>();
        List<string> colorNames = new List<string>();
        foreach (_Color c in Enum.GetValues(typeof(_Color)))
        {
            colorNames.Add(Colors.GetName(c));
        }
        ColorDropdown.AddOptions(colorNames);

        VolumeInputField = GameObject.Find("Paint Volume Value").GetComponent<TMP_InputField>();

        FillModeDropdown = GameObject.Find("Fill Mode Dropdown").GetComponent<TMP_Dropdown>();
        FillModeDropdown.AddOptions(Enum.GetNames(typeof(FillMode)).ToList());
    }

    public void Start()
    {
        ColorDropdown.SetValueWithoutNotify((int)Colors.Get_Color(OilPaintEngine.Configuration.FillPaint.Color));
        VolumeInputField.SetTextWithoutNotify("" + OilPaintEngine.Configuration.FillPaint.Volume);
        FillModeDropdown.SetValueWithoutNotify((int)OilPaintEngine.Configuration.FillMode);
    }

    public void OnClick()
    {
        _Color color = (_Color)ColorDropdown.value;
        int volume = int.Parse(VolumeInputField.text);
        FillMode fillMode = (FillMode)FillModeDropdown.value;

        OilPaintEngine.UpdateRakelPaint(color, volume, fillMode);
    }
}
