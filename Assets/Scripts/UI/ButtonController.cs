using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class ButtonController : MonoBehaviour
{
    protected OilPaintEngine OilPaintEngine;
    protected Button Button;

    public void Awake()
    {
        OilPaintEngine = GameObject.Find("OilPaintEngine").GetComponent<OilPaintEngine>();
        Button = GetComponent<Button>();
        Button.onClick.AddListener(OnClick);
    }

    protected void MakeNonInteractableInEvaluateMode()
    {
        if (OilPaintEngine.EVALUATE)
        {
            Button.interactable = false;
        }
    }

    public abstract void OnClick();
}