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
    private Sprite solid;

    protected override void OnInitialize()
    {
        Host.Renderer.ClearColor = Color.Azalea;
        //Host.Window.ClientSize = new Vector2Int(1280, 720);

        AddRange(new[]{
            hidden = new Sprite()
            {
                Texture = Assets.GetTexture("wall2.png"),
                Position = Vector2.Zero,
                Size = Host.Window.ClientSize
            },
            solid = new Sprite()
            {
                Texture = Host.Renderer.WhitePixel,
                Position = new Vector2(200, 200),
                Size = new Vector2(300, 50),
                Color = Color.Azalea
            },
            cursor = new Sprite()
            {
                Texture = Assets.GetTexture("wall.png"),
                Position = Input.MousePosition,
                Size = new Vector2(100, 200),
                Color = Color.Red
            }
        });
    }

    protected override void OnUpdate()
    {
        cursor.Position = Input.MousePosition;

        if (Input.GetKey(Keys.ShiftLeft).Pressed || Input.GetMouseButton(0).Pressed)
            hidden.Alpha = 1;
        else hidden.Alpha = 0;
    }
}
