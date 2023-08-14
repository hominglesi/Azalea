using Azalea.Extentions;
using Azalea.Graphics;
using Azalea.Graphics.Containers;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.IO.Assets;
using Azalea.IO.Stores;
using Azalea.Platform;
using Azalea.Utils;
using System.Numerics;

namespace Azalea.VisualTests;

internal class VisualTests : AzaleaGame
{
    private Sprite cursor;
    private Sprite hidden;
    private Sprite solid;
    private SpriteText text;

    private GridContainer container;

    protected override void OnInitialize()
    {
        Resources.AddStore(new DllResourceStore(typeof(VisualTests).Assembly));

        Host.Renderer.ClearColor = Color.Azalea;

        var rows = 5;
        var colls = 5;

        var count = 0;

        var content = new GameObject[rows, colls];
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < colls; c++)
            {
                content[r, c] = new Sprite()
                {
                    Texture = count == 0 ? Assets.GetTexture("Resources/wall2.png") : Assets.GetTexture("Resources/wall.png"),
                    Size = new Vector2(1, 1),
                    RelativeSizeAxes = Axes.Both,
                };
                count = count == 0 ? 1 : 0;
            }


        AddRange(new GameObject[]{
            hidden = new Sprite()
            {
                Texture = Assets.GetTexture("Resources/wall2.png"),
                Position = Vector2.Zero,
                Size = Host.Window.ClientSize
            },
            container = new GridContainer()
            {
                Content = GenerateRandomContent(5, 5).ToJagged(),
                Size = new Vector2(400, 200),
                Position = Vector2.Zero
                /*
                Content = content.ToJagged(),
                Size = new Vector2(400, 200),
                Position = new Vector2(50, 500)*/
            }/*,
            solid = new TestGameObject()
            {
                Texture = Host.Renderer.WhitePixel,
                Position = new Vector2(200, 200),
                Size = new Vector2(300, 50),
                Color = Color.Lime
            },
            text = new SpriteText()
            {
                Text = "Ovo ce nestati ako se stisne F1",
                Font = FontUsage.Default.With(size: 48),
                Position = new Vector2(100, 300)
            },
            new SpriteText()
            {
                Text = "Ide Gas 2",
                Font = FontUsage.Default,
                Position = new Vector2(50, 50)
            },
            new BasicButton()
            {
                Position = new Vector2(200, 400),
                Text = "Print Pog!",
                Action = () => { Console.WriteLine("Pog!"); }
            },
            cursor = new Sprite()
            {
                Texture = Assets.GetTexture("Resources/wall.png"),
                Position = Input.MousePosition,
                Size = new Vector2(40, 40)
            }*/
        });
    }

    private bool removed = false;

    protected override void Update()
    {
        //cursor.Position = Input.MousePosition;
        container.Size = Input.MousePosition;

        if (Input.GetKey(Keys.ShiftLeft).Pressed || Input.GetMouseButton(MouseButton.Left).Pressed)
            hidden.Alpha = 1;
        else hidden.Alpha = 0;

        if (Input.GetKey(Keys.F9).Down)
        {
            if (Host.Window.State == WindowState.Normal)
                Host.Window.State = WindowState.BorderlessFullscreen;
            else
                Host.Window.State = WindowState.Normal;
        }

        if (Input.GetKey(Keys.F1).Down && removed == false)
        {
            RemoveRange(new GameObject[] { text });
            removed = true;
        }

        if (Input.GetKey(Keys.P).Down) container.Content = GenerateRandomContent(Rng.Int(1, 8), Rng.Int(1, 8)).ToJagged();
    }

    private GameObject[,] GenerateRandomContent(int rows, int colls)
    {
        var content = new GameObject[rows, colls];
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < colls; c++)
            {
                content[r, c] = new Sprite()
                {
                    Texture = Rng.Int(2) == 0 ? Assets.GetTexture("Resources/wall2.png") : Assets.GetTexture("Resources/wall.png"),
                    Size = new Vector2(1, 1),
                    RelativeSizeAxes = Axes.Both,
                };
            }

        return content;
    }
}
