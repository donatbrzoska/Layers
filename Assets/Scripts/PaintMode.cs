using System.Collections.Generic;

public enum PaintMode
{
    Blurry,
    IntenseSmearing,
    LightSmearing
}

public static class PaintModes
{
    public static string GetName(PaintMode paintMode)
    {
        return PaintModeMapper[paintMode];
    }

    private static Dictionary<PaintMode, string> PaintModeMapper = new Dictionary<PaintMode, string>()
    {
        { PaintMode.Blurry, "Blurry" },
        { PaintMode.IntenseSmearing, "Intense Smearing" },
        { PaintMode.LightSmearing, "Light Smearing" },
    };
}