using System.Collections.Generic;
using UnityEngine;

public enum Color_
{
    TitanWhite,
    IvoryBlack,
    DarkRed,
    UltramarineBlue,
    LemonYellow,
    CadmiumYellow,
    CadmiumOrange,
    CadmiumRed,
    CadmiumGreen,
    CadmiumGreenLight,
    _________,
    Green,
    Blue,
    Purple,
    LavenderLight,
    Lavender,
    TealLight,
    TealDark,
    Rose,

}

public enum ColorSpace
{
    RGB,
    RYB
}

public struct ColorInfo
{
    public string Name;
    public Vector3 Color;
}

public class Colors
{
    public static Vector3 CANVAS_COLOR { get; private set; } = new Vector3(1, 1, 1);
    public static Vector3 NO_PAINT_COLOR { get; private set; } = new Vector3(0, 0, 0);

    public static string GetName(Color_ color)
    {
        return ColorMapper[color].Name;
    }

    public static Vector3 GetColor(Color_ color)
    {
        return ColorMapper[color].Color;
    }

    public static Color_ GetColor_(Vector3 color)
    {
        foreach (KeyValuePair<Color_, ColorInfo> entry in ColorMapper)
        {
            if (entry.Value.Color.Equals(color))
            {
                return entry.Key;
            }
        }
        // should never happen
        return 0;
    }

    private static Dictionary<Color_, ColorInfo> ColorMapper = new Dictionary<Color_, ColorInfo>()
    {
        // https://www.color-name.com/titanium-white.color
        { Color_.TitanWhite, new ColorInfo() { Name = "Titan White", Color = new Vector3(243/255.0f, 244/255.0f, 247/255.0f) } },

        // https://www.color-name.com/ivory-black.color
        { Color_.IvoryBlack, new ColorInfo() { Name = "Ivory Black", Color = new Vector3(35/255f, 31/255f, 32/255f) } },

        { Color_.DarkRed, new ColorInfo() { Name = "Red", Color = new Vector3(0.58f, 0.06f, 0f)} },

        // https://www.color-name.com/ultramarine-blue-ral.color
        { Color_.UltramarineBlue, new ColorInfo() { Name = "Ultramarine Blue", Color = new Vector3(30/255f, 54/255f, 123/255f) } }, // RAL

        // https://www.color-name.com/lemon-yellow.color
        { Color_.LemonYellow, new ColorInfo() { Name = "Lemon Yellow", Color = new Vector3(254/255f, 242/255f, 80/255f) } },

        // https://www.colorhexa.com/fff600
        { Color_.CadmiumYellow, new ColorInfo() { Name = "Cadmium Yellow", Color = new Vector3(255/255f, 246/255f, 0/255f) } },

        // https://www.colorhexa.com/ed872d
        { Color_.CadmiumOrange, new ColorInfo() { Name = "Cadmium Orange", Color = new Vector3(237/255f, 135/255f, 45/255f) } },

        // https://www.colorhexa.com/e30022
        { Color_.CadmiumRed, new ColorInfo() { Name = "Cadmium Red", Color = new Vector3(227/255f, 0/255f, 34/255f) } },

        // https://www.colorhexa.com/006b3c
        { Color_.CadmiumGreen, new ColorInfo() { Name = "Cadmium Green", Color = new Vector3(0/255f, 107/255f, 60/255f) } },

        // just taken from https://www.kremer-pigmente.com/en/shop/pigments/44500-cadmium-green-light.html
        { Color_.CadmiumGreenLight, new ColorInfo() { Name = "Cadmium Green Light", Color = new Vector3(128/255f, 181/255f, 46/255f) } },

        // just taken from https://www.kremer-pigmente.com/en/shop/pigments/44500-cadmium-green-light.html
        { Color_._________, new ColorInfo() { Name = "_________", Color = new Vector3(128/255f, 181/255f, 46/255f) } },

        { Color_.Green, new ColorInfo() { Name = "Green", Color = new Vector3(0.02f, 0.57f, 0.04f) } },
        { Color_.Blue, new ColorInfo() { Name = "Blue", Color = new Vector3(0.12f, 0.49f, 0.93f) } },
        { Color_.Purple, new ColorInfo() { Name = "Purple", Color = new Vector3(0.5f, 0.3f, 0.99f) } },
        { Color_.LavenderLight, new ColorInfo() { Name = "Lavender Light", Color = new Vector3(196/255f, 196/255f, 252/255f) } },
        { Color_.Lavender, new ColorInfo() { Name = "Lavender", Color = new Vector3(150/255f, 150/255f, 254/255f) } },
        { Color_.TealLight, new ColorInfo() { Name = "TealLight", Color = new Vector3(102/255f, 178/255f, 178/255f) } },
        { Color_.TealDark, new ColorInfo() { Name = "TealDark", Color = new Vector3(0/255f, 81/255f, 81/255f) } },
        { Color_.Rose, new ColorInfo() { Name = "Rose", Color = new Vector3(246/255f, 152/255f, 215/255f) } },
    };

    public static Vector3 RGB2RYB(Vector3 rgb)
    {
        return RGB2RYB_ST(rgb);
    }

    static Vector3 RGB2RYB_ST(Vector3 RGB)
    {
        float I_w = Mathf.Min(Mathf.Min(RGB.x, RGB.y), RGB.z);
        Vector3 rgb = RGB - new Vector3(I_w, I_w, I_w);

        Vector3 ryb = new Vector3(
            rgb.x - Mathf.Min(rgb.x, rgb.y),
            (rgb.y + Mathf.Min(rgb.x, rgb.y)) / 2,
            (rgb.z + rgb.y - Mathf.Min(rgb.x, rgb.y)) / 2
        );

        float n = (Mathf.Max(Mathf.Max(ryb.x, ryb.y), ryb.z)) / (Mathf.Max(Mathf.Max(rgb.x, rgb.y), rgb.z) + 0.00001f);
        Vector3 ryb_ = ryb / (n + 0.00001f);

        float I_b = Mathf.Min(Mathf.Min(1 - RGB.x, 1 - RGB.y), 1 - RGB.z);
        Vector3 RYB = ryb_ + new Vector3(I_b, I_b, I_b);
        return RYB;
    }
}
