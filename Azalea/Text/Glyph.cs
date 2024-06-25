namespace Azalea.Text;

public readonly struct Glyph
{
	public readonly Vector2Int[] Coordinates { get; }
	public readonly int[] ContourEndIndices { get; }
	public readonly uint UnicodeValue { get; }
	public readonly int GlyphIndex { get; }
	public readonly Vector2Int Size { get; }

	internal readonly bool Exists => Coordinates != null;

	public Glyph(Vector2Int[] coordinates, int[] contourEndIndices, int glyphIndex, uint unicodeValue, Vector2Int size)
	{
		Coordinates = coordinates;
		ContourEndIndices = contourEndIndices;
		GlyphIndex = glyphIndex;
		UnicodeValue = unicodeValue;
		Size = size;
	}
}
