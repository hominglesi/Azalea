using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Design.UserInterface;
using Azalea.Design.UserInterface.Basic;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using System;

namespace Azalea.VisualTests;

public class DefaultUserInputTest : TestScene
{
	private Composition _buttonComposition;
	private Composition _childButtonComposition;
	private BasicTextBox _textBox;

	public DefaultUserInputTest()
	{
		Add(_buttonComposition = new Composition()
		{
			RelativeSizeAxes = Axes.Both,
			Size = new(0.48f, 0.48f),
			Children = new GameObject[]
			{
				new Box()
				{
					Color = Palette.Green,
					RelativeSizeAxes = Axes.Both
				},
				new BasicButton()
				{
					Text = "Button 1",
					Action = () => { Write("Button 1 clicked"); }
				},
				new BasicButton()
				{
					Text = "Button 2",
					Position = new(1f, 0f),
					RelativePositionAxes = Axes.Both,
					Origin = Anchor.TopRight,
					Action = () => { Write("Button 2 clicked");},
					Size = new(120, 30)
				},
				new BasicButton()
				{
					Text = "Button 3",
					Position = new(0f, 1f),
					RelativePositionAxes = Axes.Both,
					Origin = Anchor.BottomLeft,
					Action = () => { Write("Button 3 clicked");},
					Size = new(80, 200)
				},
				_childButtonComposition = new Composition()
				{
					Position = new(1f, 1f),
					RelativePositionAxes = Axes.Both,
					Origin = Anchor.BottomRight,
					Size = new(150, 150),
					Children = new GameObject[]
					{
						new Box()
						{
							RelativeSizeAxes = Axes.Both,
							Color = Palette.White,
							Origin = Anchor.Center,
							Anchor = Anchor.Center
						},
						new BasicButton()
						{
							Text = "Button 4",
							Origin = Anchor.Center,
							Anchor = Anchor.Center,
							Action = () => { Write("Button 4 clicked");}
						},
					}
				},
				new BasicButton()
				{
					Text = "Button 5",
					Position = new(0.5f, 0.5f),
					RelativePositionAxes = Axes.Both,
					Origin = Anchor.Center,
					Action = () => { Write("Button 5 clicked");},
					Size = new(100, 100)
				},
			}
		});

		Add(_textBox = new BasicTextBox()
		{
			RelativePositionAxes = Axes.Both,
			Position = new(0.5f, 0)
		});
	}

	public void Write(string text)
	{
		Console.WriteLine(text);
	}
}
