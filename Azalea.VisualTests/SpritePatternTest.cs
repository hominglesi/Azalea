using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using Azalea.IO.Resources;

namespace Azalea.VisualTests;
public class SpritePatternTest : TestScene
{
	public SpritePatternTest()
	{
		Add(new SpritePattern()
		{
			RelativeSizeAxes = Axes.Both,
			Texture = Assets.GetTexture("Textures/pattern.png")
		});
	}
}
