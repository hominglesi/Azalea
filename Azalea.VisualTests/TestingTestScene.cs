using Azalea.Amends;
using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Design.Tiled;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Graphics.Textures;
using Azalea.Inputs;
using Azalea.IO.Resources;
using Azalea.Platform;
using System.Numerics;

namespace Azalea.VisualTests;

public class TestingTestScene : TestScene
{
	private Composition _wrapper;
	private Composition _comp;
	private TextContainer _composition;
	private BasicTextBox _text;
	private Box _cursor;
	private Box _igrac;
	private Sprite _sprite;

	private SpriteText _scrollDisplay;

	public TestingTestScene()
	{

		var tileset = Assets.MainStore.GetTileset("MapForTiled/TilesForMap.tsx");

		/*
		Add(new Composition()
		{
			AutoSizeAxes = Graphics.Axes.Both,
			Child = new Sprite()
			{
				Texture = tileset.Source,
				Scale = new(3),
			},
			BackgroundColor = Palette.Aqua
		});

		var anim = new TextureAnimation();
		anim.AddFrames(tileset.Tiles, 0.1f);*/

		Add(_sprite = new Sprite()
		{
			Position = new(0, 600),
			Texture = tileset.Tiles[0],
		});

		_sprite.Loop((s) =>
		{
			s.RepositionBy(new(1000, 0), 3)
			.Then().RepositionBy(new(-1000, 0), 3);
		}, 6);

		var tilemap = Assets.MainStore.GetTilemap("MapForTiled/FirstMap.tmx");

		//var zoomMultiplier = new Vector2(1280, 720) / new Vector2(tilemap.Width * tilemap.TileWidth, tilemap.Height * tilemap.TileHeight);

		foreach (var layer in tilemap.Layers)
		{
			Add(new TiledLayer(tilemap, layer));
		}

		foreach (var obj in tilemap.Objects)
		{
			if (obj.TileId != -1)
			{
				//var pos = new Vector2(obj.X, obj.Y) * zoomMultiplier / new Vector2(1280, 720);
				//var size = new Vector2(obj.Width, obj.Height) * zoomMultiplier / new Vector2(1280, 720);

				Add(new Sprite()
				{
					Texture = tilemap.GetTextureById(obj.TileId),
					//RelativePositionAxes = Axes.Both,
					//RelativeSizeAxes = Axes.Both,
					Position = obj.Position,
					Size = obj.Size,
					Origin = Anchor.BottomLeft,
					Depth = -obj.Y + 10
				});
			}
		}



		Add(_igrac = new()
		{
			Color = Palette.Orange,
			Size = new(50, 50),
			Origin = Anchor.BottomCenter
		});
		Add(_wrapper = new Composition()
		{
			RelativeSizeAxes = Axes.Both,
			Child = _composition = new TextContainer(t =>
			{ t.Font = t.Font.With(size: 40); t.Padding = new(0, 0, 0, 0); })
			{
				//Position = new Vector2(100, 150),
				RelativeSizeAxes = Axes.Both,
				Text = "Lorem ipsum dolor sit amet,\n consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum" +
				"Lorem ipsum dolor sit amet,\n consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum" +
				"Lorem ipsum dolor sit amet,\n consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum",
				//BackgroundColor = Palette.Black,
				BorderColor = Palette.Black,
				Masking = false,
			}
		});

		Add(new Sprite()
		{
			Position = new(850, 400),
			Size = new(250, 250),
			Origin = Anchor.Center,
			Texture = new TextureAnimation(tileset.Tiles, 0.2f)
		});

		_composition.AddText("Text3 ", t => { t.Color = Palette.Black; });
		_composition.AddText("Text4 ");

		Add(_text = new BasicTextBox()
		{
			Width = 500,
			Height = 30,
			Position = new Vector2(500, 500),
			Text = "Ide Gas",
			Alpha = 0
		}
		.RepositionBy(new(-100, 0), 0)
		.RepositionBy(new(100, 0), 1.5f)
		.ChangeAlphaTo(1, 1.5f)
		.Then()
		.Loop(x => x.RepositionBy(new(0, -100), 1)
				.Then().RepositionBy(new(0, 100), 1), 2));

		Add(new BasicTextBox()
		{
			Width = 500,
			Height = 30,
			Position = new Vector2(550, 300)
		});

		Add(_comp = new FlexContainer()
		{
			Direction = FlexDirection.Vertical,
			Wrapping = FlexWrapping.NoWrapping,
			BackgroundColor = Palette.Red,
			AutoSizeAxes = Axes.Both,
			Children = new GameObject[] {
				new Box()
				{
					Color = new Color(10, 10, 10, 10),
					Size = new(50, 50),
					Position = new(30, 30)
				},

				new Box()
				{
					Color = new Color(10, 10, 10, 10),
					Size = new(50, 50)
				},
			}
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
			//Alpha = 0.5f,
			BackgroundColor = ColorQuad.HorizontalGradient(Palette.Blue, Palette.Green),
			BorderColor = new ColorQuad(Palette.Black, Palette.Gray, Palette.White, Palette.Gray),
			BorderThickness = new(6)
		});

		Add(_scrollDisplay = new SpriteText()
		{
			Position = new(700, 100),
			Text = "0"
		});

		BasicWindow window;
		Add(window = new BasicWindow()
		{
			Position = new(300, 300),
			Child = new ScrollableContainer()
			{
				RelativeSizeAxes = Axes.Both,
				Child = new SpriteText()
				{
					Color = Palette.Black,
					Text = "JA sam U windowu"
				}
			}
		});

		Add(_cursor = new Box()
		{
			Size = new(50, 50)
		});
	}



	private float _elapsedTime = 0;
	protected override void Update()
	{
		_sprite.Position += Input.GetDirectionalMovement() * Time.DeltaTime * 200;


		/*
		_elapsedTime += Time.DeltaTime;
		Console.WriteLine(_elapsedTime);

		_igrac.Position = Input.MousePosition;

		_igrac.GetFirstParentOfType<Composition>().ChangeChildDepth(_igrac, -_igrac.Y);*/

		//_composition.Size = Input.MousePosition - _composition.ToScreenSpace(_composition.Position);
		//_scrollDisplay.Text = (float.Parse(_scrollDisplay.Text) + Input.MouseWheelDelta).ToString();


		/*
		_cursor.Position = Input.MousePosition;

		if (Input.GetKey(Keys.Space).DownOrRepeat) Console.WriteLine("Pressed space");

		if (Input.GetKey(Keys.M).Down)
		{
			//_comp.Invalidate(Invalidation.RequiredParentSizeToFit, InvalidationSource.Child);
			_comp.InternalComposition.Invalidate(Invalidation.RequiredParentSizeToFit, InvalidationSource.Child);
		}

		if (Input.GetKey(Keys.P).Down)
		{
			AzaleaGame.Main.Host.Window.State = WindowState.Normal;
		}
		else if (Input.GetKey(Keys.O).Down)
		{
			AzaleaGame.Main.Host.Window.State = WindowState.Minimized;
		}
		else if (Input.GetKey(Keys.I).Down)
		{
			AzaleaGame.Main.Host.Window.State = WindowState.Maximized;
		}*/
	}
}
