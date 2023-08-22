using Azalea.Graphics;
using Azalea.Graphics.Containers;
using Azalea.Graphics.Shapes;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.IO.Assets;
using Azalea.IO.Stores;
using System.Numerics;

namespace Azalea.VisualTests;

internal class VisualTests : AzaleaGame
{
    private FlexContainer _container;

    protected override void OnInitialize()
    {
        Resources.AddStore(new NamespacedResourceStore<byte[]>(new DllResourceStore(typeof(VisualTests).Assembly), "Resources"));

        Host.Renderer.ClearColor = Color.Azalea;

        Add(new Outline(_container = new FlexContainer()
        {
            Size = new Vector2(400, 400),
            Direction = FlexDirection.Vertical
        }));

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
    }
}
