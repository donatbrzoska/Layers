using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Macro2ButtonController : MonoBehaviour
{
    OilPaintEngine OilPaintEngine;
    Button Button;

    public void Awake()
    {
        OilPaintEngine = GameObject.Find("OilPaintEngine").GetComponent<OilPaintEngine>();
        Button = GetComponent<Button>();
        Button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        OilPaintEngine.DoMacro2Action();
    }
}