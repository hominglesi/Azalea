using Azalea.Design.Containers;
using Azalea.Design.Scenes;
using Azalea.Design.UserInterface;
using Azalea.Design.UserInterface.Basic;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Azalea.VisualTests;

public class TestScene : Scene
{
	private const float __sidebarWidth = 400;
	private static readonly Color __accentColor = new(0, 46, 70);
	private static readonly Color __textColor = Palette.Black;
	private const float __textFontSize = 18;
	private static readonly FontUsage __textFont = FontUsage.Default.With(size: __textFontSize);

	protected Composition? Sidebar;

	public TestScene()
	{
		BackgroundColor = Palette.Flowers.Azalea;
	}

	private int _nextSidebarIndex = 0;

	private void initializeSidebar()
	{
		if (Sidebar is not null)
			throw new InvalidOperationException("Sidebar has already been initialized");

		RelativeSizeAxes = Axes.Both;
		NegativeSize = new(__sidebarWidth, 0);
		X = __sidebarWidth;

		Add(Sidebar = new FlexContainer()
		{
			Width = __sidebarWidth,
			RelativeSizeAxes = Axes.Y,
			X = -__sidebarWidth,
			BackgroundColor = __accentColor,
			Direction = FlexDirection.Vertical,
			Spacing = new(0, 10)
		});
	}

	[MemberNotNull(nameof(Sidebar))]
	private void assertDropdownExists()
	{
		if (Sidebar is null)
			initializeSidebar();
	}

	protected BasicDropDownMenu AddDropdownMenu()
	{
		assertDropdownExists();

		const int dropdownHeight = 28;

		var dropdown = new BasicDropDownMenu()
		{
			Size = new(380, dropdownHeight),
			AccentColor = new Color(0, 0, 0, 0),
			ItemHeight = dropdownHeight,
			ItemTextColor = __textColor,
			ItemFont = __textFont,
			DropDownOffset = 5,
			Depth = _nextSidebarIndex++
		};

		dropdown.Label.Font = __textFont;
		dropdown.Label.Color = __textColor;
		dropdown.Arrow.Size = new(16);
		dropdown.Arrow.Color = __accentColor;

		Sidebar.Add(dropdown);
		return dropdown;
	}

	protected BasicButton AddButton(string text)
	{
		assertDropdownExists();

		const int buttonHeight = 28;

		var button = new BasicButton()
		{
			Size = new(380, buttonHeight),
			BackgroundColor = Palette.White,
			Text = text,
			TextColor = __textColor,
			FontSize = __textFontSize,
			Depth = _nextSidebarIndex++
		};

		Sidebar.Add(button);
		return button;
	}

	protected FlexContainer CreateFullscreenVerticalFlex(GameObject[] children)
	{
		return new FlexContainer()
		{
			RelativeSizeAxes = Axes.Both,
			Direction = FlexDirection.Vertical,
			Spacing = new(5),
			Wrapping = FlexWrapping.Wrap,
			Children = children
		};
	}

	protected BasicButton CreateActionButton(string text, Action action)
	{
		return new BasicButton()
		{
			Text = text,
			Width = 400,
			Action = action
		};
	}

	protected FlexContainer CreateObservedContainer(GameObject[] children)
	{
		return new FlexContainer()
		{
			Origin = Anchor.TopRight,
			Anchor = Anchor.TopRight,
			Width = 300,
			Direction = FlexDirection.Vertical,
			Children = children
		};
	}

	protected FlexContainer CreateObservedValue<T>(string name, ObservedValue<T>.ValueDelegate getter, ObservedValue<T>.LoggerDelegate? logger = null)
	{
		return new ObservedValue<T>(name, getter, logger);
	}

	protected class ObservedValue<T> : FlexContainer
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
