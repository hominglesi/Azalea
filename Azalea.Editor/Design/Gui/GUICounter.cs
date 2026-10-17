using Azalea.Design.Containers;
using Azalea.Graphics;

namespace Azalea.Editor.Design.Gui;

public class GUICounter : TextContainer
{
	private string _labelText;
	private int _lastValue;
	private int _value;

	internal GUICounter(string text, int initialValue)
		: base(spriteText => spriteText.Font = GUIConstants.Font)
	{
		RelativeSizeAxes = Axes.X;
		AutoSizeAxes = Axes.Y;

		_labelText = text;
		_value = _lastValue = initialValue;
	}

	public int Value { get; set; }

	protected override void Update()
	{
		if (_lastValue != Value)
		{
			Text = _labelText + _value.ToString();
			_lastValue = Value;
		}
	}
}
