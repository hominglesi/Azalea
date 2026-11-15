using Azalea.Design.Containers;
using Azalea.Platform.Rendering.Coordination;

namespace Azalea.Scenes;

public class ForestContainer : Composition
{
	public readonly Forest Forest = new();

	protected override void FixedUpdate()
	{
		Forest.Validate();
		Forest.FixedUpdate();
	}

	protected override void Update()
	{
		Forest.Validate();
	}

	public override void Draw(RenderCoordinator coordinator)
	{
		Forest.Draw(coordinator);
	}
}
