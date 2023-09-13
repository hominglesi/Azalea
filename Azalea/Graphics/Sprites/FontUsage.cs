using System;

namespace Azalea.Graphics.Sprites;

public readonly struct FontUsage : IEquatable<FontUsage>
{
	private const float default_text_size = 20;

	public static FontUsage Default => new(null);

	public string? Family { get; }
	public string? Weight { get; }
	public bool Italics { get; }
	public float Size { get; }
	public bool FixedWidth { get; }
	public string FontName { get; }

	public FontUsage(string? family = null, float size = default_text_size, string? weight = null, bool italics = false, bool fixedWidth = false)
	{
		Family = family;
		Size = size >= 0 ? size : throw new ArgumentOutOfRangeException(nameof(size), "Must be non-negative.");
		Weight = weight;
		Italics = italics;
		FixedWidth = fixedWidth;

		FontName = Family + "-";

		if (string.IsNullOrEmpty(weight) == false)
			FontName += Weight;

		if (italics)
			FontName += "Italic";

		FontName = FontName.TrimEnd('-');
	}


	public FontUsage With(string? family = null, float? size = null, string? weight = null, bool? italics = null, bool? fixedWidth = null)
		=> new(family ?? Family, size ?? Size, weight ?? Weight, italics ?? Italics, fixedWidth ?? FixedWidth);

	public override string ToString() => $"Font={FontName}, Size={Size}, Italics={Italics}, FixedWidth={FixedWidth}";
	public bool Equals(FontUsage other) => Family == other.Family && Weight == other.Weight && Italics.Equals(other.Italics)
		&& Size.Equals(other.Size) && FixedWidth.Equals(other.FixedWidth);
	public override bool Equals(object? obj) => obj is FontUsage other && Equals(other);
	public override int GetHashCode() => HashCode.Combine(Family, Weight, Italics, Size, FixedWidth);
	public static bool operator ==(FontUsage left, FontUsage right) => left.Equals(right);
	public static bool operator !=(FontUsage left, FontUsage right) => !left.Equals(right);
}
