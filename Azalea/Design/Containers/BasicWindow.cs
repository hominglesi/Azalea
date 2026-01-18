using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;

namespace Azalea.Design.Containers;
public class BasicWindow : BasicWindowContainer
{
	private Composition _contentContainer;
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

		AddInternal(_contentContainer = new Composition()
		{
			Position = new(0, 40),
			RelativeSizeAxes = Axes.Both,
			NegativeSize = new(0, 40),
			BackgroundColor = Palette.White,
		});

		//REMOVED WHEN COMPOSITION WAS SIMPLIFIED PROBABLY BROKE SOMETHING
		//RemoveInternal(InternalComposition);
		//_contentContainer.AddInternal(InternalComposition);

		AddDragableSurface(draggableBar);
	}
}
