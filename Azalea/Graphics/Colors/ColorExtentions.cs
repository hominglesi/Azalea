using System.Numerics;

namespace Azalea.Graphics.Colors;

public static class ColorExtentions
{

	internal static Vector4 ToVector4(this Color color)
		=> new(color.RNormalized, color.GNormalized, color.BNormalized, color.ANormalized);
}
