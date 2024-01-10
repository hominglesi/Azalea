using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Design.UserInterface.Basic;
using Azalea.Graphics.Colors;

namespace Azalea.VisualTests;
internal class SliderTests : TestScene
{
	public SliderTests()
	{
		Add(new Composition()
		{
			Position = new(500, 600),
			Size = new(400, 300),
			BorderThickness = new(2),
			Child = new BasicSlider()
			{
				Origin = Graphics.Anchor.TopRight,
				Anchor = Graphics.Anchor.TopRight,
				Position = new(200, 100),
				Size = new(300, 40),
				BorderColor = Palette.Teal,
				BorderThickness = new(1)
			}
		});


		Add(new BasicSlider()
		{
			Position = new(500, 200),
			Size = new(40, 300),
			Direction = SliderDirection.Vertical,
			BorderColor = Palette.Teal,
			BorderThickness = new(1)
		});
	}
}
