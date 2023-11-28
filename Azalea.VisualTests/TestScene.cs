using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.VisualTests;

public class TestScene : Composition
{
	public TestScene()
	{
		RelativeSizeAxes = Axes.Both;
	}

	protected FlexContainer CreateFullscreenVerticalFlex(GameObject[] children)
	{
		return new FlexContainer()
		{
			Position = new Vector2(50, 50),
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
			Width = 300,
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
