using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Text;
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

		for (int i = 0; i < glyph.IndexCount; i++)
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

			for (int i = 0; i < coords.Length; i++)
			{
				Add(new Line()
				{
					StartPoint = (Vector2)coords[i] * GlyphScale + Position,
					EndPoint = (Vector2)coords[(i + 1) % coords.Length] * GlyphScale + Position
				});
			}

			contourStartIndex = contourEndIndex + 1;
		}
	}
}
