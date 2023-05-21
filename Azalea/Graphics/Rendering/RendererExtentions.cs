using Azalea.Graphics.Rendering.Vertices;
using System.Numerics;

namespace Azalea.Graphics.Rendering;

public static class RendererExtentions
{
    public static void DrawQuad(this IRenderer renderer, Vector2 position, Vector2 size, Color color)
    {
        float left = position.X;
        float right = position.X + size.X;
        float top = position.Y;
        float bottom = position.Y + size.Y;

        var vertexAction = renderer.DefaultQuadBatch.AddAction;

        vertexAction(new PositionColorVertex
        {
            Position = new(left, top),
            Color = color
        });

        vertexAction(new PositionColorVertex
        {
            Position = new(right, top),
            Color = color
        });

        vertexAction(new PositionColorVertex
        {
            Position = new(right, bottom),
            Color = color
        });

        vertexAction(new PositionColorVertex
        {
            Position = new(left, bottom),
            Color = color
        });
    }
}
