using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public abstract class ToggleController : MonoBehaviour
{
    protected OilPaintEngine OilPaintEngine;
    protected Toggle Toggle;

    public void Awake()
    {
        OilPaintEngine = GameObject.Find("OilPaintEngine").GetComponent<OilPaintEngine>();
        Toggle = GetComponent<Toggle>();
        Toggle.onValueChanged.AddListener(OnValueChanged);
    }

    public abstract void OnValueChanged(bool locked);
}
