using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using System.Numerics;

#pragma warning disable CS8602, CS8618

namespace Azalea.VisualTests.UnitTesting.UnitTests.SceneGraph;
public class GameObjectTests : UnitTestSuite
{
	public class GameObjectNegativeSizeTest : UnitTest
	{
		private Box _box;
		private Composition _boxParent;

		public GameObjectNegativeSizeTest()
		{
			AddOperation("Set Box.Size to (100, 100)", () => _box.Size = new(100));
			AddOperation("Set Box.NegativeSize to (10, 20)", () => _box.NegativeSize = new(10, 20));
			AddResult("Check if Box.DrawSize is (90, 80)", () => _box.DrawSize == new Vector2(90, 80));
			AddOperation("Set Box size relatively to (150, 150) ", () =>
			{
				_box.RelativeSizeAxes = Axes.Both;
				_box.Size = Vector2.One;
			});
			AddResult("Check if Box.DrawSize is (140, 130)", () => _box.DrawSize == new Vector2(140, 130));
			AddOperation("Set Box.Parent.NegativeSize to (20, 30)", () => _boxParent.NegativeSize = new(20, 30));
			AddResult("Check if Box.DrawSize is (120, 100)", () => _box.DrawSize == new Vector2(120, 100));
		}

		public override void Setup(Composition scene)
		{
			_boxParent = new Composition()
			{
				Origin = Anchor.Center,
				Anchor = Anchor.Center,
				Size = new(150),
				Child = _box = new Box()
				{
					Origin = Anchor.Center,
					Anchor = Anchor.Center,
					Size = new(50)
				}
			};

			scene.Add(_boxParent);
		}

		public override void TearDown(Composition scene)
		{
			scene.Remove(_boxParent);
		}
	}
}

#pragma warning restore CS8602, CS8618
