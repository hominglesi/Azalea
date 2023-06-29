using Azalea.Graphics;
using Azalea.Graphics.Textures;
using System.Numerics;
using Azalea.Graphics.Rendering;
using Azalea.Inputs;
using System.Diagnostics;
using Azalea.IO.Assets;

namespace Azalea.VisualTests;

internal class VisualTests : AzaleaGame
{
    private Texture _tex1;
    private Texture _tex2;

    protected override void OnInitialize()
    {
        Host.Renderer.ClearColor = Color.Azalea;

        _tex1 = Assets.GetTexture("wall.png");
        _tex2 = Assets.GetTexture("wall2.png");
    }

    protected override void OnRender()
    {
        Host.Renderer.DrawQuad(_tex1, Input.MousePosition, new Vector2(100, 200));
        if (Input.GetKey(Keys.A).Pressed || Input.GetMouseButton(0).Pressed)
            Host.Renderer.DrawQuad(_tex2, new Vector2(150, 150), new Vector2(100, 200));
    }
}
