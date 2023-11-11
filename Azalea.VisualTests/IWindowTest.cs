using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using Azalea.IO.Assets;
using Azalea.Platform;
using Azalea.Platform.Desktop;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Azalea.VisualTests;
public class IWindowTest : TestScene
{
	private IWindow _window;
	private FlexContainer _observedContainer;

	private bool _preventsClosure;
	private FieldInfo _fullscreenField;

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
						"Set WindowState to 'Fullscreen'",
						() => _window.State = WindowState.Fullscreen),
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
					createActionButton(
						"Set icon to Azalea flower",
						() => _window.SetIconFromStream(Assets.GetTextureStream("azalea-icon.png")!)),
					createActionButton(
						"Set icon to null",
						() => _window.SetIconFromStream(null)),
					createActionButton(
						"Move window by (25, 25)",
						() => _window.Position += new Vector2Int(25, 25)),
					createActionButton(
						"Center window",
						() => _window.Center()),
					createActionButton(
						"Request Attention",
						() => {_window.RequestAttention(); Console.WriteLine("The Window has requested attention"); }),
					createActionButton(
						"Close window",
						() => _window.Close())
				}
			},
			_observedContainer = new FlexContainer()
			{
				Origin = Anchor.TopRight,
				Anchor = Anchor.TopRight,
				Width = 300,
				Direction = FlexDirection.Vertical
			}
		});

		_fullscreenField = typeof(GLFWWindow).GetField("_fullscreen", BindingFlags.NonPublic | BindingFlags.Instance)!;

		addObservedValue("WindowState",
			() => _window.State,
			(value) => $"Window state changed to {value}");

		addObservedValue("Title",
			() => _window.Title,
			(value) => $"Window title changed to {value}");

		addObservedValue("Resizable",
			() => _window.Resizable,
			(value) => $"Window resizable changed to {value}");

		addObservedValue("Prevents Closure",
			() => _preventsClosure,
			(value) => value ? $"Test now prevents closure attempts" : $"Test no longer prevents closure attempts");

		addObservedValue("Position",
			() => _window.Position,
			(value) => $"Window moved to {value}");

		addObservedValue("Size",
			() => _window.ClientSize,
			(value) => $"Window resized to {value}");

		addObservedValue("_fullscreen",
			() => _fullscreenField.GetValue(_window));
	}

	private void onWindowClosing()
	{
		if (_preventsClosure)
		{
			Console.WriteLine("The Window closure attempt was prevented by this Test");
			_window.PreventClosure();
		}
	}

	private void addObservedValue<T>(string name, ObservedValue<T>.ValueDelegate getter, ObservedValue<T>.LoggerDelegate? logger = null)
	{
		_observedContainer.Add(new ObservedValue<T>(name, getter, logger));
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

	private class ObservedValue<T> : FlexContainer
	{
		public delegate T ValueDelegate();
		public delegate string LoggerDelegate(T value);

		private ValueDelegate _getter;
		private LoggerDelegate? _logger;
		private SpriteText _valueText;

		public T Value => _getter.Invoke();

		public ObservedValue(string name, ValueDelegate getter, LoggerDelegate? logger = null)
		{
			_getter = getter;
			_logger = logger;

			RelativeSizeAxes = Axes.X;
			Size = new(1, 25);
			Children = new GameObject[]
			{
				new SpriteText()
				{
					Text = $"{name}: "
				},
				_valueText = new SpriteText()
			};

			_lastValue = Value;
			_valueText.Text = Value.ToString();
		}

		private T _lastValue;

		protected override void Update()
		{
			if (EqualityComparer<T>.Default.Equals(Value, _lastValue) == false)
			{
				_valueText.Text = Value.ToString()!;
				Console.WriteLine(_logger?.Invoke(Value));
				_lastValue = Value;
			}
		}

	}
}
