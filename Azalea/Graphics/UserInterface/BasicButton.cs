using Azalea.Graphics.Shapes;
using Azalea.Graphics.Sprites;
using Azalea.Inputs.Events;
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

    public Color HoveredColor
    {
        get => HoveredBackground.Color;
        set => HoveredBackground.Color = value;
    }

    protected Box Background;
    protected Box HoveredBackground;
    protected SpriteText SpriteText;

    public BasicButton()
    {
        Size = new Vector2(200, 70);
        AddRange(new GameObject[]
        {
            Background = new Box()
            {
                RelativeSizeAxes = Axes.Both,
                Color = Color.Blue
            },
            HoveredBackground = new Box()
            {
                RelativeSizeAxes = Axes.Both,
                Color = Color.Cornflower,
                Alpha = 0,
            },
            SpriteText = new SpriteText
            {
                Font = FrameworkFont.Regular,
                Color = Color.Azalea,
                Anchor = Anchor.Center,
                Origin = Anchor.Center
            }
        });
    }

    protected override bool OnHover(HoverEvent e)
    {
        HoveredBackground.Alpha = 1;

        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        HoveredBackground.Alpha = 0;

        base.OnHoverLost(e);
    }
}
