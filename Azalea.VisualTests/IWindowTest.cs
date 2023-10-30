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
	private SpriteText _windowTitleText;
	private SpriteText _windowResizableText;

	public IWindowTest()
	{
		AddRange(new GameObject[] {
			new FlexContainer()
		{
			RelativeSizeAxes = Axes.Both,
			Direction = FlexDirection.Vertical,
			Spacing = new(5),
			Wrapping = FlexWrapping.Wrap,
			Children = new GameObject[]
			{
				createActionButton(
					"Set WindowState to 'Normal'",
					() => AzaleaGame.Main.Host.Window.State = WindowState.Normal),
				createActionButton(
					"Set WindowState to 'Minimized'",
					() => AzaleaGame.Main.Host.Window.State = WindowState.Minimized),
				createActionButton(
					"Set WindowState to 'Maximized'",
					() => AzaleaGame.Main.Host.Window.State = WindowState.Maximized),
				createActionButton(
					"Set Resizable to 'true'",
					() => AzaleaGame.Main.Host.Window.Resizable = true),
				createActionButton(
					"Set Resizable to 'false'",
					() => AzaleaGame.Main.Host.Window.Resizable = false),
				createActionButton(
					"Set Title to 'Azalea Game'",
					() => AzaleaGame.Main.Host.Window.Title = "Azalea Game"),
				createActionButton(
					"Set Title to 'Ide Gas'",
					() => AzaleaGame.Main.Host.Window.Title = "Ide Gas"),
				createActionButton(
					"Set Title to ''",
					() => AzaleaGame.Main.Host.Window.Title = ""),
			}
		}, new FlexContainer()
		{
			Origin = Anchor.TopRight,
			Anchor = Anchor.TopRight,
			Width = 300,
			Direction = FlexDirection.Vertical,
			Children = new GameObject[]
			{
				createInfoField("WindowState", _windowStateText = new SpriteText()),
				createInfoField("Title", _windowTitleText = new SpriteText()),
				createInfoField("Resizable", _windowResizableText = new SpriteText()),
			}
		}
		});
	}

	private FlexContainer createInfoField(string name, SpriteText valueText)
	{
		return new FlexContainer()
		{
			RelativeSizeAxes = Axes.X,
			Size = new(1, 25),
			Children = new GameObject[]
			{
				new SpriteText()
				{
					Text = $"{name}: "
				},
				valueText
			}
		};
	}

	private BasicButton createActionButton(string text, Action action)
	{
		return new BasicButton()
		{
			Text = text,
			Width = 300,
			Action = action
		};
	}

	private WindowState? _lastState;
	private string? _lastTitle;
	private bool? _lastResizable;

	protected override void Update()
	{
		var window = AzaleaGame.Main.Host.Window;

		if (window.State != _lastState)
		{
			_windowStateText.Text = window.State.ToString();
			Console.WriteLine($"Window state changed to {window.State}");
			_lastState = window.State;
		};

		if (window.Title != _lastTitle)
		{
			_windowTitleText.Text = window.Title;
			Console.WriteLine($"Window state changed to {window.Title}");
			_lastTitle = window.Title;
		}

		if (window.Resizable != _lastResizable)
		{
			_windowResizableText.Text = window.Resizable.ToString();
			Console.WriteLine($"Window resizable changed to {window.Resizable}");
			_lastResizable = window.Resizable;
		}
	}
}
