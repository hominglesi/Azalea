using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs.Events;
using System.Numerics;

namespace Azalea.Design.UserInterface;

public class BasicButton : Button
{
	public string Text
	{
		get => SpriteText.Text;
		set => SpriteText.Text = value;
	}

	public ColorQuad TextColor
	{
		get => SpriteText.Color;
		set => SpriteText.Color = value;
	}

	public float FontSize
	{
		get => SpriteText.Font.Size;
		set => SpriteText.Font = SpriteText.Font.With(size: value);
	}

	public ColorQuad HoveredColor
	{
		get => HoveredBackground.Color;
		set => HoveredBackground.Color = value;
	}

	protected Box HoveredBackground;
	protected SpriteText SpriteText;

	public BasicButton()
	{
		Size = new Vector2(200, 70);
		BackgroundColor = Palette.Blue;
		AddRange(new GameObject[]
		{
			HoveredBackground = new Box()
			{
				RelativeSizeAxes = Axes.Both,
				Color = Palette.Flowers.Cornflower,
				Alpha = 0,
			},
			SpriteText = new SpriteText
			{
				Font = new("Roboto", weight: "Regular"),
				Color = Palette.Flowers.Azalea,
				Anchor = Anchor.Center,
				Origin = Anchor.Center
			}
		});
	}

	protected override bool OnHover(HoverEvent e)
	{
		HoveredBackground.Alpha = 1;

		return base.OnHover(e);
	}

	protected override void OnHoverLost(HoverLostEvent e)
	{
		HoveredBackground.Alpha = 0;

		base.OnHoverLost(e);
	}
}
