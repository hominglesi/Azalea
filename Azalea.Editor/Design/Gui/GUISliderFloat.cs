using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using System;

namespace Azalea.Editor.Design.Gui;
public class GUISliderFloat : Composition
{
	private readonly float _minValue;
	private readonly float _maxValue;
	private readonly string _stringFormat;

	private readonly Slider _slider;
	private readonly SpriteText _valueText;

	internal GUISliderFloat(string name, float minValue, float maxValue,
		float initialValue, string stringFormat, bool continuous)
	{
		_minValue = minValue;
		_maxValue = maxValue;
		_stringFormat = stringFormat;

		RelativeSizeAxes = Axes.X;
		AutoSizeAxes = Axes.Y;

		AddRange([
			_slider = new GUISlider(){
				Width = 220,
				Height = 19
			},
			new SpriteText(){
				Position = new(225, 9),
				Origin = Anchor.CenterLeft,
				Text = name,
				Font = GUIConstants.Font
			},
			_valueText = new SpriteText(){
				Position = new(110, 9),
				Origin = Anchor.Center,
				Text = "0.000",
				Font = GUIConstants.Font
			}
		]);

		if (continuous)
			_slider.OnValueChanged = onValueChanged;
		else
			_slider.OnValueSet = onValueChanged;

		_slider.Value = initialValue / (_maxValue - _minValue);
	}

	private void onValueChanged(float value)
	{
		var boundValue = _minValue + (value * (_maxValue - _minValue));

		_valueText.Text = boundValue.ToString(_stringFormat);
		_valueChanged?.Invoke(boundValue);
	}

	private Action<float>? _valueChanged;
	public void OnValueChanged(Action<float> valueChanged)
		=> _valueChanged = valueChanged;

	class GUISlider : Slider
	{
		protected override GameObject CreateBody()
		{
			return new Box()
			{
				RelativeSizeAxes = Axes.Both,
				Color = GUIConstants.Colors.AccentColor
			};
		}

		protected override GameObject CreateHead()
		{
			return new Composition()
			{
				Size = new(16, 19),
				Origin = Anchor.Center,
				Child = new Box()
				{
					Origin = Anchor.Center,
					Anchor = Anchor.Center,
					Size = new(12, 15),
					Color = GUIConstants.Colors.AccentColor2
				}
			};
		}
	}
}
