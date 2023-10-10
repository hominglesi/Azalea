using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using System;

namespace Azalea.Debugging;
public class DebugConsole : ScrollableContainer
{
	public DebugConsole()
	{
		RemoveInternal(InternalComposition);
		AddInternal(new FlexContainer()
		{
			RelativeSizeAxes = Axes.Both,
			Direction = FlexDirection.Vertical,
			Wrapping = FlexWrapping.NoWrapping,
			Child = InternalComposition
		});
	}

	public void WriteLine(string text)
	{
		Console.WriteLine(text);
		Add(new SpriteText()
		{
			Text = text
		});
	}
}
