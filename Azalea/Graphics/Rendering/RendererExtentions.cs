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
			Color = drawColorInfo.Color.BottomLeft,
			TexturePosition = new(0, 1)
		});

		vertexAction(new TexturedVertex2D
		{
			Position = vertexQuad.BottomRight,
			Color = drawColorInfo.Color.BottomRight,
			TexturePosition = new(1, 1)
		});

		vertexAction(new TexturedVertex2D
		{
			Position = vertexQuad.TopRight,
			Color = drawColorInfo.Color.TopRight,
			TexturePosition = new(1, 0)
		});

		vertexAction(new TexturedVertex2D
		{
			Position = vertexQuad.TopLeft,
			Color = drawColorInfo.Color.TopLeft,
			TexturePosition = new(0, 0)
		});
	}

	internal static void DrawRectangle(this IRenderer renderer, Quad quad, float width, DrawColorInfo color)
	{
		var texture = renderer.WhitePixel;

		var topQuad = new Quad(
			quad.TopLeft,
			quad.TopLeft + new Vector2(0, width),
			quad.TopRight + new Vector2(-width, width),
			quad.TopRight + new Vector2(-width, 0));

		var rightQuad = new Quad(
			quad.TopRight + new Vector2(-width, 0),
			quad.BottomRight + new Vector2(-width, -width),
			quad.BottomRight + new Vector2(0, -width),
			quad.TopRight);

		var bottomQuad = new Quad(
			quad.BottomLeft + new Vector2(width, -width),
			quad.BottomLeft + new Vector2(width, 0),
			quad.BottomRight,
			quad.BottomRight + new Vector2(0, -width));

		var leftQuad = new Quad(
			quad.TopLeft + new Vector2(0, -width),
			quad.BottomLeft,
			quad.BottomLeft + new Vector2(width, 0),
			quad.TopLeft + new Vector2(width, width));

		renderer.DrawQuad(texture, topQuad, color);
		renderer.DrawQuad(texture, rightQuad, color);
		renderer.DrawQuad(texture, bottomQuad, color);
		renderer.DrawQuad(texture, leftQuad, color);
	}
}
