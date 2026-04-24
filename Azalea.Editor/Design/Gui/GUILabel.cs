using Azalea.Design.Containers;
using Azalea.Graphics;
using System;

namespace Azalea.Editor.Design.Gui;
public class GUILabel : TextContainer
{
	internal GUILabel(string text)
		: base(spriteText => spriteText.Font = GUIConstants.Font)
	{
		RelativeSizeAxes = Axes.X;
		AutoSizeAxes = Axes.Y;

		Text = text;
	}
}

public class GUILabelContinuous : TextContainer
{
	private readonly Func<string> _textFunction;
	private string _text;

	internal GUILabelContinuous(Func<string> text)
		: base(spriteText => spriteText.Font = GUIConstants.Font)
	{
		_textFunction = text;

		RelativeSizeAxes = Axes.X;
		AutoSizeAxes = Axes.Y;

		Text = _text = text.Invoke();
	}

	protected override void Update()
	{
		var text = _textFunction.Invoke();

		if (text == _text)
			return;

		Text = _text = text;
	}
}
