using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Colors;

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

		OnValueChanged(CurrentValue);
		_textbox.TextChanged += (newText) => SetValue(newText);
	}

	protected override void OnValueChanged(string newValue)
	{
		_textbox.Text = newValue;
	}
}
