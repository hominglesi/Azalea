using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs.Events;
using System;

namespace Azalea.VisualTests.UnitTesting;
public class UnitTestMenuBar : FlexContainer
{
	private readonly static Color __backgroundColor = new(0, 27, 41);
	private readonly static Color __hoveredColor = new(70);
	private readonly static Color __hoveredBorderColor = new(170);
	private readonly static Color __textColor = new(230);

	public UnitTestMenuBar()
	{
		BackgroundColor = __backgroundColor;
		Direction = FlexDirection.Horizontal;
		Alignment = FlexAlignment.Center;

		Add(new Box()
		{
			Color = __backgroundColor,
			RelativeSizeAxes = Axes.Y,
			Size = new(10, 1)
		});
	}

	public void AddMenuButton(string name, Action action)
	{
		Add(new MenuBarButton(name)
		{
			Action = action
		});
	}

	private class MenuBarButton : BasicButton
	{
		public MenuBarButton(string name)
		{
			Text = name;
			SpriteText.Font = SpriteText.Font.With(size: 20);
			TextColor = __textColor;
			RelativeSizeAxes = Axes.Y;
			Size = new(SpriteText.Width + 20, 0.7f);
			BackgroundColor = __backgroundColor;
			HoveredBackground.Color = __hoveredColor;
			BorderColor = __hoveredBorderColor;
			BorderThickness = 1;
			BorderObject!.OutsideContent = false;
			BorderObject!.Alpha = 0;
		}

		protected override bool OnHover(HoverEvent e)
		{
			BorderObject!.Alpha = 1;

			return base.OnHover(e);
		}

		protected override void OnHoverLost(HoverLostEvent e)
		{
			BorderObject!.Alpha = 0;

			base.OnHoverLost(e);
		}
	}
}
