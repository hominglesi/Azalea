using Azalea.Graphics.Primitives;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Graphics.Textures;

namespace Azalea.Graphics.Rendering;

public static class RendererExtentions
{
    internal static void DrawQuad(this IRenderer renderer, Texture texture, Quad vertexQuad, DrawColorInfo drawColorInfo)
    {
        if (drawColorInfo.Alpha <= 0) return;

        renderer.BindTexture(texture);

        var vertexAction = renderer.DefaultQuadBatch.AddAction;

        vertexAction(new TexturedVertex2D
        {
            Position = vertexQuad.BottomLeft,
            Color = new Color(255, 255, 255, (byte)(drawColorInfo.Alpha * byte.MaxValue)),
            TexturePosition = new(0, 1)
        });

        vertexAction(new TexturedVertex2D
        {
            Position = vertexQuad.BottomRight,
            Color = new Color(255, 255, 255, (byte)(drawColorInfo.Alpha * byte.MaxValue)),
            TexturePosition = new(1, 1)
        });

        vertexAction(new TexturedVertex2D
        {
            Position = vertexQuad.TopRight,
            Color = new Color(255, 255, 255, (byte)(drawColorInfo.Alpha * byte.MaxValue)),
            TexturePosition = new(1, 0)
        });

        vertexAction(new TexturedVertex2D
        {
            Position = vertexQuad.TopLeft,
            Color = new Color(255, 255, 255, (byte)(drawColorInfo.Alpha * byte.MaxValue)),
            TexturePosition = new(0, 0)
        });
    }
}
