using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Text;

namespace Azalea.VisualTests.TextRendering;
public class FontInfoDisplay : Composition
{
	private SpriteText _tableCountText;
	private SpriteText _glyphCountText;
	private SpriteText _unitsPerEmText;
	public FontInfoDisplay()
	{
		Add(new SpriteText()
		{
			Anchor = Anchor.TopCenter,
			Origin = Anchor.TopCenter,
			Y = 10,
			Text = "Font Description",
			Color = Palette.Black,
			Font = FontUsage.Default.With(size: 24)
		});

		Add(new FlexContainer()
		{
			Position = new(10, 50),
			Direction = FlexDirection.Vertical,
			Color = Palette.Black,
			Children = new GameObject[]
			{
				_tableCountText = new SpriteText(),
				_glyphCountText = new SpriteText(),
				_unitsPerEmText = new SpriteText(),
			}
		});
	}

	public void Display(Font font)
	{
		_tableCountText.Text = $"Font Tables: {font.FontTableOffsets.Count}";
		_glyphCountText.Text = $"Glyph Count: {font.Glyphs.Length}";
		_unitsPerEmText.Text = $"Units Per Em: {font.UnitsPerEm}";
	}
}
