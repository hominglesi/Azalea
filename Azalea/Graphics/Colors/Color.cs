using System;
using System.Linq;

namespace Azalea.Graphics.Colors;

/// <summary>
/// Represents a color with 4 byte components (Red, Green, Blue, Alpha)
/// </summary>
public partial struct Color
{
	/// <summary>
	/// The red component of this Color
	/// </summary>
	public byte R;

	/// <summary>
	/// The green component of this Color
	/// </summary>
	public byte G;

	/// <summary>
	/// The blue color of this Color
	/// </summary>
	public byte B;

	/// <summary>
	/// The alpha component of this Color
	/// </summary>
	public byte A;

	/// <summary>
	/// Constructs a new Color from the specified components
	/// </summary>
	/// <param name="r">The red component of the new Color</param>
	/// <param name="g">The green component of the new Color</param>
	/// <param name="b">The blue component of the new Color</param>
	/// <param name="a">The alphe component of the new Color</param>
	public Color(byte r, byte g, byte b, byte a)
	{
		R = r;
		G = g;
		B = b;
		A = a;
	}

	/// <summary>
	/// Constructs a new Color from the specified components
	/// </summary>
	/// <param name="r">The red component of the new Color</param>
	/// <param name="g">The green component of the new Color</param>
	/// <param name="b">The blue component of the new Color</param>
	public Color(byte r, byte g, byte b) : this(r, g, b, byte.MaxValue) { }

	#region Normalized

	/// <summary>
	/// Value of the Red channel represented by a float between 0 and 1
	/// </summary>
	public readonly float RNormalized => (float)R / byte.MaxValue;

	/// <summary>
	/// Value of the Green channel represented by a float between 0 and 1
	/// </summary>
	public readonly float GNormalized => (float)G / byte.MaxValue;

	/// <summary>
	/// Value of the Blue channel represented by a float between 0 and 1
	/// </summary>
	public readonly float BNormalized => (float)B / byte.MaxValue;

	/// <summary>
	/// Value of the Alpha channel represented by a float between 0 and 1
	/// </summary>
	public readonly float ANormalized => (float)A / byte.MaxValue;

	#endregion Normalized

	#region Hex

	/// <summary>
	/// Returns the color represented as a string of hexadecimal values
	/// </summary>
	/// <param name="includeHashtag">Defines if the color should be preceded with a '#' or not</param>
	public readonly string ToHex(bool includeHashtag = true)
		=> string.Concat(includeHashtag ? "#" : "", R.ToString("X2"), G.ToString("X2"), B.ToString("X2"));

	/// <summary>
	/// Returns a color constructed with the provided hexcode
	/// </summary>
	/// <param name="hexCode">The hex code can include the '#' but it doesn't have to. The rest of the string has to have
	/// a length of either 6 or 3</param>
	public static Color FromHex(string hexCode)
	{
		if (hexCode.StartsWith('#')) hexCode = hexCode[1..];

		switch (hexCode.Length)
		{
			case 6:
				var r = Convert.ToByte(hexCode.Substring(0, 2), 16);
				var g = Convert.ToByte(hexCode.Substring(2, 2), 16);
				var b = Convert.ToByte(hexCode.Substring(4, 2), 16);
				return new Color(r, g, b, 255);
			case 3:
				//When hex is writen with 3 chars (like #c20) that is the short hand where both chars are the same (#cc2200)
				//We can convert them with the formula cTotal = c + (c * 16)
				var rSingle = Convert.ToByte(hexCode.Substring(0, 1), 16);
				var gSingle = Convert.ToByte(hexCode.Substring(1, 1), 16);
				var bSingle = Convert.ToByte(hexCode.Substring(2, 1), 16);
				return new Color(
					(byte)(rSingle + (rSingle * 16)),
					(byte)(gSingle + (gSingle * 16)),
					(byte)(bSingle + (bSingle * 16)),
					255);
			default:
				throw new InvalidOperationException("Invalid hex code");
		}
	}

	#endregion

	#region HSL

	//Sources for this implementation
	//https://www.niwa.nu/2013/05/math-behind-colorspace-conversions-rgb-hsl/
	//https://stackoverflow.com/a/9493060

	private float maxValue() => new[] { RNormalized, GNormalized, BNormalized }.Max();
	private float minValue() => new[] { RNormalized, GNormalized, BNormalized }.Min();

	/// <summary>
	/// The luminance of the color
	/// </summary>
	public float Luminance
	{
		get => (maxValue() + minValue()) / 2;
		set
		{
			var clampedValue = Math.Clamp(value, 0, 1);

			var newColor = FromHSL(Hue, Saturation, clampedValue);
			R = newColor.R;
			G = newColor.G;
			B = newColor.B;
		}
	}

	/// <summary>
	/// The saturation of the color
	/// </summary>
	public float Saturation
	{
		get
		{
			if (minValue() == maxValue()) return 0;

			return Luminance <= 0.5f ? (maxValue() - minValue()) / (maxValue() + minValue()) :
			(maxValue() - minValue()) / (2f - maxValue() - minValue());

		}
		set
		{
			var clampedValue = Math.Clamp(value, 0, 1);

			var newColor = FromHSL(Hue, clampedValue, Luminance);
			R = newColor.R;
			G = newColor.G;
			B = newColor.B;
		}
	}

	public float Hue
	{
		get
		{
			if (minValue() == maxValue()) return 0;

			float hueValue = 0;
			if (maxValue() == RNormalized) hueValue = (GNormalized - BNormalized) / (maxValue() - minValue());
			else if (maxValue() == GNormalized) hueValue = 2f + (BNormalized - GNormalized) / (maxValue() - minValue());
			else hueValue = 4 + (RNormalized - GNormalized) / (maxValue() - minValue());

			hueValue *= 60;

			if (hueValue < 0) hueValue += 360;
			if (hueValue > 360) hueValue -= 360;

			return hueValue;
		}
		set
		{
			var clampedValue = Math.Clamp(value, 0, 360);

			var newColor = FromHSL(clampedValue, Saturation, Luminance);
			R = newColor.R;
			G = newColor.G;
			B = newColor.B;
		}
	}

	public Color FromHSL(float hue, float saturation, float luminance)
	{
		float r, g, b;

		hue /= 360;

		if (saturation == 0)
			r = g = b = luminance; //achromatic
		else
		{
			var q = luminance < 0.5f ? luminance * (1 + saturation) : (luminance + saturation) - (luminance * saturation);
			var p = (2 * luminance) - q;
			r = hueToRgb(p, q, hue + (1 / 3f));
			g = hueToRgb(p, q, hue);
			b = hueToRgb(p, q, hue - (1 / 3f));
		}

		return new Color(
			(byte)Math.Round(r * byte.MaxValue),
			(byte)Math.Round(g * byte.MaxValue),
			(byte)Math.Round(b * byte.MaxValue));
	}

	private float hueToRgb(float p, float q, float t)
	{
		if (t < 0) t += 1;
		if (t > 1) t -= 1;
		if (t < 1f / 6f) return p + (q - p) * 6 * t;
		if (t < 1f / 2f) return q;
		if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6;
		return p;
	}

	#endregion

	public override bool Equals(object? obj)
	{
		if (obj is not Color color) return false;

		return R == color.R && G == color.G && B == color.B && A == color.A;
	}
	public static bool operator ==(Color left, Color right) => left.Equals(right);
	public static bool operator !=(Color left, Color right) => !left.Equals(right);

	public override readonly int GetHashCode() => (R, G, B, A).GetHashCode();
	public override readonly string ToString() => $"({R}, {G}, {B})";
}
