using Azalea.Graphics;
using Azalea.Graphics.UserInterface;

namespace Azalea.Debugging.BindableDisplays;
public class DebugStringDisplay : DebugBindableDisplay<string>
{
	private TextBox _textbox;

	public DebugStringDisplay(object obj, string propertyName)
		: base(obj, propertyName)
	{
		var currentValue = GetValue();

		Add(_textbox = new BasicTextBox()
		{
			RelativeSizeAxes = Axes.Both,
			Size = new(1, 1),
		});

		ValueChanged += onValueChanged;
		_textbox.TextChanged += (newText) => SetValue(newText);
	}

	private void onValueChanged(string newValue)
	{
		_textbox.Text = newValue;
	}
}
