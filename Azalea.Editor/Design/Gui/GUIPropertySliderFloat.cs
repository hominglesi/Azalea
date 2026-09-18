using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using Azalea.Utils;
using Azalea.Utils.Proxies;

namespace Azalea.Editor.Design.Gui;
public class GUIPropertySliderFloat : Composition
{
	private readonly IProxy<float> _proxy;
	private readonly float _minValue;
	private readonly float _maxValue;
	private readonly string _stringFormat;

	public Slider Slider { get; }
	private readonly SpriteText _valueText;

	internal GUIPropertySliderFloat(string name, IProxy<float> proxy,
		float minValue, float maxValue, string stringFormat)
	{
		_proxy = proxy;
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
		Slider.OnValueSet += value => _proxy.SetValue(toValueRange(value));
	}

	protected override void Update()
	{
		if(_proxy.HasNewValue(out float newValue))
			Slider.Value = toSliderRange(newValue);
	}

	private float toValueRange(float value) => _minValue + (value * (_maxValue - _minValue));
	private float toSliderRange(float value) => (value - _minValue) / (_maxValue - _minValue);
}
