using Azalea.Design.Containers;
using Azalea.Graphics;

namespace Azalea.Editor.Design.Gui;

public class GUICounter : TextContainer
{
	private readonly string _labelText;
	private int _lastValue;

	internal GUICounter(string text, int initialValue)
		: base(spriteText => spriteText.Font = GUIConstants.Font)
	{
		RelativeSizeAxes = Axes.X;
		AutoSizeAxes = Axes.Y;

		_labelText = text;
		Value = _lastValue = initialValue;
		Text = _labelText + Value.ToString();
	}

	public int Value { get; set; }

	protected override void Update()
	{
		if (_lastValue != Value)
		{
			Text = _labelText + Value.ToString();
			_lastValue = Value;
		}
	}
}
