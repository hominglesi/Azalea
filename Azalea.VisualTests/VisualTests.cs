using Azalea.Graphics;
using Azalea.Graphics.Containers;
using Azalea.Graphics.Sprites;
using Azalea.IO.Assets;
using Azalea.IO.Stores;
using System;
using System.Numerics;

namespace Azalea.VisualTests;

internal class VisualTests : AzaleaGame
{
    private FillFlowContainer _container;

    protected override void OnInitialize()
    {
        Resources.AddStore(new NamespacedResourceStore<byte[]>(new DllResourceStore(typeof(VisualTests).Assembly), "Resources"));

        Host.Renderer.ClearColor = Color.Azalea;

        Add(_container = new FillFlowContainer()
        {
            MaximumSize = new Vector2(500, 500)
        });
        _container.Direction = FillDirection.Vertical;

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
        };
    }

    private bool removed = false;

    protected override void Update()
    {
        //_container.Size = Input.MousePosition;

        Console.WriteLine(_container.Size);
    }
}
