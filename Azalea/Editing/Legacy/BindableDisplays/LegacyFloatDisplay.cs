using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Colors;

namespace Azalea.Editing.Legacy.BindableDisplays;
internal class LegacyFloatDisplay : LegacyBindableDisplay<float>
{
	private BasicFloatTextBox _textbox;

	public LegacyFloatDisplay(object obj, string propertyName)
		: base(obj, propertyName)
	{
		AddElement(_textbox = new BasicFloatTextBox()
		{
			RelativeSizeAxes = Axes.X,
			Size = new(1, 24),
			BackgroundColor = new Color(85, 85, 85)
		});

		if (CurrentValue != 0) OnValueChanged(CurrentValue);
		else _textbox.Text = "0";

		_textbox.TextChanged += _ => SetValue(_textbox.DisplayedFloat);
	}

	protected override void OnValueChanged(float newValue)
	{
		_textbox.DisplayedFloat = newValue;
	}
}
