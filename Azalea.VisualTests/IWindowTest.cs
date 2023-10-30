using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using Azalea.Platform;
using System;

namespace Azalea.VisualTests;
public class IWindowTest : TestScene
{
	private SpriteText _windowStateText;

	public IWindowTest()
	{
		AddRange(new GameObject[] {
			new FlexContainer()
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
				},
				new BasicButton()
				{
					Text = "Set Resizable to 'true'",
					Width = 300,
					Action = () => AzaleaGame.Main.Host.Window.Resizable = true
				},
				new BasicButton()
				{
					Text = "Set Resizable to 'false'",
					Width = 300,
					Action = () => AzaleaGame.Main.Host.Window.Resizable = false
				},
			}
		}, new FlexContainer()
		{
			Origin = Anchor.TopRight,
			Anchor = Anchor.TopRight,
			Width = 300,
			Children = new GameObject[]
			{
				new FlexContainer()
				{
					RelativeSizeAxes = Axes.X,
					Size = new(1, 40),
					Children = new GameObject[]
					{
						new SpriteText()
						{
							Text = "WindowState: "
						},
						_windowStateText = new SpriteText()
						{

						},
					}
				}
			}
		}
		});
	}

	private WindowState? _lastState;

	protected override void Update()
	{
		var window = AzaleaGame.Main.Host.Window;

		if (window.State != _lastState)
		{
			_windowStateText.Text = window.State.ToString();
			Console.WriteLine($"Window state changed to {window.State}");
			_lastState = window.State;
		};
	}
}
