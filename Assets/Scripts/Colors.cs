using System.Collections.Generic;
using UnityEngine;

public enum _Color
{
    TitanWhite,
    IvoryBlack,
    Red,
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
    DarkRed,
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

public struct ColorInfo
{
    public string Name;
    public Color Color;
}

public class Colors
{
    public static Color CANVAS_COLOR { get; private set; } = new Color(1, 1, 1, 1);
    public static Color NO_PAINT_COLOR { get; private set; } = new Color(0, 0, 0, 0);

    public static string GetName(_Color color)
    {
        return ColorMapper[color].Name;
    }

    public static Color GetColor(_Color color)
    {
        return ColorMapper[color].Color;
    }

    public static _Color Get_Color(Color color)
    {
        foreach (KeyValuePair<_Color, ColorInfo> entry in ColorMapper)
        {
            if (entry.Value.Color.Equals(color))
            {
                return entry.Key;
            }
        }
        // should never happen
        return 0;
    }

    public static Dictionary<_Color, ColorInfo> ColorMapper = new Dictionary<_Color, ColorInfo>()
    {
        // https://www.color-name.com/titanium-white.color
        { _Color.TitanWhite, new ColorInfo() { Name = "Titan White", Color = new Color(243/255.0f, 244/255.0f, 247/255.0f) } },

        // https://www.color-name.com/ivory-black.color
        { _Color.IvoryBlack, new ColorInfo() { Name = "Ivory Black", Color = new Color(35/255f, 31/255f, 32/255f) } },

        { _Color.Red, new ColorInfo() { Name = "Red", Color = new Color(0.58f, 0.06f, 0f)} },

        // https://www.color-name.com/ultramarine.color
        { _Color.UltramarineBlue, new ColorInfo() { Name = "Ultramarine Blue", Color = new Color(33/255f, 66/255f, 171/255f) } },

        // https://www.color-name.com/ultramarine-blue-ral.color
        { _Color.UltramarineBlueRAL, new ColorInfo() { Name = "Ultramarine Blue (RAL)", Color = new Color(30/255f, 54/255f, 123/255f) } }, // RAL

        // https://www.color-name.com/lemon-yellow.color
        { _Color.LemonYellow, new ColorInfo() { Name = "Lemon Yellow", Color = new Color(254/255f, 242/255f, 80/255f) } },

        // https://en.wikipedia.org/wiki/Cadmium_pigments
        { _Color.CadmiumYellow, new ColorInfo() { Name = "Cadmium Yellow", Color = new Color(255/255f, 246/255f, 0/255f) } },

        // https://en.wikipedia.org/wiki/Cadmium_pigments
        { _Color.CadmiumOrange, new ColorInfo() { Name = "Cadmium Orange", Color = new Color(237/255f, 135/255f, 45/255f) } },

        // https://en.wikipedia.org/wiki/Cadmium_pigments
        { _Color.CadmiumRed, new ColorInfo() { Name = "Cadmium Red", Color = new Color(227/255f, 0/255f, 34/255f) } },

        // https://en.wikipedia.org/wiki/Cadmium_pigments
        { _Color.CadmiumGreen, new ColorInfo() { Name = "Cadmium Green", Color = new Color(0/255f, 107/255f, 60/255f) } },

        // just taken from https://www.kremer-pigmente.com/en/shop/pigments/44500-cadmium-green-light.html
        { _Color.CadmiumGreenLight, new ColorInfo() { Name = "Cadmium Green Light", Color = new Color(128/255f, 181/255f, 46/255f) } },

        { _Color.Anthracite, new ColorInfo() { Name = "Anthracite", Color = new Color(0.25f, 0.25f, 0.25f) } },
        { _Color.Red_, new ColorInfo() { Name = "Red_", Color = new Color(0.8f, 0.08f, 0.03f) } },
        { _Color.DarkRed, new ColorInfo() { Name = "Dark Red", Color = new Color(0.58f, 0.06f, 0f) } },
        { _Color.Green, new ColorInfo() { Name = "Green", Color = new Color(0.02f, 0.57f, 0.04f) } },
        { _Color.Blue, new ColorInfo() { Name = "Blue", Color = new Color(0.12f, 0.49f, 0.93f) } },
        { _Color.DarkBlue, new ColorInfo() { Name = "Dark Blue", Color = new Color(0.05f, 0.12f, 0.32f)} },
        { _Color.Yellow, new ColorInfo() { Name = "Yellow", Color = new Color(1f, 0.88f, 0.12f) } },
        { _Color.Orange, new ColorInfo() { Name = "Orange", Color = new Color(1f, 0.64f, 0.06f) } },
        { _Color.Purple, new ColorInfo() { Name = "Purple", Color = new Color(0.5f, 0.3f, 0.99f) } },
        { _Color.White, new ColorInfo() { Name = "White", Color = new Color(0.99f, 0.99f, 0.99f) } },
        { _Color.Black, new ColorInfo() { Name = "Black", Color = new Color(0.01f, 0.01f, 0.01f) } },
        { _Color.RedRed, new ColorInfo() { Name = "Red Red", Color = new Color(1f, 0f, 0f) } },
        { _Color.GreenGreen, new ColorInfo() { Name = "Green Green", Color = new Color(0f, 1f, 0f) } },
        { _Color.BlueBlue, new ColorInfo() { Name = "Blue Blue", Color = new Color(0f, 0f, 1f) } },
    };
}
