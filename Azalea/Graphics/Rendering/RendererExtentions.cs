using Azalea.Graphics.Colors;
using Azalea.Graphics.Primitives;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Graphics.Textures;
using System.Numerics;

namespace Azalea.Graphics.Rendering;

public static class RendererExtentions
{
	internal static void DrawQuad(this IRenderer renderer, Texture texture, Quad vertexQuad, DrawColorInfo drawColorInfo)
	{
		renderer.BindTexture(texture);

		var vertexAction = renderer.DefaultQuadBatch.AddAction;

		vertexAction(new TexturedVertex2D
		{
			Position = vertexQuad.BottomLeft,
			Color = drawColorInfo.Color,
			TexturePosition = new(0, 1)
		});

		vertexAction(new TexturedVertex2D
		{
			Position = vertexQuad.BottomRight,
			Color = drawColorInfo.Color,
			TexturePosition = new(1, 1)
		});

		vertexAction(new TexturedVertex2D
		{
			Position = vertexQuad.TopRight,
			Color = drawColorInfo.Color,
			TexturePosition = new(1, 0)
		});

		vertexAction(new TexturedVertex2D
		{
			Position = vertexQuad.TopLeft,
			Color = drawColorInfo.Color,
			TexturePosition = new(0, 0)
		});
	}

	internal static void DrawRectangle(this IRenderer renderer, Quad quad, float width, DrawColorInfo color)
	{
		var texture = renderer.WhitePixel;

		var topQuad = new Quad(
			quad.TopLeft + new Vector2(-width, -width),
			quad.TopLeft + new Vector2(-width, 0),
			quad.TopRight + new Vector2(width, 0),
			quad.TopRight + new Vector2(width, -width));

		var rightQuad = new Quad(
			quad.TopRight,
			quad.BottomRight + new Vector2(0, width),
			quad.BottomRight + new Vector2(width, width),
			quad.TopRight + new Vector2(width, 0));

		var bottomQuad = new Quad(
			quad.BottomLeft,
			quad.BottomLeft + new Vector2(0, width),
			quad.BottomRight + new Vector2(0, width),
			quad.BottomRight);

		var leftQuad = new Quad(
			quad.TopLeft + new Vector2(-width, 0),
			quad.BottomLeft + new Vector2(-width, width),
			quad.BottomLeft + new Vector2(0, width),
			quad.TopLeft);

		renderer.DrawQuad(texture, topQuad, color);
		renderer.DrawQuad(texture, rightQuad, color);
		renderer.DrawQuad(texture, bottomQuad, color);
		renderer.DrawQuad(texture, leftQuad, color);
	}
}
