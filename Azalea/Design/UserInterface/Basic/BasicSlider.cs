using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;

namespace Azalea.Design.UserInterface.Basic;
public class BasicSlider : Slider
{
	protected override GameObject CreateBody()
		=> new HollowBox()
		{
			RelativeSizeAxes = Axes.Both,
		};

	protected override GameObject CreateHead()
		=> new Box()
		{
			Origin = Anchor.Center,
			Size = new(25),
			Color = Palette.Flowers.Cornflower
		};
}
