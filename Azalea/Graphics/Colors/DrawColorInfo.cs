using System;

namespace Azalea.Graphics.Colors;

public struct DrawColorInfo(ColorQuad? color = null) : IEquatable<DrawColorInfo>
{
	public ColorQuad Color = color ?? ColorQuad.SolidColor(Palette.White);

	public readonly bool Equals(DrawColorInfo other) => Color == other.Color;
	public readonly override bool Equals(object? obj)
		=> obj is DrawColorInfo drawInfo && Equals(drawInfo);

	public static bool operator ==(DrawColorInfo left, DrawColorInfo right) => left.Equals(right);
	public static bool operator !=(DrawColorInfo left, DrawColorInfo right) => !left.Equals(right);

	public readonly override int GetHashCode() => HashCode.Combine(Color);
}
