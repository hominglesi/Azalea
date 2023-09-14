using System;
using System.Linq;

namespace Azalea.Graphics;

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

	private float maxValue() => new[] { RNormalized, BNormalized, GNormalized }.Max();
	private float minValue() => new[] { RNormalized, BNormalized, GNormalized }.Min();

	/// <summary>
	/// The luminance of the color
	/// </summary>
	public float Luminance
	{
		get => (maxValue() + minValue()) / 2;
	}

	/// <summary>
	/// The saturation of the color
	/// </summary>
	public float Saturation
	{
		get => Luminance <= 0.5f ?
			(maxValue() - minValue()) / (maxValue() + minValue()) :
			(maxValue() - minValue()) / (2f - maxValue() - minValue());
	}

	public float Hue
	{
		get
		{
			float hueValue = 0;
			if (maxValue() == RNormalized) hueValue = (GNormalized - BNormalized) / (maxValue() - minValue());
			else if (maxValue() == GNormalized) hueValue = 2f + (BNormalized - GNormalized) / (maxValue() - minValue());
			else hueValue = 4 + (RNormalized - GNormalized) / (maxValue() - minValue());

			hueValue *= 60;

			if (hueValue < 0) return hueValue + 360;
			if (hueValue > 360) return hueValue - 360;
			return hueValue;
		}
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
}
