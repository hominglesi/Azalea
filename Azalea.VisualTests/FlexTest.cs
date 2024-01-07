using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Extentions.EnumExtentions;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.Utils;
using System.Numerics;

namespace Azalea.VisualTests;
public class FlexTest : TestScene
{
	private FlexContainer _flex;

	public FlexTest()
	{
		Add(_flex = new FlexContainer()
		{
			RelativeSizeAxes = Axes.Both,

		});
	}

	private GameObject createBox(Vector2 size, int number)
	{
		var comp = new Composition()
		{
			Size = size
		};
		var color = Rng.Color();
		comp.Add(new Box()
		{
			RelativeSizeAxes = Axes.Both,
			Color = color
		});

		comp.Add(new SpriteText()
		{
			Origin = Anchor.Center,
			Anchor = Anchor.Center,
			Text = number.ToString(),
			Color = color.Brightness > 127 ? Palette.Black : Palette.White,
			Font = FontUsage.Default.With(size: 28)
		});
		return comp;
	}

	protected override void Update()
	{
		if (Input.GetKey(Keys.Space).DownOrRepeat)
		{
			var size = Input.GetKey(Keys.ShiftLeft).Pressed ? Rng.Int(50, 150) : 100;
			_flex.Add(createBox(new(size), _flex.Children.Count));
		}

		if (Input.GetKey(Keys.D).Down)
			_flex.Direction = _flex.Direction.NextValue();

		if (Input.GetKey(Keys.J).Down)
			_flex.Justification = _flex.Justification.NextValue();

		if (Input.GetKey(Keys.A).Down)
			_flex.Alignment = _flex.Alignment.NextValue();

		if (Input.GetKey(Keys.KeypadPlus).DownOrRepeat)
			_flex.Spacing += Vector2.One;

		if (Input.GetKey(Keys.KeypadMinus).DownOrRepeat && _flex.Spacing.X > 0)
			_flex.Spacing -= Vector2.One;

		if (Input.GetKey(Keys.Delete).Down)
			_flex.Clear();

		if (Input.GetKey(Keys.Enter).Down)
			_flex.AddNewLine();

		if (Input.GetKey(Keys.Backspace).DownOrRepeat)
			_flex.Remove(_flex.Children[_flex.Children.Count - 1]);
	}
}
