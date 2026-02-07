using Azalea.Amends;
using Azalea.Design.Shapes;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Inputs.Events;

namespace Azalea.Editor.Design;
internal class EditorScrollbar : Slider
{
	private const float _animationSpeed = 0.15f;

	public EditorScrollbar()
	{
		Origin = Anchor.TopRight;
		Anchor = Anchor.TopRight;
		Direction = SliderDirection.Vertical;
		RelativeSizeAxes = Axes.Y;
		Height = 1f;
		Width = 12;
	}

	protected override GameObject CreateBody()
		=> new Box()
		{
			Color = Palette.Gray,
			Alpha = 0,
			RelativeSizeAxes = Axes.Both
		};

	protected override GameObject CreateHead()
		=> new Box()
		{
			Origin = Anchor.Center,
			Color = Palette.White,
			RelativeSizeAxes = Axes.Y,
			Height = 0.2f,
			Width = 4
		};

	protected override bool OnHover(HoverEvent e)
	{
		Body.FinishAmends();
		Body.ChangeAlphaTo(1, _animationSpeed);
		Head.Width = 8;

		return true;
	}

	protected override void OnHoverLost(HoverLostEvent e)
	{
		if (IsHeld) return;

		retract();
	}

	private void retract()
	{
		Body.FinishAmends();
		Body.ChangeAlphaTo(0, _animationSpeed);
		Head.Width = 6;
	}

	protected override void Update()
	{
		base.Update();

		if (Hovered == false && IsHeld == false && Head.Width == 8)
			retract();
	}
}
