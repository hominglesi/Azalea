using Azalea.Design.Containers;
using Azalea.Graphics;
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
		Add(new FlexContainer()
		{
			Direction = FlexDirection.Vertical,
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
		_glyphCountText.Text = $"Glyph Count: {font.Glyphs.Length}";
		_unitsPerEmText.Text = $"Units Per Em: {font.UnitsPerEm}";
	}
}
