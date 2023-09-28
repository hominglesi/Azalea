using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;

namespace Azalea.Design.Compositions;
public class BasicWindow : Window
{
	public BasicWindow()
	{
		GameObject draggableBar;

		Size = new(500, 440);
		AddInternal(draggableBar = new Box()
		{
			Color = Palette.Black,
			RelativeSizeAxes = Axes.X,
			Size = new(1, 40),
			Depth = -100
		});
		InternalComposition.Position = new(0, 40);
		InternalComposition.RelativeSizeAxes = Axes.None;
		InternalComposition.Size = new(500, 400);
		Add(new Box()
		{
			Color = Palette.White,
			RelativeSizeAxes = Axes.Both
		});

		AddDragableSurface(draggableBar);
	}
}
