using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Containers;
using Azalea.Graphics.Shapes;

namespace Azalea.Design.Compositions;
public class BasicWindow : Window
{
	private readonly Container _content;
	protected override Container ContentContainer => _content;

	public BasicWindow()
	{
		GameObject draggableBar;

		Size = new(500, 440);
		AddRangeInternal(new GameObject[]
		{
			_content = new Container()
			{
				Position = new(0, 40),
				Size = new(500, 400),
				Child = new Box()
				{
					Color = Palette.White,
					RelativeSizeAxes = Axes.Both
				}
			},
			draggableBar = new Box()
			{
				Color = Palette.Black,
				RelativeSizeAxes = Axes.X,
				Size = new(1, 40)
			}
		});
		AddDragableSurface(draggableBar);
	}
}
