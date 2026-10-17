using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Utils;

namespace Azalea.VisualTests;

public class SplitContainerTest : TestScene
{
	private readonly SplitContainer _subContainer;
	private readonly SplitContainer _container;
	public SplitContainerTest()
	{
		_subContainer = new SplitContainer(new Box() { Color = Rng.Color() }, new Box() { Color = Rng.Color() })
		{
			//Direction = SplitDirection.Vertical,
			ReversedPriority = true
		};

		Add(_container = new SplitContainer(new Box() { Color = Rng.Color() }, _subContainer)
		{
			RelativeSizeAxes = Axes.Both,
		});
	}
}
