using Azalea.Design.Shapes;
using Azalea.Graphics.Colors;
using System.Numerics;

namespace Azalea.Graphics.UserInterface;

public class BasicTextBox : TextBox
{
	public BasicTextBox()
	{
		Add(new Box
		{
			RelativeSizeAxes = Axes.Both,
			Depth = 1,
			Color = Palette.Green
		});

		Width = 500;
		Height = 30;
	}

	protected virtual float CaretWidth => 2;

	protected override Caret CreateCaret() => new BasicCaret
	{
		CaretWidth = CaretWidth
	};

	public class BasicCaret : Caret
	{
		private readonly Box _box;

		public BasicCaret()
		{
			RelativeSizeAxes = Axes.Y;
			Size = new Vector2(0, 0.9f);

			Anchor = Anchor.CenterLeft;
			Origin = Anchor.CenterLeft;

			AddInternal(_box = new Box()
			{
				RelativeSizeAxes = Axes.Both,
				Color = Palette.White
			});
		}

		public float CaretWidth { get; set; }

		public override void DisplayAt(Vector2 position, float? selectionWidth)
		{
			if (selectionWidth != null)
			{
				Position = position;
				Width = selectionWidth.Value + (CaretWidth / 2);
				Alpha = 0.5f;
				_box.Alpha = 0.5f;
			}
			else
			{
				Position = new Vector2(position.X - (CaretWidth / 2), position.Y);
				Width = CaretWidth;
				Alpha = 1f;
				_box.Alpha = 1f;
			}
		}
	}
}
