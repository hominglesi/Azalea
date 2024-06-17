namespace Azalea.Text;
public readonly struct Glyph
{
	public int IndexCount => Coordinates.Length;
	public int[] ContourEndIndices { get; init; }
	public Vector2Int[] Coordinates { get; init; }

	public Glyph(int[] contourEndIndices, int[] xCoordinates, int[] yCoordinates)
	{
		ContourEndIndices = contourEndIndices;

		Coordinates = new Vector2Int[xCoordinates.Length];
		for (int i = 0; i < xCoordinates.Length; i++)
			Coordinates[i] = new Vector2Int(xCoordinates[i], yCoordinates[i]);
	}
}
