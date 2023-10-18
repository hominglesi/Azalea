using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.UserInterface;

namespace Azalea.Debugging.BindableDisplays;
public class DebugStringDisplay : DebugBindableDisplay<string>
{
	private TextBox _textbox;

	public DebugStringDisplay(object obj, string propertyName)
		: base(obj, propertyName)
	{
		AddElement(_textbox = new BasicTextBox()
		{
			RelativeSizeAxes = Axes.X,
			Size = new(1, 24),
			BackgroundColor = new Color(85, 85, 85)
		});

		ValueChanged += onValueChanged;
		_textbox.TextChanged += (newText) => SetValue(newText);
	}

	private void onValueChanged(string newValue)
	{
		_textbox.Text = newValue;
	}
}
