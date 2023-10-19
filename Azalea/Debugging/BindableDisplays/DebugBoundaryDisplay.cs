using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Colors;

namespace Azalea.Debugging.BindableDisplays;
public class DebugBoundaryDisplay : DebugBindableDisplay<Boundary>
{
	private BasicFloatTextBox _TopTextbox;
	private BasicFloatTextBox _RightTextbox;
	private BasicFloatTextBox _BottomTextbox;
	private BasicFloatTextBox _LeftTextbox;

	public DebugBoundaryDisplay(object obj, string propertyName)
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
				_TopTextbox = new BasicFloatTextBox()
				{
					RelativeSizeAxes = Axes.Both,
					Size = new(0.24f, 1),
					BackgroundColor = new Color(85, 85, 85),
				},
				_RightTextbox = new BasicFloatTextBox()
				{
					RelativeSizeAxes = Axes.Both,
					Size = new(0.24f, 1),
					BackgroundColor = new Color(85, 85, 85)
				},
				_BottomTextbox = new BasicFloatTextBox()
				{
					RelativeSizeAxes = Axes.Both,
					Size = new(0.24f, 1),
					BackgroundColor = new Color(85, 85, 85)
				},
				_LeftTextbox = new BasicFloatTextBox()
				{
					RelativeSizeAxes = Axes.Both,
					Size = new(0.24f, 1),
					BackgroundColor = new Color(85, 85, 85)
				}
			}
		});

		OnValueChanged(CurrentValue);
		if (CurrentValue.Top == 0) _TopTextbox.Text = "0";
		if (CurrentValue.Right == 0) _RightTextbox.Text = "0";
		if (CurrentValue.Bottom == 0) _BottomTextbox.Text = "0";
		if (CurrentValue.Left == 0) _LeftTextbox.Text = "0";


		_TopTextbox.TextChanged += _ => ChangeBoundary(top: _TopTextbox.DisplayedFloat);
		_RightTextbox.TextChanged += _ => ChangeBoundary(right: _RightTextbox.DisplayedFloat);
		_BottomTextbox.TextChanged += _ => ChangeBoundary(bottom: _BottomTextbox.DisplayedFloat);
		_LeftTextbox.TextChanged += _ => ChangeBoundary(left: _LeftTextbox.DisplayedFloat);

	}

	private void ChangeBoundary(float? top = null, float? right = null, float? bottom = null, float? left = null)
	{
		var newValue = CurrentValue;
		if (top is not null) newValue.Top = top.Value;
		if (right is not null) newValue.Right = right.Value;
		if (bottom is not null) newValue.Bottom = bottom.Value;
		if (left is not null) newValue.Left = left.Value;
		SetValue(newValue);
	}

	protected override void OnValueChanged(Boundary newValue)
	{
		_TopTextbox.DisplayedFloat = newValue.Top;
		_RightTextbox.DisplayedFloat = newValue.Right;
		_BottomTextbox.DisplayedFloat = newValue.Bottom;
		_LeftTextbox.DisplayedFloat = newValue.Left;
	}
}
