using Azalea.Graphics.Colors;
using Azalea.Graphics.Primitives;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Graphics.Textures;
using Azalea.Numerics;
using Azalea.Platform;
using System;

namespace Azalea.Graphics.Rendering;

public static class RendererExtentions
{
	internal static bool ClientContainsQuad(Quad quad)
	{
		var clientSize = Window.ClientSize;

		var mostLeft = MathF.Min(quad.TopLeft.X, MathF.Min(quad.TopRight.X, MathF.Min(quad.BottomLeft.X, quad.BottomRight.X)));
		var mostTop = MathF.Min(quad.TopLeft.Y, MathF.Min(quad.TopRight.Y, MathF.Min(quad.BottomLeft.Y, quad.BottomRight.Y)));
		var mostRight = MathF.Max(quad.TopLeft.X, MathF.Max(quad.TopRight.X, MathF.Max(quad.BottomLeft.X, quad.BottomRight.X)));
		var mostBottom = MathF.Max(quad.TopLeft.Y, MathF.Max(quad.TopRight.Y, MathF.Max(quad.BottomLeft.Y, quad.BottomRight.Y)));

		if (mostLeft < clientSize.X && mostRight > 0 && mostTop < clientSize.Y)
			return mostBottom > 0;

		return false;
	}

	public static void DrawQuad(this IRenderer renderer, Texture texture, Quad vertexQuad, DrawColorInfo drawColorInfo, Rectangle? customUV = null)
	{
		if (ClientContainsQuad(vertexQuad) == false)
			return;

		renderer.BindTexture(texture);

		var vertexAction = renderer.DefaultQuadBatch.AddAction;

		var textureRegion = customUV ?? texture.GetUVCoordinates();

		vertexAction(new TexturedVertex2D
		{
			Position = vertexQuad.BottomLeft,
			Color = drawColorInfo.Color.BottomLeft,
			TexturePosition = textureRegion.BottomLeft
		});

		vertexAction(new TexturedVertex2D
		{
			Position = vertexQuad.BottomRight,
			Color = drawColorInfo.Color.BottomRight,
			TexturePosition = textureRegion.BottomRight
		});

		vertexAction(new TexturedVertex2D
		{
			Position = vertexQuad.TopRight,
			Color = drawColorInfo.Color.TopRight,
			TexturePosition = textureRegion.TopRight
		});

		vertexAction(new TexturedVertex2D
		{
			Position = vertexQuad.TopLeft,
			Color = drawColorInfo.Color.TopLeft,
			TexturePosition = textureRegion.TopLeft
		});
	}

	public static void DrawRectangle(this IRenderer renderer, Rectangle rect, Matrix3 drawMatrix, Boundary thickness, DrawColorInfo color, bool towardsOutside)
	{

		var texture = renderer.WhitePixel;

		var topRect = towardsOutside ?
			new Rectangle(rect.Top - thickness.Top, rect.Left - thickness.Left, rect.Width + thickness.Left, thickness.Top) :
			new Rectangle(rect.Top, rect.Left, rect.Width - thickness.Right, thickness.Top);

		var topColor = new ColorQuad(
			color.Color.TopLeft,
			color.Color.TopLeft,
			color.Color.TopRight,
			color.Color.TopRight);

		var rightRect = towardsOutside ?
			new Rectangle(rect.Width, rect.Top - thickness.Top, thickness.Right, rect.Height + thickness.Top) :
			new Rectangle(rect.Width - thickness.Right, rect.Top, thickness.Right, rect.Height - thickness.Bottom);

		var rightColor = new ColorQuad(
			color.Color.TopRight,
			color.Color.BottomRight,
			color.Color.BottomRight,
			color.Color.TopRight);

		var bottomRect = towardsOutside ?
			new Rectangle(rect.Left, rect.Height, rect.Width + thickness.Right, thickness.Bottom) :
			new Rectangle(rect.Left + thickness.Left, rect.Height - thickness.Bottom, rect.Width - thickness.Left, thickness.Bottom);

		var bottomColor = new ColorQuad(
			color.Color.BottomLeft,
			color.Color.BottomLeft,
			color.Color.BottomRight,
			color.Color.BottomRight);

		var leftRect = towardsOutside ?
			new Rectangle(rect.Left - thickness.Left, rect.Top, thickness.Left, rect.Height + thickness.Bottom) :
			new Rectangle(rect.Left, rect.Top + thickness.Top, thickness.Left, rect.Height - thickness.Bottom);

		var leftColor = new ColorQuad(
			color.Color.TopLeft,
			color.Color.BottomLeft,
			color.Color.BottomLeft,
			color.Color.TopLeft);

		renderer.DrawQuad(texture, Quad.FromRectangle(topRect) * drawMatrix, new DrawColorInfo(topColor));
		renderer.DrawQuad(texture, Quad.FromRectangle(rightRect) * drawMatrix, new DrawColorInfo(rightColor));
		renderer.DrawQuad(texture, Quad.FromRectangle(bottomRect) * drawMatrix, new DrawColorInfo(bottomColor));
		renderer.DrawQuad(texture, Quad.FromRectangle(leftRect) * drawMatrix, new DrawColorInfo(leftColor));
	}
}
