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

	public readonly float RNormalized => (float)R / byte.MaxValue;
	public readonly float GNormalized => (float)G / byte.MaxValue;
	public readonly float BNormalized => (float)B / byte.MaxValue;
	public readonly float ANormalized => (float)A / byte.MaxValue;

	public override bool Equals(object? obj)
	{
		if (obj is not Color color) return false;

		return R == color.R && G == color.G && B == color.B && A == color.A;
	}
	public static bool operator ==(Color left, Color right) => left.Equals(right);
	public static bool operator !=(Color left, Color right) => !left.Equals(right);

	public override int GetHashCode() => (R, G, B, A).GetHashCode();
}
