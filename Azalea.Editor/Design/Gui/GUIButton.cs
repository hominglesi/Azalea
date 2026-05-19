using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using Azalea.Inputs.Events;
using System;

namespace Azalea.Editor.Design.Gui;
public class GUIButton : Composition
{
	private readonly SpriteText _text;
	private readonly Sprite _background;

	internal GUIButton(string text, Action clickAction)
	{
		ClickAction = _ => clickAction();
		Height = GUIConstants.ElementHeight;
		AutoSizeAxes = Axes.X;
		NegativeSize = new(-10, 0);
		AddRange([
			_background = new Box() {
				IgnoredForAutoSizeAxes = Axes.Both,
				RelativeSizeAxes = Axes.Both,
				Color = GUIConstants.Colors.AccentColor,
			},
			_text = new SpriteText(){
				Font = GUIConstants.Font,
				Origin = Anchor.Center,
				Anchor = Anchor.Center,
				Text = text,
			}
			]);
	}

	protected override bool OnHover(HoverEvent e)
	{
		_background.Color = GUIConstants.Colors.AccentColor2;
		return true;
	}

	protected override void OnHoverLost(HoverLostEvent e)
	{
		_background.Color = GUIConstants.Colors.AccentColor;
	}
}
