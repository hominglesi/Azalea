using System;
using System.Numerics;

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

	public Color(float r, float g, float b, float a)
		: this((byte)(r * byte.MaxValue),
			(byte)(g * byte.MaxValue),
			(byte)(b * byte.MaxValue),
			(byte)(a * byte.MaxValue))
	{

	}

	public void MultiplyAlpha(float alpha)
	{
		if (alpha >= 1f) return;

		A = (byte)(A * alpha);
	}

	#region Linear

	private float toLinear(float srgb)
	{
		if (srgb == 1.0f) return 1.0f;

		return srgb <= 0.04045f ? srgb / 12.92f : MathF.Pow((srgb + 0.055f) / 1.055f, 2.4f);
	}

	public Color ToLinear() => new(toLinear(RNormalized), toLinear(GNormalized), toLinear(BNormalized), ANormalized);

	private float toSRBG(float linear)
	{
		if (linear == 1.0f) return 1.0f;

		return linear < 0.0031308f ? 12.92f * linear : 1.055f * MathF.Pow(linear, 1.0f / 2.4f) - 0.055f;
	}

	public Color ToSRBG() => new(toSRBG(RNormalized), toSRBG(GNormalized), toSRBG(BNormalized), ANormalized);

	public static Color operator *(Color first, Color second)
	{
		var firstLinear = first.ToLinear();
		var secondLinear = second.ToLinear();

		return new Color(
			firstLinear.RNormalized * secondLinear.RNormalized,
			firstLinear.GNormalized * secondLinear.GNormalized,
			firstLinear.BNormalized * secondLinear.BNormalized,
			firstLinear.ANormalized * secondLinear.ANormalized
			).ToSRBG();
	}

	#endregion

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


	private Vector4 toHSL()
	{
		var max = Math.Max(RNormalized, Math.Max(GNormalized, BNormalized));
		var min = Math.Min(RNormalized, Math.Min(GNormalized, BNormalized));
		var diff = max - min;

		var h = 0.0f;
		if (max == RNormalized) h = ((GNormalized - BNormalized) / diff) % 6;
		else if (max == GNormalized) h = ((BNormalized - RNormalized) / diff) + 2f;
		else if (max == BNormalized) h = ((RNormalized - GNormalized) / diff) + 4f;

		if (h < 0) h += 6;
		else if (h > 6) h -= 6;

		var hue = h * 60f;

		var luminance = (max + min) / 2f;

		var saturation = 0.0f;
		if ((1.0f - Math.Abs((2.0f * luminance) - 1.0f)) != 0)
			saturation = diff / (1.0f - Math.Abs((2.0f * luminance) - 1.0f));

		return new Vector4(hue, saturation, luminance, ANormalized);

	}

	private void fromHSL(Vector4 hsl)
	{
		var hue = hsl.X;
		var saturation = hsl.Y;
		var luminance = hsl.Z;
		var alpha = hsl.W;

		var c = (1.0f - Math.Abs((2.0f * luminance) - 1.0f)) * saturation;

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

		var m = luminance - (c / 2.0f);
		if (m < 0)
		{
			m = 0;
		}

		R = (byte)((r + m) * byte.MaxValue);
		G = (byte)((g + m) * byte.MaxValue);
		B = (byte)((b + m) * byte.MaxValue);
		A = (byte)(alpha * byte.MaxValue);
	}

	/// <summary>
	/// The luminance of the color
	/// </summary>
	public float Luminance
	{
		get => toHSL().Z;
		set
		{
			var clamped = Math.Clamp(value, 0, 1);

			var hsl = toHSL();
			hsl.Z = clamped;
			fromHSL(hsl);
		}
	}

	/// <summary>
	/// The saturation of the color
	/// </summary>
	public float Saturation
	{
		get => toHSL().Y;
		set
		{
			var clamped = Math.Clamp(value, 0, 1);

			var hsl = toHSL();
			hsl.Y = clamped;
			fromHSL(hsl);
		}
	}

	public float Hue
	{
		get => toHSL().X;
		set
		{
			var clamped = Math.Clamp(value, 0, 360);

			var hsl = toHSL();
			hsl.X = clamped;
			fromHSL(hsl);
		}
	}

	#endregion

	public override readonly bool Equals(object? obj)
	{
		if (obj is not Color color) return false;

		return R == color.R && G == color.G && B == color.B && A == color.A;
	}
	public static bool operator ==(Color left, Color right) => left.Equals(right);
	public static bool operator !=(Color left, Color right) => !left.Equals(right);

	public override readonly int GetHashCode() => (R, G, B, A).GetHashCode();
	public override readonly string ToString() => $"({R}, {G}, {B})";
}
