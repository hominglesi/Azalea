using Azalea.Design.Containers;
using Azalea.Design.Docking;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using Azalea.IO.Resources;
using Azalea.Utils;
using System.Numerics;

namespace Azalea.VisualTests;
public class DockingTest : TestScene
{
	public BasicDockingContainer _leftDocker;
	public BasicDockingContainer _bottomDocker;
	public BasicDockingContainer _rightDocker;

	public DockingTest()
	{
		Add(_leftDocker = new BasicDockingContainer()
		{
			Width = 250
		});

		Add(_bottomDocker = new BasicDockingContainer()
		{
			Anchor = Anchor.BottomLeft,
			Origin = Anchor.BottomLeft,
			Height = 300,
		});

		Add(_rightDocker = new BasicDockingContainer()
		{
			Anchor = Anchor.TopRight,
			Origin = Anchor.TopRight,
			RelativeSizeAxes = Axes.Y,
			Width = 350,
			ContentPadding = 10
		});

		_leftDocker.AddDockable("Azalea", new Sprite()
		{
			Texture = Assets.GetTexture("Textures/azalea-icon.png"),
			Anchor = Anchor.Center,
			Origin = Anchor.Center
		});

		_leftDocker.AddDockable("Missing Texture", new Sprite()
		{
			Texture = Assets.MissingTexture,
			Size = new(128),
			Anchor = Anchor.Center,
			Origin = Anchor.Center
		});

		FlexContainer iconsFlex;

		_bottomDocker.AddDockable("Icons", new ScrollableContainer()
		{
			RelativeSizeAxes = Axes.Both,
			Child = iconsFlex = new FlexContainer()
			{
				RelativeSizeAxes = Axes.X,
				AutoSizeAxes = Axes.Y,
				Direction = FlexDirection.Horizontal,
				Wrapping = FlexWrapping.Wrap,
				Spacing = new(5)
			}
		});

		for (int i = 0; i < 106; i++)
		{
			iconsFlex.Add(new Box()
			{
				Size = new(80),
				Color = Rng.Color()
			});
		}

		FlexContainer listFlex;

		_bottomDocker.AddDockable("List", new ScrollableContainer()
		{
			RelativeSizeAxes = Axes.Both,
			Child = listFlex = new FlexContainer()
			{
				RelativeSizeAxes = Axes.X,
				AutoSizeAxes = Axes.Y,
				Direction = FlexDirection.Vertical,
				Spacing = new(5)
			}
		});

		for (int i = 0; i < 20; i++)
		{
			listFlex.Add(new Box()
			{
				RelativeSizeAxes = Axes.X,
				Size = new(0.95f, 60),
				Color = Rng.Color()
			});
		}

		_bottomDocker.AddDockable("Lorem Ipsum", new TextContainer()
		{
			RelativeSizeAxes = Axes.Both,
			Text = TextUtils.GenerateLoremIpsum(128)
		});

		_rightDocker.AddDockable("Properties", new FlexContainer()
		{
			RelativeSizeAxes = Axes.Both,
			Direction = FlexDirection.Vertical,
			Spacing = new(20),
			Children = new[]
			{
				new SpriteText() {Text = "Property 1: 42.63"},
				new SpriteText() {Text = "Property 2: \"Text\""},
				new SpriteText() {Text = "Property 3: true"},
				new SpriteText() {Text = "Property 4: 5.36"},
				new SpriteText() {Text = "Property 5: 882"},
				new SpriteText() {Text = "Property 6: false"},
				new SpriteText() {Text = "Property 7: \"Names\""},
			}
		});
	}

	private Vector2 _lastDrawSize;
	protected override void Update()
	{
		if (DrawSize != _lastDrawSize)
		{
			_leftDocker.Height = DrawHeight - _bottomDocker.Height;
			_bottomDocker.Width = DrawWidth - _rightDocker.Width;

			_lastDrawSize = DrawSize;
		}
	}
}
