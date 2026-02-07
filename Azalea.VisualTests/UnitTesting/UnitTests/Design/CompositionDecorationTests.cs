using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Graphics.Colors;

namespace Azalea.VisualTests.UnitTesting.UnitTests.Design;
public class CompositionDecorationTests : UnitTestSuite
{
	private static readonly Composition _composition = new()
	{
		Origin = Anchor.Center,
		Anchor = Anchor.Center,
		Size = new(300, 200),
		BackgroundColor = Palette.Flowers.Azalea,
		BorderAlpha = 0.2f,
		BorderThickness = new(top: 20, right: 10, bottom: 15, left: 25),
		BorderAlignment = BorderAlignment.Outer
	};

	public class BorderAlignmentTest : UnitTest
	{

		public BorderAlignmentTest()
		{
			AddOperation("Add black border", () => _composition.BorderColor = Palette.Black);

			AddOperation("Set alignment to Inside", () => _composition.BorderAlignment = BorderAlignment.Inner);
			AddOperation("Set alignment to Center", () => _composition.BorderAlignment = BorderAlignment.Center);
		}

		public override void Setup(UnitTestContainer scene)
		{
			base.Setup(scene);

			scene.Add(_composition);
		}

		public override void TearDown(UnitTestContainer scene)
		{
			base.TearDown(scene);

			_composition.BorderColor = null;
			_composition.BorderAlignment = BorderAlignment.Outer;
			scene.Remove(_composition);
		}
	}
}
