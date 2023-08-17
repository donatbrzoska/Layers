using System;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public abstract class DropdownController : MonoBehaviour
{
    protected OilPaintEngine OilPaintEngine;
    protected TMP_Dropdown Dropdown;

    public void Awake()
    {
        OilPaintEngine = GameObject.Find("OilPaintEngine").GetComponent<OilPaintEngine>();
        Dropdown = GetComponent<TMP_Dropdown>();
        Dropdown.onValueChanged.AddListener(OnValueChanged);
    }

    protected void InitializeElements(Type enumType)
    {
        Dropdown.AddOptions(Enum.GetNames(enumType).ToList());
    }

    protected void MakeNonInteractableInEvaluateMode()
    {
        if (OilPaintEngine.EVALUATE)
        {
            Dropdown.interactable = false;
        }
    }

    public abstract void OnValueChanged(int value);
}
