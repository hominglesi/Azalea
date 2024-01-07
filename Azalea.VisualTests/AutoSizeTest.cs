using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Graphics.Colors;
using Azalea.Inputs;
using System.Numerics;

namespace Azalea.VisualTests;
public class AutoSizeTest : TestScene
{
	private Composition _composition;

	private Box _box;

	public AutoSizeTest()
	{
		Add(_composition = new Composition()
		{
			Position = new(100),
			AutoSizeAxes = Graphics.Axes.Both,
			BorderColor = Palette.Silver,
			Child = _box = new Box()
			{
				Size = new(100),
				Color = Palette.Orange
			}
		});
	}

	protected override void Update()
	{
		_box.Position = Input.MousePosition - new Vector2(100);
	}
}
