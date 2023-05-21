namespace Azalea.Graphics;

public static class ColorExtentions
{
    internal static Microsoft.Xna.Framework.Color ToXNAColor(this Color color)
    {
        return new Microsoft.Xna.Framework.Color(color.R, color.G, color.B, color.A);
    }
}
