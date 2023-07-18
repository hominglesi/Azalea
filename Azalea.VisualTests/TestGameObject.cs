using Azalea.Graphics.Sprites;
using Azalea.Inputs.Events;
using System.Numerics;

namespace Azalea.VisualTests;

public class TestGameObject : Sprite
{
    protected override bool OnClick(ClickEvent e)
    {
        Position = new Vector2(Rng.Float(0, 500), Rng.Float(0, 500));
        return true;
    }
}
