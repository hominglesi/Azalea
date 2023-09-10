using Azalea.Graphics;
using Azalea.Graphics.Containers;
using Azalea.Graphics.Shapes;
using Azalea.Graphics.Sprites;
using Azalea.Graphics.UserInterface;
using Azalea.Inputs;
using Azalea.IO.Assets;
using Azalea.IO.Stores;
using System.Numerics;

namespace Azalea.VisualTests;

public class VisualTests : AzaleaGame
{
	private FlexContainer _container;
	private BasicTextBox _text;

	protected override void OnInitialize()
	{
		Resources.AddStore(new NamespacedResourceStore<byte[]>(new DllResourceStore(typeof(VisualTests).Assembly), "Resources"));

		Host.Renderer.ClearColor = Color.Azalea;

		Add(new Outline(_container = new FlexContainer()
		{
			Size = new Vector2(400, 400),
			Direction = FlexDirection.Vertical
		}));
		Add(_text = new BasicTextBox()
		{
			Width = 500,
			Height = 30,
			Position = new Vector2(500, 500)
		});
		_text.Text = "Ide Gas";

		_container.Children = new GameObject[]
		{
			new Sprite()
			{
				Texture = Assets.GetTexture("wall.png"),
				Size = new Vector2(100, 100)
			},
			new Sprite()
			{
				Texture = Assets.GetTexture("wall.png"),
				Size = new Vector2(100, 100)
			},
			new Sprite()
			{
				Texture = Assets.GetTexture("wall.png"),
				Size = new Vector2(100, 100)
			},
			new Sprite()
			{
				Texture = Assets.GetTexture("wall.png"),
				Size = new Vector2(100, 100)
			},
			new Sprite()
			{
				Texture = Assets.GetTexture("wall.png"),
				Size = new Vector2(100, 100)
			},
			new Sprite()
			{
				Texture = Assets.GetTexture("wall.png"),
				Size = new Vector2(100, 100)
			},
			new Sprite()
			{
				Texture = Assets.GetTexture("wall.png"),
				Size = new Vector2(100, 100)
			},
			new Sprite()
			{
				Texture = Assets.GetTexture("wall.png"),
				Size = new Vector2(100, 100)
			}
		};
	}

	protected override void Update()
	{
		_container.Size = Input.MousePosition;

		if (Input.GetKey(Keys.Escape).Down) Host.Window.Close();
	}
}
