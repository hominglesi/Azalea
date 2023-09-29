using System;
using System.Diagnostics;

namespace Azalea.Graphics.Colors;
public partial struct ColorQuad : IEquatable<ColorQuad>
{
	public Color TopLeft;
	public Color BottomLeft;
	public Color BottomRight;
	public Color TopRight;
	public bool HasSingleColor;

	public ColorQuad(Color topLeft, Color bottomLeft, Color bottomRight, Color topRight)
	{
		TopLeft = topLeft;
		BottomLeft = bottomLeft;
		BottomRight = bottomRight;
		TopRight = topRight;

		HasSingleColor = topLeft == bottomLeft && topLeft == bottomRight && topLeft == topRight;
	}

	public ColorQuad(Color color)
	{
		TopLeft = color;
		BottomLeft = color;
		BottomRight = color;
		TopRight = color;

		HasSingleColor = true;
	}

	public static ColorQuad SolidColor(Color color) => new(color);

	public static ColorQuad GradientHorizontal(Color leftColor, Color rightColor)
		=> new(leftColor, leftColor, rightColor, rightColor);

	public static ColorQuad GradientVertical(Color topColor, Color bottomColor)
		=> new(topColor, bottomColor, bottomColor, topColor);

	internal Color SingleColor
	{
		readonly get
		{
			Debug.Assert(HasSingleColor);
			return TopLeft;
		}
		set
		{
			TopLeft = BottomLeft = BottomRight = TopRight = value;
			HasSingleColor = true;
		}
	}

	public readonly bool TryGetSingleColor(out Color color)
	{
		color = TopLeft;
		return HasSingleColor;
	}

	public void ApplyChild(ColorQuad child)
	{
		if (child.HasSingleColor && HasSingleColor)
		{
			SingleColor *= child.SingleColor;
		}
		else
		{
			HasSingleColor = false;
			TopLeft *= child.TopLeft;
			BottomLeft *= child.BottomLeft;
			BottomRight *= child.BottomRight;
			TopRight *= child.TopRight;
		}
	}

	public readonly ColorQuad MultiplyAlpha(float alpha)
	{
		if (alpha == 1f) return this;

		var result = this;
		result.TopLeft.MultiplyAlpha(alpha);
		result.BottomLeft.MultiplyAlpha(alpha);
		result.BottomRight.MultiplyAlpha(alpha);
		result.TopRight.MultiplyAlpha(alpha);

		return result;
	}

	public bool Equals(ColorQuad other)
	{
		if (HasSingleColor == false)
		{
			if (other.HasSingleColor) return false;

			return TopLeft.Equals(other.TopLeft) &&
				BottomLeft.Equals(other.BottomLeft) &&
				BottomRight.Equals(other.BottomRight) &&
				TopRight.Equals(other.TopRight);
		}

		return other.HasSingleColor && SingleColor.Equals(other.SingleColor);
	}

	public override bool Equals(object? obj) => obj is ColorQuad other && Equals(other);
	public static bool operator ==(ColorQuad left, ColorQuad right) => left.Equals(right);
	public static bool operator !=(ColorQuad left, ColorQuad right) => left.Equals(right) == false;

	public static implicit operator ColorQuad(Color color) => SolidColor(color);

	public override readonly int GetHashCode() => HashCode.Combine(TopLeft, BottomLeft, BottomRight, TopRight, HasSingleColor);
}
