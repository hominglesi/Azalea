using Azalea.Graphics;
using Azalea.Graphics.Textures;
using System.Numerics;
using Azalea.Graphics.Rendering;
using Azalea.Inputs;
using System.Diagnostics;

namespace Azalea.VisualTests;

internal class VisualTests : AzaleaGame
{
    private Texture _tex1;
    private Texture _tex2;

    protected override void OnInitialize()
    {
        Host.Renderer.ClearColor = Color.Azalea;

        using var stream1 = File.OpenRead("wall.png");
        using var stream2 = File.OpenRead("wall2.png");
        _tex1 = Texture.FromStream(Host.Renderer, stream1) ?? throw new Exception();
        _tex2 = Texture.FromStream(Host.Renderer, stream2) ?? throw new Exception();
    }

    protected override void OnRender()
    {
        Host.Renderer.DrawQuad(_tex1, Input.MousePosition, new Vector2(100, 200));
        if (Input.GetKey(Keys.A).Pressed || Input.GetMouseButton(0).Pressed)
            Host.Renderer.DrawQuad(_tex2, new Vector2(150, 150), new Vector2(100, 200));

    }
}
