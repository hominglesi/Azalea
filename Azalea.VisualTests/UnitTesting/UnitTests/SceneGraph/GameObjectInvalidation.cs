using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using System;

#pragma warning disable CS8602, CS8618

namespace Azalea.VisualTests.UnitTesting.UnitTests.SceneGraph;
public class GameObjectInvalidation : UnitTestSuite
{
	public class InvalidateAlpha : UnitTest
	{
		private readonly Composition _testComposition = new()
		{
			RelativeSizeAxes = Axes.Both,
		};

		private Composition? _parentComposition;
		private Box? _childBox;

		public InvalidateAlpha()
		{
			AddOperation("Add Parent composition at alpha 0.5f",
				() => _testComposition.Add(_parentComposition = new Composition()
				{
					Origin = Anchor.Center,
					Anchor = Anchor.Center,
					Size = new(300, 150),
					Alpha = 0.5f,
					BorderColor = Palette.Black
				}));

			AddOperation("Add white Child box",
				() => _parentComposition.Add(_childBox = new Box()
				{
					RelativeSizeAxes = Axes.Both,
					Color = Palette.White
				}));

			AddResult("Check if DrawColorInfo is correct", () =>
			{
				var colorInfo = _childBox.DrawColorInfo.Color;

				if (colorInfo.TryGetSingleColor(out var color))
				{
					var targetColor = Palette.White;
					targetColor.MultiplyAlpha(0.5f);

					return color == targetColor;
				}

				Console.WriteLine("GameObjectInvalidateAlpha: Returned DrawColorInfo had multiple colors");
				return false;
			});

			AddOperation("Set child Alpha to 0.5f",
				() => _childBox.Alpha = 0.5f);

			AddResult("Check if DrawColorInfo is correct", () =>
			{
				var colorInfo = _childBox.DrawColorInfo.Color;

				if (colorInfo.TryGetSingleColor(out var color))
				{
					var targetColor = Palette.White;
					targetColor.MultiplyAlpha(0.5f);
					targetColor.MultiplyAlpha(0.5f);

					return color == targetColor;
				}

				Console.WriteLine("GameObjectInvalidateAlpha: Returned DrawColorInfo had multiple colors");
				return false;
			});

			AddResult("Check invalidation count", () =>
			{
				if (_parentComposition.InvalidationID != 1)
					return false;

				return _childBox.InvalidationID == 2;
			});

			AddOperation("Change parent Alpha to 0.75f",
				() => _parentComposition.Alpha = 0.75f);

			AddResult("Check if DrawColorInfo is correct", () =>
			{
				var colorInfo = _childBox.DrawColorInfo.Color;

				if (colorInfo.TryGetSingleColor(out var color))
				{
					var targetColor = Palette.White;
					targetColor.MultiplyAlpha(0.75f);
					targetColor.MultiplyAlpha(0.5f);

					return color == targetColor;
				}

				Console.WriteLine("GameObjectInvalidateAlpha: Returned DrawColorInfo had multiple colors");
				return false;
			});

			AddResult("Check invalidation count", () =>
			{
				if (_parentComposition.InvalidationID != 2)
					return false;

				return _childBox.InvalidationID == 3;
			});
		}

		public override void Setup(UnitTestContainer scene)
		{
			base.Setup(scene);

			scene.Add(_testComposition);
		}

		public override void TearDown(UnitTestContainer scene)
		{
			base.TearDown(scene);

			scene.Remove(_testComposition);
		}
	}

	public class InvalidateColor : UnitTest
	{
		private readonly Composition _testComposition = new()
		{
			RelativeSizeAxes = Axes.Both,
		};

		private Composition? _parentComposition;
		private Box? _childBox;

		public InvalidateColor()
		{
			AddOperation("Add red Parent composition",
				() => _testComposition.Add(_parentComposition = new Composition()
				{
					Origin = Anchor.Center,
					Anchor = Anchor.Center,
					Size = new(300, 150),
					Color = Palette.Red,
					BorderColor = Palette.Black
				}));

			AddOperation("Add blue Child box",
				() => _parentComposition.Add(_childBox = new Box()
				{
					RelativeSizeAxes = Axes.Both,
					Color = Palette.Blue
				}));

			AddResult("Check if DrawColorInfo is correct", () =>
			{
				var colorInfo = _childBox.DrawColorInfo.Color;

				if (colorInfo.TryGetSingleColor(out var color))
				{
					var targetColor = Palette.Red;
					targetColor *= Palette.Blue;

					return color == targetColor;
				}

				Console.WriteLine("GameObjectInvalidateAlpha: Returned DrawColorInfo had multiple colors");
				return false;
			});

			AddOperation("Set child Color to green", () => _childBox.Color = Palette.Green);

			AddResult("Check if DrawColorInfo is correct", () =>
			{
				var colorInfo = _childBox.DrawColorInfo.Color;

				if (colorInfo.TryGetSingleColor(out var color))
				{
					var targetColor = Palette.Red;
					targetColor *= Palette.Green;

					return color == targetColor;
				}

				Console.WriteLine("GameObjectInvalidateAlpha: Returned DrawColorInfo had multiple colors");
				return false;
			});

			AddResult("Check invalidation count", () =>
			{
				if (_parentComposition.InvalidationID != 1)
					return false;

				return _childBox.InvalidationID == 2;
			});

			AddOperation("Change parent Color to White", () => _parentComposition.Color = Palette.White);

			AddResult("Check if DrawColorInfo is correct", () =>
			{
				var colorInfo = _childBox.DrawColorInfo.Color;

				if (colorInfo.TryGetSingleColor(out var color))
				{
					var targetColor = Palette.White;
					targetColor *= Palette.Green;

					return color == targetColor;
				}

				Console.WriteLine("GameObjectInvalidateAlpha: Returned DrawColorInfo had multiple colors");
				return false;
			});

			AddResult("Check invalidation count", () =>
			{
				if (_parentComposition.InvalidationID != 2)
					return false;

				return _childBox.InvalidationID == 3;
			});
		}

		public override void Setup(UnitTestContainer scene)
		{
			base.Setup(scene);

			scene.Add(_testComposition);
		}

		public override void TearDown(UnitTestContainer scene)
		{
			base.TearDown(scene);

			scene.Remove(_testComposition);
		}
	}
}

#pragma warning restore CS8602, CS8618
