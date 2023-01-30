using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ClearCanvasButtonController : MonoBehaviour
{
    OilPaintEngine OilPaintEngine;
    Button Button;

    public void Awake()
    {
        OilPaintEngine = GameObject.Find("OilPaintEngine").GetComponent<OilPaintEngine>();
        Button = GetComponent<Button>();
        Button.onClick.AddListener(OnClick);
    }

    public void Start()
    {

    }

    public void OnClick()
    {
        OilPaintEngine.ClearCanvas();
    }
}
