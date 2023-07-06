using Azalea.Graphics.Primitives;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using System;

namespace Azalea.Graphics.Sprites;

public class SpriteDrawNode : TexturedShaderDrawNode
{
    protected Texture? Texture { get; set; }
    protected Quad ScreenSpaceDrawQuad { get; set; }

    protected new Sprite Source => (Sprite)base.Source;

    public SpriteDrawNode(IGameObject source)
        : base(source) { }

    public override void ApplyState()
    {
        base.ApplyState();

        Texture = Source.Texture;
        ScreenSpaceDrawQuad = Source.ScreenSpaceDrawQuad;
    }

    protected virtual void Blit(IRenderer renderer)
    {
        renderer.DrawQuad(
            Texture ?? throw new Exception("Texture must be applied before drawing"),
            ScreenSpaceDrawQuad,
            DrawColorInfo);
    }

    public override void Draw(IRenderer renderer)
    {
        base.Draw(renderer);

        Blit(renderer);
    }
}
