using Azalea.Caching;
using Azalea.Design.Containers;
using Azalea.Graphics;

namespace Azalea.Editor.Design.Gui;
public class GUICounter : TextContainer
{
	private string _labelText;
	private int _value;
	private readonly Cached _displayedValue = new();

	internal GUICounter(string text, int initialValue)
		: base(spriteText => spriteText.Font = GUIConstants.Font)
	{
		RelativeSizeAxes = Axes.X;
		AutoSizeAxes = Axes.Y;

		_labelText = text;
		_value = initialValue;
	}

	public int Value
	{
		get => _value;
		set
		{
			if (value == _value)
				return;

			_value = value;
			_displayedValue.Invalidate();
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
			_displayedValue.Invalidate();
		}
	}

	protected override void Update()
	{
		if (_displayedValue.IsValid == false)
		{
			base.Text = _labelText + _value.ToString();
			_displayedValue.Validate();
		}
	}
}
