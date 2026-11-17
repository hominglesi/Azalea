using Azalea.Design.Containers;
using Azalea.Scenes;
using Azalea.Scenes.Drawables2D;

namespace Azalea.VisualTests.UnitTesting.UnitTests.Scenes;

public class InteractiveSceneTests : UnitTestSuite
{
	public class MouseFollow : UnitTest
	{
		private CustomScene2DContainer? _container;

		public MouseFollow()
		{
			AddOperation("Create container", () => TestContainer!.Add(_container = new CustomScene2DContainer()));
			AddOperation("Add sprite", () => _container!.Scene.Add(new Sprite()
			{
				Position = new(900, 500),
				Size = new(200, 100)
			}));
			AddOperation("Sprite follows mouse", () => _container!.FollowTarget = _container.Scene.Children[0]);
		}

		public override void TearDown(UnitTestContainer scene)
		{
			if (_container is not null)
				scene.Remove(_container);
		}

		class CustomScene2DContainer : Scene2DContainer
		{
			public Leaf? FollowTarget = null;

			protected override void Update()
			{
				FollowTarget?.Position = App.Input.State.MousePosition;
				base.Update();
			}
		}
	}
}
