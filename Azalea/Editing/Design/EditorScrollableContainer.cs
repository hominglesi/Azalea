using Azalea.Design.Containers;
using Azalea.Design.UserInterface;

namespace Azalea.Editing.Design;
internal class EditorScrollableContainer : ScrollableContainer
{
	protected override Slider CreateSlider()
		=> new EditorScrollbar();
}
