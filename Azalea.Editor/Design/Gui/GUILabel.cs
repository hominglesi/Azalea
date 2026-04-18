using Azalea.Design.Containers;
using Azalea.Graphics;

namespace Azalea.Editor.Design.Gui;
public class GUILabel : TextContainer
{
	internal GUILabel(string text)
		: base(spriteText => spriteText.Font = GUIConstants.Font)
	{
		RelativeSizeAxes = Axes.X;
		AutoSizeAxes = Axes.Y;

		Text = text;
	}
}
