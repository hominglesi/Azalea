using Azalea.Graphics.Shapes;
using Azalea.Graphics.Sprites;
using System.Numerics;

namespace Azalea.Graphics.UserInterface;

public class BasicButton : Button
{
    public string Text
    {
        get => SpriteText.Text;
        set => SpriteText.Text = value;
    }

    public Color TextColor
    {
        get => SpriteText.Color;
        set => SpriteText.Color = value;
    }

    public Color BackgroundColor
    {
        get => Background.Color;
        set => Background.Color = value;
    }

    protected Box Background;
    protected SpriteText SpriteText;

    public BasicButton()
    {
        Size = new Vector2(200, 70);
        AddRange(new GameObject[]
        {
            Background = new Box()
            {
                Color = Color.Blue,
                Size = Size
            },
            SpriteText = new SpriteText
            {
                Font = FrameworkFont.Regular,
                Color = Color.Azalea,
                Anchor = Anchor.Center
            },
            new Box()
            {
                Size = new Vector2(40,20),
                Anchor = Anchor.Center,
                Origin = Anchor.Center
            }
        });
    }
}
