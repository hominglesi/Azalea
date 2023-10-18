using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Colors;

namespace Azalea.Debugging.BindableDisplays;
internal class DebugFloatDisplay : DebugBindableDisplay<float>
{
	private BasicFloatTextBox _textbox;

	public DebugFloatDisplay(object obj, string propertyName)
		: base(obj, propertyName)
	{
		AddElement(_textbox = new BasicFloatTextBox()
		{
			RelativeSizeAxes = Axes.X,
			Size = new(1, 24),
			BackgroundColor = new Color(85, 85, 85)
		});

		ValueChanged += onValueChanged;
		_textbox.TextChanged += _ => SetValue(_textbox.DisplayedFloat);
	}

	private void onValueChanged(float newValue)
	{
		_textbox.DisplayedFloat = newValue;
	}
}
