using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using System.Numerics;

namespace Azalea.Editing.Legacy.BindableDisplays;
internal class LegacyVector2Display : LegacyBindableDisplay<Vector2>
{
	private BasicFloatTextBox _Xtextbox;
	private BasicFloatTextBox _Ytextbox;

	public LegacyVector2Display(object obj, string propertyName)
		: base(obj, propertyName)
	{
		AddElement(new FlexContainer()
		{
			Wrapping = FlexWrapping.NoWrapping,
			Spacing = new(5, 0),
			RelativeSizeAxes = Axes.X,
			Size = new(1, 24),
			Children = new GameObject[]
			{
				_Xtextbox = new BasicFloatTextBox()
				{
					RelativeSizeAxes = Axes.Both,
					Size = new(0.49f, 1),
					BackgroundColor = new Color(85, 85, 85),
				},
				_Ytextbox = new BasicFloatTextBox()
				{
					RelativeSizeAxes = Axes.Both,
					Size = new(0.49f, 1),
					BackgroundColor = new Color(85, 85, 85)
				}
			}
		});

		OnValueChanged(CurrentValue);
		if (CurrentValue.X == 0) _Xtextbox.Text = "0";
		if (CurrentValue.Y == 0) _Ytextbox.Text = "0";

		_Xtextbox.TextChanged += _ => SetValue(new Vector2(_Xtextbox.DisplayedFloat, GetValue().Y));
		_Ytextbox.TextChanged += _ => SetValue(new Vector2(GetValue().X, _Ytextbox.DisplayedFloat));
	}

	protected override void OnValueChanged(Vector2 newValue)
	{
		_Xtextbox.DisplayedFloat = newValue.X;
		_Ytextbox.DisplayedFloat = newValue.Y;
	}
}
