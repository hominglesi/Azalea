using System;

namespace Azalea.Graphics.Colors;

public struct DrawColorInfo : IEquatable<DrawColorInfo>
{
	public ColorInfo Color;

	public DrawColorInfo(ColorInfo? color = null)
	{
		Color = color ?? ColorInfo.SolidColor(Palette.White);
	}

	public readonly bool Equals(DrawColorInfo other) => Color == other.Color;
}
