using Azalea.Design.Containers;
using Azalea.Editing.Gizmos;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using Azalea.IO.Resources;

namespace Azalea.Editing.Views;
public class TemplateEditor : Composition
{
	private readonly ResizeGizmo _resizeGizmo;

	public TemplateEditor()
	{
		RelativeSizeAxes = Axes.Both;

		AddInternal(new SpritePattern()
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

		gameObject.Position = DrawSize / 2 - gameObject.DrawSize / 2;

		if (_resizeGizmo.Parent != this)
			AddInternal(_resizeGizmo);
		_resizeGizmo.SetTarget(gameObject);
	}
}
