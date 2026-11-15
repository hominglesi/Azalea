using System;
using System.Numerics;

namespace Azalea.Graphics.Colors;

public static partial class ColorSpace
{
	public static class HSL
	{
		public static Vector4 FromColor(Color color)
		{
			var red = color.RNormalized;
			var green = color.GNormalized;
			var blue = color.BNormalized;

			var max = Math.Max(red, Math.Max(green, blue));
			var min = Math.Min(red, Math.Min(green, blue));
			var diff = max - min;

			var h = 0.0f;
			if (max == red) h = ((green - blue) / diff) % 6;
			else if (max == green) h = ((blue - red) / diff) + 2f;
			else if (max == blue) h = ((red - green) / diff) + 4f;


			if (h < 0) h += 6;
			else if (h > 6) h -= 6;
			else if (float.IsNaN(h)) h = 0;

			var hue = h * 60f;

			var luminance = (max + min) / 2f;

			var saturation = 0.0f;
			if ((1.0f - Math.Abs((2.0f * luminance) - 1.0f)) != 0)
				saturation = diff / (1.0f - Math.Abs((2.0f * luminance) - 1.0f));

			return new Vector4(hue, saturation, luminance, color.ANormalized);
		}

		public static float GetHue(Color color) => FromColor(color).X;
		public static float GetSaturation(Color color) => FromColor(color).Y;
		public static float GetLightness(Color color) => FromColor(color).Z;

		public static Color ToColor(Vector4 hsl)
		{
			var hue = hsl.X;
			var saturation = hsl.Y;
			var lightness = hsl.Z;
			var alpha = hsl.W;

			var c = (1.0f - Math.Abs((2.0f * lightness) - 1.0f)) * saturation;

			var h = hue / 60f;
			var x = c * (1.0f - Math.Abs((h % 2.0f) - 1.0f));

			float r = 0.0f, g = 0.0f, b = 0.0f;

			if (h >= 0.0f && h < 1.0f)
			{
				r = c;
				g = x;
				b = 0.0f;
			}
			else if (h >= 1.0f && h < 2.0f)
			{
				r = x;
				g = c;
				b = 0.0f;
			}
			else if (h >= 2.0f && h < 3.0f)
			{
				r = 0.0f;
				g = c;
				b = x;
			}
			else if (h >= 3.0f && h < 4.0f)
			{
				r = 0.0f;
				g = x;
				b = c;
			}
			else if (h >= 4.0f && h < 5.0f)
			{
				r = x;
				g = 0.0f;
				b = c;
			}
			else if (h >= 5.0f && h < 6.0f)
			{
				r = c;
				g = 0.0f;
				b = x;
			}

			var m = lightness - (c / 2.0f);
			if (m < 0)
			{
				m = 0;
			}

			return new Color(r + m, g + m, b + m, alpha);
		}

		public static Color SetHue(Color color, float hue)
		{
			var hsl = FromColor(color);
			hsl.X = Math.Clamp(hue % 360, 0, 360);
			return ToColor(hsl);
		}

		public static Color AddHue(Color color, float hue)
		{
			var hsl = FromColor(color);
			hsl.X += hue;
			hsl.X = Math.Clamp(hsl.X, 0, 360);
			return ToColor(hsl);
		}

		public static Color SetSaturation(Color color, float saturation)
		{
			var hsl = FromColor(color);
			hsl.Y = Math.Clamp(saturation, 0, 1);
			return ToColor(hsl);
		}

		public static Color AddSaturation(Color color, float saturation)
		{
			var hsl = FromColor(color);
			hsl.Y += saturation;
			hsl.Y = Math.Clamp(hsl.Y, 0, 1);
			return ToColor(hsl);
		}

		public static Color SetLightness(Color color, float lightness)
		{
			var hsl = FromColor(color);
			hsl.Z = Math.Clamp(lightness, 0, 1);
			return ToColor(hsl);
		}

		public static Color AddLightness(Color color, float lightness)
		{
			var hsl = FromColor(color);
			hsl.Z += lightness;
			hsl.Z = Math.Clamp(hsl.Z, 0, 1);
			return ToColor(hsl);
		}

		public static ColorQuad AddHue(ColorQuad colorQuad, float value)
		{
			if (colorQuad.HasSingleColor)
				return ColorQuad.SolidColor(AddHue(colorQuad.SingleColor, value));
			else
				return new ColorQuad(
						AddHue(colorQuad.TopLeft, value),
						AddHue(colorQuad.BottomLeft, value),
						AddHue(colorQuad.BottomRight, value),
						AddHue(colorQuad.TopRight, value));
		}

		public static ColorQuad AddSaturation(ColorQuad colorQuad, float value)
		{
			if (colorQuad.HasSingleColor)
				return ColorQuad.SolidColor(AddSaturation(colorQuad.SingleColor, value));
			else
				return new ColorQuad(
							AddSaturation(colorQuad.TopLeft, value),
							AddSaturation(colorQuad.BottomLeft, value),
							AddSaturation(colorQuad.BottomRight, value),
							AddSaturation(colorQuad.TopRight, value));
		}

		public static ColorQuad AddLightness(ColorQuad colorQuad, float value)
		{
			if (colorQuad.HasSingleColor)
				return ColorQuad.SolidColor(AddLightness(colorQuad.SingleColor, value));
			else
				return new ColorQuad(
								AddLightness(colorQuad.TopLeft, value),
								AddLightness(colorQuad.BottomLeft, value),
								AddLightness(colorQuad.BottomRight, value),
								AddLightness(colorQuad.TopRight, value));
		}
	}
}
