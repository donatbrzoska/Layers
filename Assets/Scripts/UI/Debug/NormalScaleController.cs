using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class NormalScaleController : MonoBehaviour
{
    protected OilPaintEngine OilPaintEngine;
    protected Slider Slider;

    void Awake()
    {
        OilPaintEngine = GameObject.Find("OilPaintEngine").GetComponent<OilPaintEngine>();
        Slider = GetComponent<Slider>();
        Slider.onValueChanged.AddListener(OnValueChanged);
    }

    public void Start()
    {
        Slider.SetValueWithoutNotify(OilPaintEngine.Config.CanvasConfig.NormalScale);
    }

    public void OnValueChanged(float arg0)
    {
        OilPaintEngine.UpdateNormalScale(arg0);
    }
}
