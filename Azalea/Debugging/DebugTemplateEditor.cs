using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using Azalea.IO.Resources;

namespace Azalea.Debugging;
public class DebugTemplateEditor : Composition
{
	private readonly SpritePattern _background;
	public DebugTemplateEditor()
	{
		AddInternal(_background = new SpritePattern()
		{
			Texture = Assets.GetTexture("Textures/pattern.png"),
			RelativeSizeAxes = Axes.Both
		});
	}

	public void InspectObject(GameObject gameObject)
	{
		Clear();
		Add(gameObject);
		gameObject.Origin = Anchor.Center;
		gameObject.Anchor = Anchor.Center;
	}
}
