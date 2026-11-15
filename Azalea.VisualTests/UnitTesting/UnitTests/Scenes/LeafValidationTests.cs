using Azalea.Scenes;

namespace Azalea.VisualTests.UnitTesting.UnitTests.Scenes;

public class LeafValidationTests : UnitTestSuite
{
	public class CreationValidation : UnitTest
	{
		private Forest? _forest;
		private Leaf? _leaf1;
		private Leaf? _leaf2;

		public CreationValidation()
		{
			AddOperation("Create forest", () => _forest = new Forest());
			AddResult("Forest inv: None", () => _forest!.Invalidations == Invalidation.None);
			AddOperation("Create leaf1", () => _leaf1 = new Leaf());
			AddResult("Leaf1 inv: None", () => _leaf1!.Invalidations == Invalidation.None);
			AddOperation("Add leaf1 to forest", () => _forest!.Add(_leaf1!));
			AddResult("Forest inv: Presence", () => _forest!.Invalidations == Invalidation.Presence);
			AddResult("Leaf1 inv: Presence | Transform", () => _leaf1!.Invalidations == (Invalidation.Presence | Invalidation.Transform));

			AddOperation("Validate forest", () => _forest!.Validate());
			AddResult("Forest inv: None", () => _forest!.Invalidations == Invalidation.None);
			AddResult("Leaf1 inv: None", () => _leaf1!.Invalidations == Invalidation.None);

			AddOperation("Add leaf2 to leaf1", () => _leaf1!.Add(_leaf2 = new Leaf()));
			AddResult("Forest inv: Presence", () => _forest!.Invalidations == Invalidation.Presence);
			AddResult("Leaf1 inv: Presence", () => _leaf1!.Invalidations == Invalidation.Presence);
			AddResult("Leaf2 inv: Presence | Transform", () => _leaf2!.Invalidations == (Invalidation.Presence | Invalidation.Transform));

			AddOperation("Validate forest", () => _forest!.Validate());
			AddResult("Everything is valid", () =>
			{
				return _forest!.Invalidations == Invalidation.None &&
					_leaf1!.Invalidations == Invalidation.None &&
					_leaf2!.Invalidations == Invalidation.None;
			});
		}
	}
}
