namespace Azalea.Text;

/// <summary>
/// All the data representing a single text character glyph.
/// </summary>
public struct Glyph
{
	/// <summary>
	/// Coordinates of all the indices for this glyph.
	/// </summary>
	public Vector2Int[] Coordinates { get; }

	/// <summary>
	/// Array of all the indices that mark an end of a contour
	/// </summary>
	public int[] ContourEndIndices { get; }

	public uint Unicode { get; set; }

	public Glyph(Vector2Int[] coordinates, int[] contourEndIndices)
	{
		Coordinates = coordinates;
		ContourEndIndices = contourEndIndices;
		Unicode = 0;
	}
}
