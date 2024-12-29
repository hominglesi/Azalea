using Azalea.Design.Containers;
using Azalea.Graphics.Colors;
using Azalea.Inputs;
using Azalea.Utils;
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
			BorderThickness = 2,
			Child = _flex = new FlexContainer()
			{
				Direction = FlexDirection.Vertical,
				Size = new(200),
				AutoSizeAxes = Graphics.Axes.Y,
				BorderColor = Palette.Green,
				BorderThickness = 2,
			}
		});

		regenerateText();
	}

	protected override void Update()
	{
		_flex.Position = Input.MousePosition - new Vector2(100);

		if (Input.GetKey(Keys.Space).DownOrRepeat)
			regenerateText();
	}

	private void regenerateText()
	{
		_flex.Clear();

		for (int i = 0; i < 2; i++)
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
