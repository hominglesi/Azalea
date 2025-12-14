using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.IO.Resources;

namespace Azalea.VisualTests.NearDistanceField;
public class NearDistanceFieldTests : TestScene
{
	public NearDistanceFieldTests()
	{
		Assets.MainStore.AddMsdfFont("Roboto-Regular",
			"Fonts/Roboto-Regular.csv", $"Fonts/Roboto-Regular.bmp");

		Assets.MainStore.AddMsdfFont("TitanOne-Regular",
			"Fonts/TitanOne-Regular.csv", $"Fonts/TitanOne-Regular.bmp");

		Add(new Box()
		{
			Size = new(423, 106),
			Color = Palette.Red,
			Origin = Anchor.CenterLeft,
			Anchor = Anchor.CenterLeft,
			Alpha = 0.3f
		});
		Add(new SpriteText()
		{
			Text = "Ide Gas ;!'",
			Font = FontUsage.Default.With(size: 100),
			Origin = Anchor.CenterLeft,
			Anchor = Anchor.CenterLeft
		});
		Add(new SpriteText()
		{
			Text = "e as ;",
			Font = FontUsage.Default.With(size: 100),
			Origin = Anchor.CenterLeft,
			Anchor = Anchor.CenterLeft,
			Y = 200
		});
		Add(new Box()
		{
			Color = Palette.Red,
			Position = new(355, 106),
			Size = new(4)
		});
		/*

		Add(new SpriteText()
		{
			Text = "Haters will say it's fake",
			Font = FontUsage.Default.With(size: 20),
			Position = new(150, 300)
		});

		Add(new SpriteText()
		{
			Text = "Ruan Mei",
			Font = FontUsage.Default.With(size: 200),
			Position = new(650, 180)
		});

		Add(new SpriteText()
		{
			Text = "Ruan Mei",
			Font = FontUsage.Default.With(size: 200),
			Position = new(650, 350)
		});

		Add(new SpriteText()
		{
			Text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
			Font = FontUsage.Default.With(size: 60),
			Position = new(0, 480)
		});

		Add(new SpriteText()
		{
			Text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower(),
			Font = FontUsage.Default.With(size: 60),
			Position = new(0, 540)
		});

		Add(new SpriteText()
		{
			Text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
			Font = FontUsage.Default.With(size: 200),
			Position = new(0, 700)
		});

		Add(new SpriteText()
		{
			Text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower(),
			Font = FontUsage.Default.With(size: 200),
			Position = new(0, 875)
		});*/
	}
}
