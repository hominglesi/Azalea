using Azalea.Platform.Rendering.Coordination;
using Azalea.Scenes;

namespace Azalea.Design.Containers;

public class Scene2DContainer : Composition
{
	public readonly Scene2D Scene = new();

	protected override void FixedUpdate()
	{
		Scene.Validate();
		Scene.FixedUpdate();
	}

	protected override void Update()
	{
		Scene.Validate();
	}

	public override void Draw(RenderCoordinator coordinator)
	{
		Scene.Draw(coordinator);
	}
}
