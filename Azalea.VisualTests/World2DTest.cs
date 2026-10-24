using Azalea.SceneGraph;

namespace Azalea.VisualTests;

public class World2DTest : TestScene
{
	private readonly World2D _world;

	public World2DTest()
	{
		Add(_world = new World2D
		{
			Origin = Graphics.Anchor.Center,
			Anchor = Graphics.Anchor.Center,
			RelativeSizeAxes = Graphics.Axes.Both,
			NegativeSize = new(30),
			Child = new Entity
			{
				Children = [
					new Entity
					{
						Child = new Sprite2D(),
					},
					new Entity
					{
						Children = [
							new Entity(),
							new Entity(),
							new Entity()
						]
					}
				]
			}
		});
	}
}
