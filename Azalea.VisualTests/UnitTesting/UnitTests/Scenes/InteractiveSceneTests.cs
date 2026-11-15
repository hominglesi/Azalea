using Azalea.Scenes;

namespace Azalea.VisualTests.UnitTesting.UnitTests.Scenes;

public class InteractiveSceneTests : UnitTestSuite
{
	public class MouseFollow : UnitTest
	{
		private CustomForestContainer? _container;

		public MouseFollow()
		{
			AddOperation("Create container", () => TestContainer!.Add(_container = new CustomForestContainer()));
			AddOperation("Add sprite", () => _container!.Forest.Add(new Sprite()
			{
				Position = new(900, 500),
				Size = new(200, 100)
			}));
			AddOperation("Sprite follows mouse", () => _container!.FollowTarget = _container.Forest.Children[0]);
		}

		public override void TearDown(UnitTestContainer scene)
		{
			if (_container is not null)
				scene.Remove(_container);
		}

		class CustomForestContainer : ForestContainer
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
