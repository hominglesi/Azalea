using System;
using System.Text;

namespace Azalea.Graphics.Sprites;

public readonly struct FontUsage : IEquatable<FontUsage>
{
	public const string DefaultFontName = "Roboto-Regular";
	private const float __defaultTextSize = 20;

	public string? Family { get; }
	public string? Weight { get; }
	public bool Italics { get; }
	public float Size { get; }
	public string FontName { get; }

	public static FontUsage Default => new("Roboto", __defaultTextSize, "Regular", false);

	public FontUsage(string family = "Roboto", float size = __defaultTextSize,
		string? weight = "Regular", bool italics = false)
	{
		Family = family;
		Size = size >= 0 ? size :
			throw new ArgumentOutOfRangeException(nameof(size), "Must be non-negative.");
		Weight = weight;
		Italics = italics;

		var fontNameBuilder = new StringBuilder();
		fontNameBuilder.Append(Family);

		if (string.IsNullOrEmpty(weight) == false || italics)
			fontNameBuilder.Append('-');

		if (string.IsNullOrEmpty(weight) == false)
			fontNameBuilder.Append(Weight);

		if (italics)
			fontNameBuilder.Append(Weight);

		FontName = fontNameBuilder.ToString();
	}

	public readonly FontUsage With(string? family = null, float? size = null,
		string? weight = null, bool? italics = null)
		=> new(family ?? Family!, size ?? Size, weight ?? Weight, italics ?? Italics);

	public override readonly string ToString() => $"Font={FontName}, Size={Size}, Weight={Weight} Italics={Italics}";
	public readonly bool Equals(FontUsage other) => Family == other.Family && Weight == other.Weight
		&& Italics.Equals(other.Italics) && Size.Equals(other.Size);
	public override bool Equals(object? obj) => obj is FontUsage other && Equals(other);
	public override readonly int GetHashCode() => HashCode.Combine(Family, Weight, Italics, Size);
	public static bool operator ==(FontUsage left, FontUsage right) => left.Equals(right);
	public static bool operator !=(FontUsage left, FontUsage right) => !left.Equals(right);
}
