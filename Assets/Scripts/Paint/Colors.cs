using System.Collections.Generic;
using UnityEngine;

public enum Color_
{
    TitanWhite,
    IvoryBlack,
    DarkRed,
    UltramarineBlue,
    UltramarineBlueRAL,
    LemonYellow,
    CadmiumYellow,
    CadmiumOrange,
    CadmiumRed,
    CadmiumGreen,
    CadmiumGreenLight,
    Anthracite,
    Red_,
    Green,
    Blue,
    DarkBlue,
    Yellow,
    Orange,
    Purple,
    White,
    Black,
    RedRed,
    GreenGreen,
    BlueBlue
}

public enum ColorSpace
{
    RGB,
    RYB
}

public struct ColorInfo
{
    public string Name;
    public Color Color;
}

public class Colors
{
    public static Color CANVAS_COLOR { get; private set; } = new Color(1, 1, 1, 1);
    public static Color NO_PAINT_COLOR { get; private set; } = new Color(0, 0, 0, 0);

    public static string GetName(Color_ color)
    {
        return ColorMapper[color].Name;
    }

    public static Color GetColor(Color_ color)
    {
        return ColorMapper[color].Color;
    }

    public static Color_ GetColor_(Color color)
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
        { Color_.TitanWhite, new ColorInfo() { Name = "Titan White", Color = new Color(243/255.0f, 244/255.0f, 247/255.0f) } },

        // https://www.color-name.com/ivory-black.color
        { Color_.IvoryBlack, new ColorInfo() { Name = "Ivory Black", Color = new Color(35/255f, 31/255f, 32/255f) } },

        { Color_.DarkRed, new ColorInfo() { Name = "Red", Color = new Color(0.58f, 0.06f, 0f)} },

        // https://www.color-name.com/ultramarine.color
        { Color_.UltramarineBlue, new ColorInfo() { Name = "Ultramarine Blue", Color = new Color(33/255f, 66/255f, 171/255f) } },

        // https://www.color-name.com/ultramarine-blue-ral.color
        { Color_.UltramarineBlueRAL, new ColorInfo() { Name = "Ultramarine Blue (RAL)", Color = new Color(30/255f, 54/255f, 123/255f) } }, // RAL

        // https://www.color-name.com/lemon-yellow.color
        { Color_.LemonYellow, new ColorInfo() { Name = "Lemon Yellow", Color = new Color(254/255f, 242/255f, 80/255f) } },

        // https://www.colorhexa.com/fff600
        { Color_.CadmiumYellow, new ColorInfo() { Name = "Cadmium Yellow", Color = new Color(255/255f, 246/255f, 0/255f) } },

        // https://www.colorhexa.com/ed872d
        { Color_.CadmiumOrange, new ColorInfo() { Name = "Cadmium Orange", Color = new Color(237/255f, 135/255f, 45/255f) } },

        // https://www.colorhexa.com/e30022
        { Color_.CadmiumRed, new ColorInfo() { Name = "Cadmium Red", Color = new Color(227/255f, 0/255f, 34/255f) } },

        // https://www.colorhexa.com/006b3c
        { Color_.CadmiumGreen, new ColorInfo() { Name = "Cadmium Green", Color = new Color(0/255f, 107/255f, 60/255f) } },

        // just taken from https://www.kremer-pigmente.com/en/shop/pigments/44500-cadmium-green-light.html
        { Color_.CadmiumGreenLight, new ColorInfo() { Name = "Cadmium Green Light", Color = new Color(128/255f, 181/255f, 46/255f) } },

        { Color_.Anthracite, new ColorInfo() { Name = "Anthracite", Color = new Color(0.25f, 0.25f, 0.25f) } },
        { Color_.Red_, new ColorInfo() { Name = "Red_", Color = new Color(0.8f, 0.08f, 0.03f) } },
        { Color_.Green, new ColorInfo() { Name = "Green", Color = new Color(0.02f, 0.57f, 0.04f) } },
        { Color_.Blue, new ColorInfo() { Name = "Blue", Color = new Color(0.12f, 0.49f, 0.93f) } },
        { Color_.DarkBlue, new ColorInfo() { Name = "Dark Blue", Color = new Color(0.05f, 0.12f, 0.32f)} },
        { Color_.Yellow, new ColorInfo() { Name = "Yellow", Color = new Color(1f, 0.88f, 0.12f) } },
        { Color_.Orange, new ColorInfo() { Name = "Orange", Color = new Color(1f, 0.64f, 0.06f) } },
        { Color_.Purple, new ColorInfo() { Name = "Purple", Color = new Color(0.5f, 0.3f, 0.99f) } },
        { Color_.White, new ColorInfo() { Name = "White", Color = new Color(0.99f, 0.99f, 0.99f) } },
        { Color_.Black, new ColorInfo() { Name = "Black", Color = new Color(0.01f, 0.01f, 0.01f) } },
        { Color_.RedRed, new ColorInfo() { Name = "Red Red", Color = new Color(1f, 0f, 0f) } },
        { Color_.GreenGreen, new ColorInfo() { Name = "Green Green", Color = new Color(0f, 1f, 0f) } },
        { Color_.BlueBlue, new ColorInfo() { Name = "Blue Blue", Color = new Color(0f, 0f, 1f) } },
    };

    public static Color RGB2RYB(Color rgb)
    {
        Vector3 rgb_ = new Vector3(rgb.r, rgb.g, rgb.b);
        Vector3 result = RGB2RYB_ST(rgb_);
        return new Color(result.x, result.y, result.z, 1);
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
