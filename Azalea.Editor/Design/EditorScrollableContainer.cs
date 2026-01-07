using Azalea.Design.Containers;
using Azalea.Design.UserInterface;

namespace Azalea.Editor.Design;
internal class EditorScrollableContainer : ScrollableContainer
{
	protected override Slider CreateSlider()
		=> new EditorScrollbar();
}
