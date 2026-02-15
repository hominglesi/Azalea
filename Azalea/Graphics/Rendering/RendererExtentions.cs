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

	public static void DrawQuad(this IRenderer renderer, INativeTexture texture, Quad vertexQuad, DrawColorInfo drawColorInfo, Rectangle? textureRegion = null)
	{
		if (ClientContainsQuad(vertexQuad) == false)
			return;

		renderer.BindTexture(texture);

		var textureUV = textureRegion ?? Rectangle.One;
		var vertexAction = renderer.DefaultQuadBatch.AddAction;

		vertexAction(new TexturedVertex2D
		{
			Position = vertexQuad.BottomLeft,
			Color = drawColorInfo.Color.BottomLeft,
			TexturePosition = textureUV.BottomLeft
		});

		vertexAction(new TexturedVertex2D
		{
			Position = vertexQuad.BottomRight,
			Color = drawColorInfo.Color.BottomRight,
			TexturePosition = textureUV.BottomRight
		});

		vertexAction(new TexturedVertex2D
		{
			Position = vertexQuad.TopRight,
			Color = drawColorInfo.Color.TopRight,
			TexturePosition = textureUV.TopRight
		});

		vertexAction(new TexturedVertex2D
		{
			Position = vertexQuad.TopLeft,
			Color = drawColorInfo.Color.TopLeft,
			TexturePosition = textureUV.TopLeft
		});
	}

	public static void DrawRectangle(this IRenderer renderer, Rectangle rect, Matrix3 drawMatrix, Boundary thickness, DrawColorInfo color, BorderAlignment alignment)
	{
		var whitePixelTexture = renderer.WhitePixel.GetNativeTexture();

		var topRect = alignment switch
		{
			BorderAlignment.Outer => new Rectangle(
				rect.Left - thickness.Left,
				rect.Top - thickness.Top,
				rect.Width + thickness.Left,
				thickness.Top),
			BorderAlignment.Inner => new Rectangle(
				rect.Top,
				rect.Left,
				rect.Width - thickness.Right,
				thickness.Top),
			/* BorderAlignment.Center */
			_ => new Rectangle(
				rect.Left - (thickness.Left / 2),
				rect.Top - (thickness.Top / 2),
				rect.Width - (thickness.Right / 2) + (thickness.Left / 2),
				thickness.Top),
		};

		var topColor = new ColorQuad(
			color.Color.TopLeft,
			color.Color.TopLeft,
			color.Color.TopRight,
			color.Color.TopRight);

		var rightRect = alignment switch
		{
			BorderAlignment.Outer => new Rectangle(
				rect.Width,
				rect.Top - thickness.Top,
				thickness.Right,
				rect.Height + thickness.Top),
			BorderAlignment.Inner => new Rectangle(
				rect.Width - thickness.Right,
				rect.Top,
				thickness.Right,
				rect.Height - thickness.Bottom),
			/* BorderAlignment.Center */
			_ => new Rectangle(
				rect.Width - (thickness.Right / 2),
				rect.Top - (thickness.Top / 2),
				thickness.Right,
				rect.Height - (thickness.Bottom / 2) + (thickness.Top / 2)),
		};

		var rightColor = new ColorQuad(
			color.Color.TopRight,
			color.Color.BottomRight,
			color.Color.BottomRight,
			color.Color.TopRight);

		var bottomRect = alignment switch
		{
			BorderAlignment.Outer => new Rectangle(
				rect.Left,
				rect.Height,
				rect.Width + thickness.Right,
				thickness.Bottom),
			BorderAlignment.Inner => new Rectangle(
				rect.Left + thickness.Left,
				rect.Height - thickness.Bottom,
				rect.Width - thickness.Left,
				thickness.Bottom),
			/* BorderAlignment.Center */
			_ => new Rectangle(
				rect.Left + (thickness.Left / 2),
				rect.Height - (thickness.Bottom / 2),
				rect.Width - (thickness.Left / 2) + (thickness.Right / 2),
				thickness.Bottom),
		};

		var bottomColor = new ColorQuad(
			color.Color.BottomLeft,
			color.Color.BottomLeft,
			color.Color.BottomRight,
			color.Color.BottomRight);

		var leftRect = alignment switch
		{
			BorderAlignment.Outer => new Rectangle(
				rect.Left - thickness.Left,
				rect.Top,
				thickness.Left,
				rect.Height + thickness.Bottom),
			BorderAlignment.Inner => new Rectangle(
				rect.Left,
				rect.Top + thickness.Top,
				thickness.Left,
				rect.Height - thickness.Top),
			/* BorderAlignment.Center */
			_ => new Rectangle(
				rect.Left - (thickness.Left / 2),
				rect.Top + (thickness.Top / 2),
				thickness.Left,
				rect.Height + (thickness.Bottom / 2) - (thickness.Top / 2)),
		};

		var leftColor = new ColorQuad(
			color.Color.TopLeft,
			color.Color.BottomLeft,
			color.Color.BottomLeft,
			color.Color.TopLeft);

		renderer.DrawQuad(whitePixelTexture, Quad.FromRectangle(topRect) * drawMatrix, new DrawColorInfo(topColor));
		renderer.DrawQuad(whitePixelTexture, Quad.FromRectangle(rightRect) * drawMatrix, new DrawColorInfo(rightColor));
		renderer.DrawQuad(whitePixelTexture, Quad.FromRectangle(bottomRect) * drawMatrix, new DrawColorInfo(bottomColor));
		renderer.DrawQuad(whitePixelTexture, Quad.FromRectangle(leftRect) * drawMatrix, new DrawColorInfo(leftColor));
	}
}
