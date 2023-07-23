using System.Numerics;

namespace Azalea.Graphics;

public static class ColorExtentions
{
    internal static Microsoft.Xna.Framework.Color ToXNAColor(this Color color)
        => new(color.R, color.G, color.B, color.A);

    internal static Vector4 ToVector4(this Color color)
        => new(color.RNormalized, color.GNormalized, color.BNormalized, color.ANormalized);
}
