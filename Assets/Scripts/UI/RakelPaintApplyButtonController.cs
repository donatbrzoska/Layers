using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(Button))]
public class RakelPaintApplyButtonController : MonoBehaviour
{
    OilPaintEngine OilPaintEngine;
    Button Button;

    TMP_Dropdown ColorDropdown;
    TMP_InputField VolumeInputField;

    Dictionary<string, Color> ColorMapper = new Dictionary<string, Color>()
    {
        { "Titan White", new Color(243/255.0f, 244/255.0f, 247/255.0f)}, // https://www.color-name.com/titanium-white.color
        { "Ivory Black", new Color(35/255f, 31/255f, 32/255f)}, // https://www.color-name.com/ivory-black.color
        { "Red", new Color(0.58f, 0.06f, 0f) },
        { "Ultramarine Blue", new Color(33/255f, 66/255f, 171/255f)}, // https://www.color-name.com/ultramarine.color
        { "Ultramarine Blue (RAL)", new Color(30/255f, 54/255f, 123/255f)}, // RAL
        { "Lemon Yellow", new Color(254/255f, 242/255f, 80/255f)}, // https://www.color-name.com/lemon-yellow.color
        { "Cadmium Yellow", new Color(255/255f, 246/255f, 0/255f)}, // https://en.wikipedia.org/wiki/Cadmium_pigments
        { "Cadmium Orange", new Color(237/255f, 135/255f, 45/255f)}, // https://en.wikipedia.org/wiki/Cadmium_pigments
        { "Cadmium Red", new Color(227/255f, 0/255f, 34/255f)}, // https://en.wikipedia.org/wiki/Cadmium_pigments
        { "Cadmium Green", new Color(0/255f, 107/255f, 60/255f)}, // https://en.wikipedia.org/wiki/Cadmium_pigments
        { "Cadmium Green Light", new Color(128/255f, 181/255f, 46/255f)}, // just taken from https://www.kremer-pigmente.com/en/shop/pigments/44500-cadmium-green-light.html

        { "Anthracite", new Color(0.25f, 0.25f, 0.25f) },
        { "Red_", new Color(0.8f, 0.08f, 0.03f) },
        { "Dark Red", new Color(0.58f, 0.06f, 0f) },
        { "Green", new Color(0.02f, 0.57f, 0.04f) },
        { "Blue", new Color(0.12f, 0.49f, 0.93f) },
        { "Dark Blue", new Color(0.05f, 0.12f, 0.32f) },
        { "Yellow", new Color(1f, 0.88f, 0.12f) },
        { "Orange", new Color(1f, 0.64f, 0.06f) },
        { "Purple", new Color(0.5f, 0.3f, 0.99f) },
        { "White", new Color(0.99f, 0.99f, 0.99f) },
        { "Black", new Color(0.01f, 0.01f, 0.01f) },
        { "Red Red", new Color(1f, 0f, 0f) },
        { "Green Green", new Color(0f, 1f, 0f) },
        { "Blue Blue", new Color(0f, 0f, 1f) },
    };

    public void Awake()
    {
        OilPaintEngine = GameObject.Find("OilPaintEngine").GetComponent<OilPaintEngine>();
        Button = GetComponent<Button>();
        Button.onClick.AddListener(OnClick);

        ColorDropdown = GameObject.Find("Predefined Colors Dropdown").GetComponent<TMP_Dropdown>();
        VolumeInputField = GameObject.Find("Paint Volume Value").GetComponent<TMP_InputField>();
    }

    public void Start()
    {
        ColorDropdown.SetValueWithoutNotify(9); // TODO retrieve from OilPaintEngine? Requires backwards mapping though ...
        VolumeInputField.SetTextWithoutNotify("40");
    }

    public void OnClick()
    {
        Color color = ColorMapper[ColorDropdown.options[ColorDropdown.value].text];
        int volume = int.Parse(VolumeInputField.text);

        OilPaintEngine.UpdateRakelPaint(new Paint(color, volume));
    }
}
