using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.IO.Assets;
using System.Numerics;

namespace Azalea.VisualTests;

internal class VisualTests : AzaleaGame
{
    private Sprite cursor;
    private Sprite hidden;

    protected override void OnInitialize()
    {
        Host.Renderer.ClearColor = Color.Azalea;


        Add(cursor = new Sprite()
        {
            Texture = Assets.GetTexture("wall.png"),
            Position = Input.MousePosition,
            Size = new Vector2(100, 200)
        });

        Add(hidden = new Sprite()
        {
            Texture = Assets.GetTexture("wall2.png"),
            Position = new Vector2(150, 150),
            Size = new Vector2(100, 200)
        });
    }

    protected override void OnUpdate()
    {
        cursor.Position = Input.MousePosition;

        if (Input.GetKey(Keys.A).Pressed || Input.GetMouseButton(0).Pressed)
            hidden.Alpha = 1;
        else hidden.Alpha = 0;
    }
}
