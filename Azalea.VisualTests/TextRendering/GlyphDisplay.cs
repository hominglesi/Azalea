using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Text;
using Azalea.Utils;
using System;
using System.Numerics;

namespace Azalea.VisualTests.TextRendering;
public class GlyphDisplay : Composition
{
	public float GlyphScale = 1f;

	public GlyphDisplay()
	{
		AddInternal(new Box()
		{
			Origin = Anchor.Center,
			Size = new(10),
			Color = Palette.Black
		});
	}

	public void Display(Glyph glyph)
	{
		Clear();

		for (int i = 0; i < glyph.Coordinates.Length; i++)
		{
			var position = (Vector2)glyph.Coordinates[i] * GlyphScale;

			Add(new Box()
			{
				Origin = Anchor.Center,
				Size = new(10),
				Position = position
			});
		}

		int contourStartIndex = 0;

		foreach (var contourEndIndex in glyph.ContourEndIndices)
		{
			int contourIndexCount = contourEndIndex - contourStartIndex + 1;
			Span<Vector2Int> coords = glyph.Coordinates.AsSpan(contourStartIndex, contourIndexCount);

			for (int i = 0; i < coords.Length; i += 2)
			{
				var c0 = (Vector2)coords[i] * GlyphScale;
				var c1 = (Vector2)coords[(i + 1) % coords.Length] * GlyphScale;
				var c2 = (Vector2)coords[(i + 2) % coords.Length] * GlyphScale;
				addBezier(c0, c1, c2, 10);
			}

			contourStartIndex = contourEndIndex + 1;
		}
	}

	private void addBezier(Vector2 p0, Vector2 p1, Vector2 p2, int resolution)
	{
		Vector2 prevPointOnCurve = p0;

		for (int i = 0; i < resolution; i++)
		{
			float t = (i + 1f) / resolution;
			Vector2 nextPointOnCurve = MathUtils.InterpolateBezier(p0, p1, p2, t);
			Add(new Line()
			{
				StartPoint = prevPointOnCurve + Position,
				EndPoint = nextPointOnCurve + Position
			});
			prevPointOnCurve = nextPointOnCurve;
		}
	}
}
