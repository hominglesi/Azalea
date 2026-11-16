using Azalea.Scenes;

namespace Azalea.VisualTests.UnitTesting.UnitTests.Scenes;

public class LeafValidationTests : UnitTestSuite
{
	public class CreationValidation : UnitTest
	{
		private Scene2D? _scene2D;
		private Leaf? _leaf1;
		private Leaf? _leaf2;

		public CreationValidation()
		{
			AddOperation("Create scene2D", () => _scene2D = new Scene2D());
			AddResult("Scene2D inv: None", () => _scene2D!.Invalidations == Invalidation.None);
			AddOperation("Create leaf1", () => _leaf1 = new Leaf());
			AddResult("Leaf1 inv: None", () => _leaf1!.Invalidations == Invalidation.None);
			AddOperation("Add leaf1 to scene2D", () => _scene2D!.Add(_leaf1!));
			AddResult("Scene2D inv: Presence", () => _scene2D!.Invalidations == Invalidation.Presence);
			AddResult("Leaf1 inv: Presence | Transform", () => _leaf1!.Invalidations == (Invalidation.Presence | Invalidation.Transform));

			AddOperation("Validate scene2D", () => _scene2D!.Validate());
			AddResult("Scene2D inv: None", () => _scene2D!.Invalidations == Invalidation.None);
			AddResult("Leaf1 inv: None", () => _leaf1!.Invalidations == Invalidation.None);

			AddOperation("Add leaf2 to leaf1", () => _leaf1!.Add(_leaf2 = new Leaf()));
			AddResult("Scene2D inv: Presence", () => _scene2D!.Invalidations == Invalidation.Presence);
			AddResult("Leaf1 inv: Presence", () => _leaf1!.Invalidations == Invalidation.Presence);
			AddResult("Leaf2 inv: Presence | Transform", () => _leaf2!.Invalidations == (Invalidation.Presence | Invalidation.Transform));

			AddOperation("Validate scene2D", () => _scene2D!.Validate());
			AddResult("Everything is valid", () =>
			{
				return _scene2D!.Invalidations == Invalidation.None &&
					_leaf1!.Invalidations == Invalidation.None &&
					_leaf2!.Invalidations == Invalidation.None;
			});
		}
	}
}
