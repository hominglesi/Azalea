using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using Azalea.Platform;
using System;

namespace Azalea.VisualTests;
public class IWindowTest : TestScene
{
	private IWindow _window;

	private SpriteText _windowStateText;
	private SpriteText _windowTitleText;
	private SpriteText _windowResizableText;
	private SpriteText _windowPreventsClosureText;

	private bool _preventsClosure;

	public IWindowTest()
	{
		_window = AzaleaGame.Main.Host.Window;
		_window.Closing += onWindowClosing;

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
						() => _window.State = WindowState.Normal),
					createActionButton(
						"Set WindowState to 'Minimized'",
						() => _window.State = WindowState.Minimized),
					createActionButton(
						"Set WindowState to 'Maximized'",
						() => _window.State = WindowState.Maximized),
					createActionButton(
						"Set Resizable to 'true'",
						() => _window.Resizable = true),
					createActionButton(
						"Set Resizable to 'false'",
						() => _window.Resizable = false),
					createActionButton(
						"Set Title to 'Azalea Game'",
						() => _window.Title = "Azalea Game"),
					createActionButton(
						"Set Title to 'Ide Gas'",
						() => _window.Title = "Ide Gas"),
					createActionButton(
						"Set Title to ''",
						() => _window.Title = ""),
					createActionButton(
						"Attempt to Close Window",
						() => _window.Close()),
					createActionButton(
						"Set this test to prevent Closing",
						() => _preventsClosure = true),
					createActionButton(
						"Set this test to not prevent Closing",
						() => _preventsClosure = false),
				}
			},
			new FlexContainer()
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
					createInfoField("Prevents Closure", _windowPreventsClosureText = new SpriteText()),
				}
			}
		});

		_lastState = _window.State;
		_lastTitle = _window.Title;
		_lastResizable = _window.Resizable;
		_lastPreventsClosure = _preventsClosure;
	}

	private void onWindowClosing()
	{
		if (_preventsClosure)
		{
			Console.WriteLine("The Window closure attempt was prevented by this Test");
			_window.PreventClosure();
		}
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

	private WindowState _lastState;
	private string _lastTitle;
	private bool _lastResizable;
	private bool _lastPreventsClosure;

	protected override void Update()
	{
		if (_window.State != _lastState)
		{
			_windowStateText.Text = _window.State.ToString();
			Console.WriteLine($"Window state changed to {_window.State}");
			_lastState = _window.State;
		};

		if (_window.Title != _lastTitle)
		{
			_windowTitleText.Text = _window.Title;
			Console.WriteLine($"Window title changed to {_window.Title}");
			_lastTitle = _window.Title;
		}

		if (_window.Resizable != _lastResizable)
		{
			_windowResizableText.Text = _window.Resizable.ToString();
			Console.WriteLine($"Window resizable changed to {_window.Resizable}");
			_lastResizable = _window.Resizable;
		}

		if (_preventsClosure != _lastPreventsClosure)
		{
			_windowPreventsClosureText.Text = _preventsClosure.ToString();
			if (_preventsClosure)
				Console.WriteLine($"Test now prevents closure attempts");
			else
				Console.WriteLine($"Test no longer prevents closure attempts");
			_lastPreventsClosure = _preventsClosure;
		}
	}
}
