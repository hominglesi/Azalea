using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using System.Numerics;

namespace Azalea.Editing;
internal class DisplayValues : FlexContainer
{
	internal DisplayValues()
	{
		Origin = Anchor.BottomLeft;
		Anchor = Anchor.BottomLeft;
		Direction = FlexDirection.VerticalReverse;
		RelativeSizeAxes = Axes.Both;
		Wrapping = FlexWrapping.Wrap;
		Position = new(5, -5);
		Spacing = new(5);
		Depth = -1000;
	}

	public void AddDisplayedValue(string name, DisplayedValueDelegate getValue)
		=> Add(new DisplayedValue(name, getValue));

	public void ClearDisplayedValues()
		=> Clear();

	public void RemoveDisplayedValue(int index)
		=> Remove(Children[index]);

	private class DisplayedValue : Composition
	{
		public SpriteText Text { get; init; }

		private readonly string _name;
		private readonly DisplayedValueDelegate _getValue;

		public DisplayedValue(string name, DisplayedValueDelegate getValue)
		{
			_name = name;
			_getValue = getValue;

			Add(new Box()
			{
				RelativeSizeAxes = Axes.Both,
				Color = Palette.Black,
				Alpha = 0.7f
			});

			Add(Text = new SpriteText()
			{
				Origin = Anchor.Center,
				Anchor = Anchor.Center,
			});
		}

		protected override void Update()
		{
			Text.Text = $"{_name}: {_getValue.Invoke()}";
			Size = Text.Size + new Vector2(4);
		}
	}
	public delegate object DisplayedValueDelegate();
}
