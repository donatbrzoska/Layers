using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public abstract class InputFieldController : MonoBehaviour
{
    protected OilPaintEngine OilPaintEngine;
    protected TMP_InputField InputField;

    void Awake()
    {
        OilPaintEngine = GameObject.Find("OilPaintEngine").GetComponent<OilPaintEngine>();
        InputField = GetComponent<TMP_InputField>();
        InputField.onValueChanged.AddListener(OnValueChanged);
    }

    protected void MakeNonInteractableInEvaluateMode()
    {
        if (OilPaintEngine.EVALUATE)
        {
            InputField.interactable = false;
        }
    }

    abstract public void OnValueChanged(string arg0);
}
