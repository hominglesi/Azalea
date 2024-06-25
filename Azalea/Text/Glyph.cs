namespace Azalea.Text;

public readonly struct Glyph
{
	public readonly GlyphPoint[] Coordinates { get; }
	public readonly int[] ContourEndIndices { get; }
	public readonly uint UnicodeValue { get; }
	public readonly int GlyphIndex { get; }
	public readonly Vector2Int Size { get; }

	internal readonly bool Exists => Coordinates != null;

	public Glyph(GlyphPoint[] coordinates, int[] contourEndIndices, int glyphIndex, uint unicodeValue, Vector2Int size)
	{
		Coordinates = coordinates;
		ContourEndIndices = contourEndIndices;
		GlyphIndex = glyphIndex;
		UnicodeValue = unicodeValue;
		Size = size;
	}
}

public readonly struct GlyphPoint
{
	public readonly Vector2Int Position;
	public readonly bool OnCurve;

	public GlyphPoint(int x, int y, bool onCurve)
	{
		Position = new Vector2Int(x, y);
		OnCurve = onCurve;
	}
}
