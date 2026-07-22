using Azalea.Design.Containers;
using Azalea.Graphics;

namespace Azalea.Editor.Design.Gui;
public class GUICounter : TextContainer
{
	private string _labelText;
	private int _value;

	internal GUICounter(string text, int initialValue)
		: base(spriteText => spriteText.Font = GUIConstants.Font)
	{
		RelativeSizeAxes = Axes.X;
		AutoSizeAxes = Axes.Y;

		_labelText = text;
		_value = initialValue;

		updateDisplayText();
	}

	public int Value
	{
		get => _value;
		set
		{
			if (value == _value)
				return;

			_value = value;
			updateDisplayText();
		}
	}

	public new string Text
	{
		get => _labelText;
		set
		{
			if (value == _labelText)
				return;

			_labelText = value;
			updateDisplayText();
		}
	}

	private void updateDisplayText() => base.Text = _labelText + _value.ToString();
}
