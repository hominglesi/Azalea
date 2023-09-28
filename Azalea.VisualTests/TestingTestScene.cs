using Azalea.Design.Compositions;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Containers;
using Azalea.Graphics.Sprites;
using Azalea.Graphics.UserInterface;
using Azalea.Inputs;
using System;
using System.Numerics;

namespace Azalea.VisualTests;

public class TestingTestScene : TestScene
{
	private TextContainer _container;
	private BasicTextBox _text;

	public TestingTestScene()
	{
		Add(_container = new TextContainer(t => { t.Font = t.Font.With(size: 40); })
		{
			Size = new Vector2(400, 400),
			LineSpacing = 1f,
			Text = "Lorem ipsum dolor sit amet,\n consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum",
			//BackgroundColor = ColorInfo.SolidColor(Palette.Black),
			BorderColor = ColorInfo.SolidColor(Palette.Black),
		});

		_container.AddText("Text3 ", t => { t.Color = Palette.Black; });
		_container.AddText("Text4 ");

		Add(_text = new BasicTextBox()
		{
			Width = 500,
			Height = 30,
			Position = new Vector2(500, 500)
		});
		_text.Text = "Ide Gas";

		Add(new BasicTextBox()
		{
			Width = 500,
			Height = 30,
			Position = new Vector2(550, 300)
		});

		Add(new Box()
		{
			Size = new(100, 100),
			Origin = Anchor.BottomRight,
			Color = Palette.Blue,
			Anchor = Anchor.BottomRight
		});
		var modifiedColor = Palette.Blue;
		modifiedColor.Luminance += 0.3f;
		Add(new Box()
		{
			Size = new(100, 100),
			Origin = Anchor.TopRight,
			Color = modifiedColor,
			Anchor = Anchor.TopRight
		});

		Add(new Composition()
		{
			Size = new(200, 200),
			Position = new(500, 50),
			Alpha = 0.5f,
			BackgroundColor = ColorInfo.GradientHorizontal(Palette.Blue, Palette.Green)
		});

		BasicWindow window;
		Add(window = new BasicWindow()
		{
			Position = new(300, 300)
		});

		window.Add(new SpriteText()
		{
			Color = Palette.Black,
			Text = "JA sam U windowu"
		});
	}

	protected override void Update()
	{
		_container.Size = Input.MousePosition;

		if (Input.GetKey(Keys.P).Down && _container.Children.Count > 0) _container.Remove(_container.Children[0]);

		if (Input.GetKey(Keys.Space).DownOrRepeat) Console.WriteLine("Pressed space");
	}
}
