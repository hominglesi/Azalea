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
						"Set Opacity to 1",
						() => _window.Opacity = 1),
					createActionButton(
						"Set Opacity to 0.6",
						() => _window.Opacity = 0.6f),
					createActionButton(
						"Set Opacity to 0.3",
						() => _window.Opacity = 0.3f),
					createActionButton(
						"Set Decorated to true",
						() => _window.Decorated = true),
					createActionButton(
						"Set Decorated to false",
						() => _window.Decorated = false),
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

		addObservedValue("WindowState",
			() => _window.State,
			(value) => $"Window state changed to {value}");

		addObservedValue("Title",
			() => _window.Title,
			(value) => $"Window title changed to {value}");

		addObservedValue("Resizable",
			() => _window.Resizable,
			(value) => $"Window resizable changed to {value}");

		addObservedValue("Decorated",
			() => _window.Decorated,
			(value) => $"Window decorated changed to {value}");

		addObservedValue("Prevents Closure",
			() => _preventsClosure,
			(value) => value ? $"Test now prevents closure attempts" : $"Test no longer prevents closure attempts");

		addObservedValue("Position",
			() => _window.Position,
			(value) => $"Window moved to {value}");

		addObservedValue("Size",
			() => _window.ClientSize,
			(value) => $"Window resized to {value}");

		addObservedValue("Opacity",
			() => _window.Opacity,
			(value) => $"Window opacity changed to {value}");

		addObservedValue("_fullscreen",
			() => getField<bool>(_window, "_fullscreen"));

		addObservedValue("_maximized",
			() => getField<bool>(_window, "_maximized"));

		addObservedValue("_minimized",
			() => getField<bool>(_window, "_minimized"));

		addObservedValue("_lastPosition",
			() => getField<Vector2Int>(_window, "_lastPosition"));

		addObservedValue("_lastSize",
			() => getField<Vector2Int>(_window, "_lastSize"));

		addObservedValue("_preMinimizedMaximized",
			() => getField<bool>(_window, "_preMinimizedMaximized"));

		addObservedValue("_preMinimizedFullscreen",
			() => getField<bool>(_window, "_preMinimizedFullscreen"));
	}

	private T getField<T>(object? obj, string name)
	{
		return (T)typeof(GLFWWindow).GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(obj)!;
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

		protected ValueDelegate _getter;
		private LoggerDelegate? _logger;
		private SpriteText _valueText;

		public virtual T Value => _getter.Invoke();

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
				var logMessage = _logger?.Invoke(Value);
				if (logMessage is not null) Console.WriteLine(logMessage);
				_lastValue = Value;
			}
		}

	}
}
