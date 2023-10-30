using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Platform;

namespace Azalea.VisualTests;
public class IWindowTest : TestScene
{
	public IWindowTest()
	{
		Add(new FlexContainer()
		{
			Direction = FlexDirection.Vertical,
			Spacing = new(0, 5),
			Children = new GameObject[]
			{
				new BasicButton()
				{
					Text = "Set WindowState to 'Normal'",
					Width = 300,
					Action = () => AzaleaGame.Main.Host.Window.State = WindowState.Normal
				},
				new BasicButton()
				{
					Text = "Set WindowState to 'Minimized'",
					Width = 300,
					Action = () => AzaleaGame.Main.Host.Window.State = WindowState.Minimized
				},
				new BasicButton()
				{
					Text = "Set WindowState to 'Maximized'",
					Width = 300,
					Action = () => AzaleaGame.Main.Host.Window.State = WindowState.Maximized
				}
			}
		});
	}
}
