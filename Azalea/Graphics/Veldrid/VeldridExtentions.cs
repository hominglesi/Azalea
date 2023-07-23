using Veldrid;

namespace Azalea.Graphics.Veldrid;

internal static class VeldridExtentions
{
    public static RgbaFloat ToRgbaFloat(this Color color) => new(color.RNormalized, color.GNormalized, color.BNormalized, color.ANormalized);
}
