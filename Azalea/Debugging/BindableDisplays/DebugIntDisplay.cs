using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Colors;

namespace Azalea.Debugging.BindableDisplays;
internal class DebugIntDisplay : DebugBindableDisplay<int>
{
	private BasicIntTextBox _textbox;

	public DebugIntDisplay(object obj, string propertyName)
		: base(obj, propertyName)
	{
		AddElement(_textbox = new BasicIntTextBox()
		{
			RelativeSizeAxes = Axes.X,
			Size = new(1, 24),
			BackgroundColor = new Color(85, 85, 85)
		});

		if (CurrentValue != 0) OnValueChanged(CurrentValue);
		else _textbox.Text = "0";

		_textbox.TextChanged += _ => SetValue(_textbox.DisplayedInt);
	}

	protected override void OnValueChanged(int newValue)
	{
		_textbox.DisplayedInt = newValue;
	}
}
