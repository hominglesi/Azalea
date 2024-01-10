using Azalea.Design.Containers;
using Azalea.Graphics.Colors;
using Azalea.Inputs;
using Azalea.Utils;
using System;
using System.Numerics;

namespace Azalea.VisualTests;
public class AutoSizeTest : TestScene
{
	private Composition _composition;

	private FlexContainer _flex;

	public AutoSizeTest()
	{
		Add(_composition = new Composition()
		{
			Position = new(100),
			AutoSizeAxes = Graphics.Axes.Both,
			BorderColor = Palette.Blue,
			Child = _flex = new FlexContainer()
			{
				Direction = FlexDirection.Vertical,
				Width = 200,
				AutoSizeAxes = Graphics.Axes.Y,
				BorderColor = Palette.Green
			}
		});
	}

	private Vector2 _drawSize;

	protected override void Update()
	{
		_flex.Position = Input.MousePosition - new Vector2(100);

		if (_drawSize != _composition.DrawSize)
		{
			Console.WriteLine(_composition.DrawSize);

			_drawSize = _composition.DrawSize;
		}

		if (Input.GetKey(Keys.KeypadPlus).Down)
		{
			_flex.Clear();

			for (int i = 0; i < 5; i++)
			{
				_flex.Add(new TextContainer()
				{
					RelativeSizeAxes = Graphics.Axes.X,
					AutoSizeAxes = Graphics.Axes.Y,
					Text = TextUtils.GenerateLoremIpsum(20)
				});
			}
		}
	}
}
