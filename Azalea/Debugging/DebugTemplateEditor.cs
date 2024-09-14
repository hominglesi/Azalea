using Azalea.Debugging.Gizmos;
using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using Azalea.IO.Resources;

namespace Azalea.Debugging;
public class DebugTemplateEditor : Composition
{
	private readonly SpritePattern _background;
	private readonly ResizeGizmo _resizeGizmo;

	public DebugTemplateEditor()
	{
		AddInternal(_background = new SpritePattern()
		{
			Texture = Assets.GetTexture("Textures/pattern.png"),
			RelativeSizeAxes = Axes.Both
		});

		_resizeGizmo = new ResizeGizmo()
		{
			RelativeSizeAxes = Axes.Both
		};
	}

	public void InspectObject(GameObject gameObject)
	{
		Clear();
		Add(gameObject);

		gameObject.Position = (DrawSize / 2) - (gameObject.DrawSize / 2);

		if (_resizeGizmo.Parent != this)
			AddInternal(_resizeGizmo);
		_resizeGizmo.SetTarget(gameObject);
	}
}
