using System;

namespace Azalea.Graphics.Colors;

public struct DrawColorInfo : IEquatable<DrawColorInfo>
{
	public ColorQuad Color;

	public DrawColorInfo(ColorQuad? color = null)
	{
		Color = color ?? ColorQuad.SolidColor(Palette.White);
	}

	public readonly bool Equals(DrawColorInfo other) => Color == other.Color;
}
