using System;

namespace Azalea.Graphics.Colors;

public struct DrawColorInfo : IEquatable<DrawColorInfo>
{
	public Color Color;

	public DrawColorInfo(Color color)
	{
		Color = color;
	}

	public readonly bool Equals(DrawColorInfo other) => Color == other.Color;
}
