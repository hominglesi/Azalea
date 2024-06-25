using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Text;
using System.Numerics;

namespace Azalea.VisualTests.TextRendering;
public class StringGlyphDisplay : FlexContainer
{
	private string _text = "";

	public string Text
	{
		get => _text;
		set
		{
			if (value == _text) return;

			_text = value;

			recalculateDisplay();
		}
	}

	private Font? _font;
	public Font? Font
	{
		get => _font;
		set
		{
			if (value == _font) return;

			_font = value;

			recalculateDisplay();
		}
	}

	private float _glyphScale;
	public float GlyphScale
	{
		get => _glyphScale;
		set
		{
			if (value == _glyphScale) return;

			_glyphScale = value;

			recalculateDisplay();
		}
	}

	private void recalculateDisplay()
	{
		if (_text == "" || _font is null)
			return;

		Clear();

		for (int i = 0; i < _text.Length; i++)
		{
			var chr = _text[i];

			if (chr == ' ')
			{
				Add(new Box() { Width = 100 * _glyphScale, Alpha = 1 });
			}
			else
			{
				var glyph = _font.GetGlyph(chr);

				var display = new GlyphDisplay()
				{
					GlyphScale = _glyphScale,
					Size = (Vector2)glyph.Size * _glyphScale,
					Margin = new(0, 30 * _glyphScale, 0, 0)
				};

				Add(display);

				display.Display(glyph);
			}
		}
	}
}
