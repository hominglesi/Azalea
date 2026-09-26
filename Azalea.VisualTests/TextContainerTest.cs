using Azalea.Design.Containers;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using Azalea.Platform;
using Azalea.Utils;
using Azalea.Platform.Windowing;

namespace Azalea.VisualTests;
public class TextContainerTest : TestScene
{
	private ScrollableContainer _scrollable;
	private FlexContainer _flex;

	public TextContainerTest()
	{
		Add(_scrollable = new ScrollableContainer()
		{
			BorderColor = Palette.Teal,
			Size = new(800, 600),
			Position = new(50, 50)
			//RelativeSizeAxes = Graphics.Axes.Both,
		});

		_scrollable.Add(_flex = new FlexContainer()
		{
			AutoSizeAxes = Graphics.Axes.Y,
			RelativeSizeAxes = Graphics.Axes.X,
			Direction = FlexDirection.Vertical,
			Spacing = new(0, 50)
		});

		for (int i = 0; i < 6; i++)
		{
			var container = new TextContainer((t) => { t.Font = FontUsage.Default.With(size: 24); })
			{
				AutoSizeAxes = Graphics.Axes.Y,
				RelativeSizeAxes = Graphics.Axes.X,
				Width = 0.6f,
				Origin = i % 2 == 0 ? Graphics.Anchor.TopLeft : Graphics.Anchor.TopRight,
				Anchor = i % 2 == 0 ? Graphics.Anchor.TopLeft : Graphics.Anchor.TopRight,
				Justification = i % 2 == 0 ? FlexJustification.Start : FlexJustification.End,
			};

			for (int j = 0; j < 3; j++)
			{
				container.AddText(TextUtils.GenerateLoremIpsum(Rng.Int(10, 40)));
				if (Rng.Int(2) == 0)
					container.AddText("\n");
				if (Rng.Int(2) == 0)
					container.AddText("\n");
			}


			_flex.Add(container);
		}

	}

	protected override void Initialize()
	{
		App.Window.SetClientSize(new(1680, 960));
		App.Window.Center();
	}

	protected override bool OnKeyDown(KeyDownEvent e)
	{
		if(e.Key == Keys.Space)
		{
			_scrollable.Size = _scrollable.ToLocalSpace(e.State.MousePosition);
			return true;
		}

		return false;
	}
}
