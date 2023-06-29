using Azalea.Graphics.Rendering.Vertices;
using Azalea.Graphics.Textures;
using System.Numerics;

namespace Azalea.Graphics.Rendering;

public static class RendererExtentions
{
    public static void DrawQuad(this IRenderer renderer, Texture texture, Vector2 position, Vector2 size, Color drawColor = default)
    {
        renderer.BindTexture(texture);

        float left = position.X;
        float right = position.X + size.X;
        float top = position.Y;
        float bottom = position.Y + size.Y;

        var vertexAction = renderer.DefaultQuadBatch.AddAction;

        vertexAction(new TexturedVertex2D
        {
            Position = new(left, top),
            Color = drawColor,
            TexturePosition = new(0, 0)
        });

        vertexAction(new TexturedVertex2D
        {
            Position = new(right, top),
            Color = drawColor,
            TexturePosition = new(1, 0)
        });

        vertexAction(new TexturedVertex2D
        {
            Position = new(right, bottom),
            Color = drawColor,
            TexturePosition = new(1, 1)
        });

        vertexAction(new TexturedVertex2D
        {
            Position = new(left, bottom),
            Color = drawColor,
            TexturePosition = new(0, 1)
        });
    }
}
