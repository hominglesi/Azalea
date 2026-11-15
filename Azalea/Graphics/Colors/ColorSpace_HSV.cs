using System;
using System.Numerics;

namespace Azalea.Graphics.Colors;

public static partial class ColorSpace
{
	public static class HSV
	{
		public static Vector4 FromColor(Color color)
		{
			float hue, value, saturation;

			var min = Math.Min(color.RNormalized, Math.Min(color.BNormalized, color.GNormalized));
			var max = Math.Max(color.RNormalized, Math.Max(color.BNormalized, color.GNormalized));

			value = max;
			var delta = max - min;
			if (delta < 0.00001)
			{
				saturation = 0;
				hue = 0;
				return new Vector4(hue, value, saturation, color.ANormalized);
			}

			if (max > 0.0)
				saturation = (delta / max);
			else
			{
				saturation = 0;
				hue = 0;
				return new Vector4(hue, value, saturation, color.ANormalized);
			}


			if (color.RNormalized >= max)
				hue = (color.GNormalized - color.BNormalized) / delta;
			else if (color.GNormalized >= max)
				hue = 2.0f + (color.BNormalized - color.RNormalized) / delta;
			else
				hue = 4.0f + (color.RNormalized - color.GNormalized) / delta;

			hue *= 60;

			if (hue < 0.0)
				hue += 360;

			return new Vector4(hue, value, saturation, color.ANormalized);
		}

		public static float GetHue(Color color) => FromColor(color).X;
		public static float GetSaturation(Color color) => FromColor(color).Y;
		public static float GetValue(Color color) => FromColor(color).Z;

		public static Color ToColor(Vector4 hsv)
		{
			float red, green, blue;

			if (hsv.Y <= 0)
			{
				red = hsv.Z;
				green = hsv.Z;
				blue = hsv.Z;
				return new Color(red, green, blue, hsv.W);
			}

			var hue = hsv.X;
			if (hue >= 360) hue = 0;
			hue /= 60;

			var i = (int)Math.Floor(hue);

			var ff = hue - i;
			var p = hsv.Z * (1 - hsv.Y);
			var q = hsv.Z * (1 - (hsv.Y * ff));
			var t = hsv.Z * (1 - (hsv.Y * (1 - ff)));

			switch (i)
			{
				case 0:
					red = hsv.Z;
					green = t;
					blue = p;
					break;
				case 1:
					red = q;
					green = hsv.Z;
					blue = p;
					break;
				case 2:
					red = p;
					green = hsv.Z;
					blue = t;
					break;

				case 3:
					red = p;
					green = q;
					blue = hsv.Z;
					break;
				case 4:
					red = t;
					green = p;
					blue = hsv.Z;
					break;
				default:
					red = hsv.Z;
					green = p;
					blue = q;
					break;
			}

			return new Color(red, green, blue, hsv.W);
		}

		public static Color SetHue(Color color, float hue)
		{
			var hsv = FromColor(color);
			hsv.X = Math.Clamp(hue % 360, 0, 360);
			return ToColor(hsv);
		}

		public static Color AddHue(Color color, float hue)
		{
			var hsv = FromColor(color);
			hsv.X += hue;
			hsv.X = Math.Clamp(hsv.X, 0, 360);
			return ToColor(hsv);
		}

		public static Color SetSaturation(Color color, float saturation)
		{
			var hsv = FromColor(color);
			hsv.Y = Math.Clamp(saturation, 0, 1);
			return ToColor(hsv);
		}

		public static Color AddSaturation(Color color, float saturation)
		{
			var hsv = FromColor(color);
			hsv.Y += saturation;
			hsv.Y = Math.Clamp(hsv.Y, 0, 1);
			return ToColor(hsv);
		}

		public static Color SetValue(Color color, float value)
		{
			var hsv = FromColor(color);
			hsv.Z = Math.Clamp(value, 0, 1);
			return ToColor(hsv);
		}

		public static Color AddValue(Color color, float value)
		{
			var hsv = FromColor(color);
			hsv.Z += value;
			hsv.Z = Math.Clamp(hsv.Z, 0, 1);
			return ToColor(hsv);
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

		public static ColorQuad AddValue(ColorQuad colorQuad, float value)
		{
			if (colorQuad.HasSingleColor)
				return ColorQuad.SolidColor(AddValue(colorQuad.SingleColor, value));
			else
				return new ColorQuad(
								AddValue(colorQuad.TopLeft, value),
								AddValue(colorQuad.BottomLeft, value),
								AddValue(colorQuad.BottomRight, value),
								AddValue(colorQuad.TopRight, value));
		}
	}
}
