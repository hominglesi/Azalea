using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Text;
public static class TextExtentions
{
	public static List<Vector2[]> CreateContoursWithImpliedPoints(this Glyph glyph, float scale)
	{
		var contours = new List<Vector2[]>();
		int contoursStart = 0;

		foreach (var contourEnd in glyph.ContourEndIndices)
		{
			Span<GlyphPoint> originalContour = glyph.Coordinates.AsSpan(contoursStart, contourEnd - contoursStart + 1);
			int pointOffset;

			for (pointOffset = 0; pointOffset < originalContour.Length; pointOffset++)
				if (originalContour[pointOffset].OnCurve) break;

			var newContour = new List<Vector2>();
			for (int i = 0; i <= originalContour.Length; i++)
			{
				GlyphPoint current = originalContour[(i + pointOffset) % originalContour.Length];
				GlyphPoint next = originalContour[(i + pointOffset + 1) % originalContour.Length];
				newContour.Add(current.Position * scale);

				if (current.OnCurve == next.OnCurve && i < originalContour.Length)
				{
					var midpoint = (current.Position + next.Position) / 2 * scale;
					newContour.Add(midpoint);
				}
			}
			contours.Add(newContour.ToArray());
			contoursStart = contourEnd + 1;
		}

		return contours;
	}
}
