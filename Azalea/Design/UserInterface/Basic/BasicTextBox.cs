using Azalea.Amends;
using Azalea.Design.Shapes;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs.Events;
using System;
using System.Numerics;

namespace Azalea.Design.UserInterface.Basic;
public class BasicTextBox : TextBox
{
	private Box _carat;

	public BasicTextBox(Action<SpriteText>? defaultCreationParameters = null)
		: base(defaultCreationParameters)
	{
		Size = new(500, 45);
		BorderColor = Palette.Black;

		AddInternal(_carat = new Box()
		{
			Size = new(1, 20),
			Alpha = 0
		});
	}

	protected override void OnFocus(FocusEvent e)
	{
		_carat.Loop(
			x => x.Execute(x => x.Alpha = 1)
			.Then().Wait(0.5f)
			.Then().Execute(x => x.Alpha = 0), 1f);
	}

	protected override void OnFocusLost(FocusLostEvent e)
	{
		_carat.RemoveAmends();
		_carat.Alpha = 0;
	}

	protected override void OnCaratPositionChanged(Vector2 position)
	{
		_carat.Position = position;
	}
}
