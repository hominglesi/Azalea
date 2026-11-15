using System;
using System.Numerics;

namespace Azalea.Graphics.Colors;

public static partial class ColorSpace
{
	public static class Linear
	{
		public static Vector4 FromColor(Color color)
		{
			return new(
				toLinear(color.RNormalized),
				toLinear(color.GNormalized),
				toLinear(color.BNormalized),
				color.ANormalized);

			static float toLinear(float rgb)
			{
				if (rgb == 1.0f) return 1.0f;

				return rgb <= 0.04045f ? rgb / 12.92f : MathF.Pow((rgb + 0.055f) / 1.055f, 2.4f);
			}
		}

		public static Color ToColor(Vector4 hsl)
		{
			return new Color(fromLinear(hsl.X), fromLinear(hsl.Y), fromLinear(hsl.Z), hsl.W);

			static float fromLinear(float linear)
			{
				if (linear == 1.0f) return 1.0f;

				return linear < 0.0031308f ? linear * 12.92f : 1.055f * MathF.Pow(linear, 1 / 2.4f) - 0.055f;
			}
		}
	}
}
