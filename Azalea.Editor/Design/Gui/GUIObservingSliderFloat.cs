using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using Azalea.Utils;

namespace Azalea.Editor.Design.Gui;
public class GUIObservingSliderFloat : Composition
{
	private readonly ObservableProxy<float> _observable;
	private readonly float _minValue;
	private readonly float _maxValue;
	private readonly string _stringFormat;

	public Slider Slider { get; }
	private readonly SpriteText _valueText;

	internal GUIObservingSliderFloat(string name, ObservableProxy<float> observable,
		float minValue, float maxValue, string stringFormat)
	{
		_observable = observable;
		_minValue = minValue;
		_maxValue = maxValue;
		_stringFormat = stringFormat;

		RelativeSizeAxes = Axes.X;
		AutoSizeAxes = Axes.Y;

		AddRange([
			Slider = new GUISliderFloat.GUISlider(){
				Width = 220,
				Height = GUIConstants.ElementHeight
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

		Slider.OnValueChanged += value => _valueText.Text = toValueRange(value).ToString(_stringFormat);
		Slider.OnValueSet += value => _observable.SetValue(toValueRange(value));
	}

	protected override void Update()
	{
		if(_observable.TryGetInvalid(out float newValue))
			Slider.Value = toSliderRange(newValue);
	}

	private float toValueRange(float value) => _minValue + (value * (_maxValue - _minValue));
	private float toSliderRange(float value) => (value - _minValue) / (_maxValue - _minValue);
}
