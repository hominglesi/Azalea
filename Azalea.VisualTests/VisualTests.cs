using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.IO.Assets;
using Azalea.IO.Stores;
using System.Numerics;

namespace Azalea.VisualTests;

internal class VisualTests : AzaleaGame
{
    private Sprite cursor;
    private Sprite hidden;
    private Sprite solid;
    private SpriteText text;

    protected override void OnInitialize()
    {
        Resources.AddStore(new DllResourceStore(typeof(VisualTests).Assembly));

        Host.Renderer.ClearColor = Color.Azalea;

        AddRange(new GameObject[]{
            hidden = new Sprite()
            {
                Texture = Assets.GetTexture("Resources/wall2.png"),
                Position = Vector2.Zero,
                Size = Host.Window.ClientSize
            },
            solid = new TestGameObject()
            {
                Texture = Host.Renderer.WhitePixel,
                Position = new Vector2(200, 200),
                Size = new Vector2(300, 50),
                Color = Color.Lime
            },
            cursor = new Sprite()
            {
                Texture = Assets.GetTexture("Resources/wall.png"),
                Position = Input.MousePosition,
                Size = new Vector2(400, 400)
            },
            text = new SpriteText()
            {
                Text = "Ide Gas",
                Font = FontUsage.Default.With(size: 48),
                Position = new Vector2(100, 100)
            },
            new SpriteText()
            {
                Text = "Ide Gas 2",
                Font = FontUsage.Default,
                Position = new Vector2(50, 50)
            }
        });
    }

    protected override void Update()
    {
        cursor.Position = Input.MousePosition;

        if (Input.GetKey(Keys.ShiftLeft).Pressed || Input.GetMouseButton(MouseButton.Left).Pressed)
            hidden.Alpha = 1;
        else hidden.Alpha = 0;

        if (Input.GetKey(Keys.Enter).Down) TriggerClick();
    }
}
