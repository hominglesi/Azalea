using Azalea.Design.Containers;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Utils;

namespace Azalea.VisualTests;
public class TextContainerTest : TestScene
{
	private ScrollableContainer _scrollable;
	private FlexContainer _flex;

	public TextContainerTest()
	{
		Add(_scrollable = new ScrollableContainer()
		{
			RelativeSizeAxes = Graphics.Axes.Both,
		});

		_scrollable.Add(_flex = new FlexContainer()
		{
			AutoSizeAxes = Graphics.Axes.Y,
			RelativeSizeAxes = Graphics.Axes.X,
			InternalRelativeSizeAxes = Graphics.Axes.X,
			Direction = FlexDirection.Vertical,
			Spacing = new(0, 50),
			BorderColor = Palette.Brown
		});

		for (int i = 0; i < 6; i++)
		{
			var container = new TextContainer((t) => { t.Font = FontUsage.Default.With(size: 20); })
			{
				AutoSizeAxes = Graphics.Axes.Y,
				RelativeSizeAxes = Graphics.Axes.X,
				InternalRelativeSizeAxes = Graphics.Axes.X,
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

	protected override void UpdateAfterChildren()
	{
		base.UpdateAfterChildren();

		var maxChildOffset = 0f;

		/*
		foreach (var child in _flex.Children)
		{
			maxChildOffset = MathF.Max(maxChildOffset, child.Y + child.Height);
		}*/

		_scrollable.ContentBoundaries = new Graphics.Boundary(0, 0, _flex.Height, 0);
	}
}
