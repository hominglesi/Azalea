using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Text;
using Azalea.Utils;
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

	private Vector2 _lastPosition;

	protected override void Update()
	{
		if (_lastPosition != Position)
		{
			Display(_glyph);
			_lastPosition = Position;
		}
	}

	private Glyph _glyph;
	public void Display(Glyph glyph)
	{
		_glyph = glyph;

		Clear();

		for (int i = 0; i < glyph.Coordinates.Length; i++)
		{
			var position = glyph.Coordinates[i].Position * GlyphScale;

			Add(new Box()
			{
				Origin = Anchor.Center,
				Size = new(10),
				Position = position
			});
		}
		Add(new HollowBox()
		{
			Size = (Vector2)glyph.Size * GlyphScale,
			Y = -glyph.Size.Y * GlyphScale
		});

		var glyphContours = glyph.CreateContoursWithImpliedPoints(GlyphScale);

		foreach (var contour in glyphContours)
		{
			for (int i = 0; i < contour.Length; i += 2)
			{
				var c0 = contour[i];
				var c1 = contour[(i + 1) % contour.Length];
				var c2 = contour[(i + 2) % contour.Length];
				addBezier(c0, c1, c2, 10);
			}
		}
	}

	private void addBezier(Vector2 p0, Vector2 p1, Vector2 p2, int resolution)
	{
		Vector2 prevPointOnCurve = p0;

		for (int i = 0; i < resolution; i++)
		{
			float t = (i + 1f) / resolution;
			Vector2 nextPointOnCurve = MathUtils.InterpolateBezier(p0, p1, p2, t);
			var line = new Line()
			{
				StartPoint = prevPointOnCurve + Position,
				EndPoint = nextPointOnCurve + Position
			};

			if (Parent is not null)
			{
				line.StartPoint += Parent.Position;
				line.EndPoint += Parent.Position;
			}

			Add(line);
			prevPointOnCurve = nextPointOnCurve;
		}
	}
}
