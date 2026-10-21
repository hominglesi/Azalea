using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Primitives;
using Azalea.IO.Resources;
using Azalea.Numerics;
using Azalea.Platform;
using Azalea.Platform.Rendering.Coordination;

namespace Azalea.Design.Shapes;

public partial class HollowBox : GameObject
{
	public Boundary Thickness { get; set; } = new(3);
	public BorderAlignment Alignment { get; set; }

	public override void Draw(RenderCoordinator coordinator)
	{
		var rect = DrawRectangle;
		var color = DrawColorQuad;

		var topRect = Alignment switch
		{
			BorderAlignment.Outer => new Rectangle(
				rect.Left - Thickness.Left,
				rect.Top - Thickness.Top,
				rect.Width + Thickness.Left,
				Thickness.Top),
			BorderAlignment.Inner => new Rectangle(
				rect.Top,
				rect.Left,
				rect.Width - Thickness.Right,
				Thickness.Top),
			/* BorderAlignment.Center */
			_ => new Rectangle(
				rect.Left - (Thickness.Left / 2),
				rect.Top - (Thickness.Top / 2),
				rect.Width - (Thickness.Right / 2) + (Thickness.Left / 2),
				Thickness.Top),
		};

		var topColor = new ColorQuad(
			color.TopLeft,
			color.TopLeft,
			color.TopRight,
			color.TopRight);

		var rightRect = Alignment switch
		{
			BorderAlignment.Outer => new Rectangle(
				rect.Width,
				rect.Top - Thickness.Top,
				Thickness.Right,
				rect.Height + Thickness.Top),
			BorderAlignment.Inner => new Rectangle(
				rect.Width - Thickness.Right,
				rect.Top,
				Thickness.Right,
				rect.Height - Thickness.Bottom),
			/* BorderAlignment.Center */
			_ => new Rectangle(
				rect.Width - (Thickness.Right / 2),
				rect.Top - (Thickness.Top / 2),
				Thickness.Right,
				rect.Height - (Thickness.Bottom / 2) + (Thickness.Top / 2)),
		};

		var rightColor = new ColorQuad(
			color.TopRight,
			color.BottomRight,
			color.BottomRight,
			color.TopRight);

		var bottomRect = Alignment switch
		{
			BorderAlignment.Outer => new Rectangle(
				rect.Left,
				rect.Height,
				rect.Width + Thickness.Right,
				Thickness.Bottom),
			BorderAlignment.Inner => new Rectangle(
				rect.Left + Thickness.Left,
				rect.Height - Thickness.Bottom,
				rect.Width - Thickness.Left,
				Thickness.Bottom),
			/* BorderAlignment.Center */
			_ => new Rectangle(
				rect.Left + (Thickness.Left / 2),
				rect.Height - (Thickness.Bottom / 2),
				rect.Width - (Thickness.Left / 2) + (Thickness.Right / 2),
				Thickness.Bottom),
		};

		var bottomColor = new ColorQuad(
			color.BottomLeft,
			color.BottomLeft,
			color.BottomRight,
			color.BottomRight);

		var leftRect = Alignment switch
		{
			BorderAlignment.Outer => new Rectangle(
				rect.Left - Thickness.Left,
				rect.Top,
				Thickness.Left,
				rect.Height + Thickness.Bottom),
			BorderAlignment.Inner => new Rectangle(
				rect.Left,
				rect.Top + Thickness.Top,
				Thickness.Left,
				rect.Height - Thickness.Top),
			/* BorderAlignment.Center */
			_ => new Rectangle(
				rect.Left - (Thickness.Left / 2),
				rect.Top + (Thickness.Top / 2),
				Thickness.Left,
				rect.Height + (Thickness.Bottom / 2) - (Thickness.Top / 2)),
		};

		var leftColor = new ColorQuad(
			color.TopLeft,
			color.BottomLeft,
			color.BottomLeft,
			color.TopLeft);

		coordinator.BindTexture(Assets.WhitePixelNative);
		coordinator.BindShader(GameHost.Instance.Loader.DefaultQuadShader);

		coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, Quad.FromRectangle(topRect) * DrawInfo.Matrix, topColor, Rectangle.One);
		coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, Quad.FromRectangle(rightRect) * DrawInfo.Matrix, rightColor, Rectangle.One);
		coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, Quad.FromRectangle(bottomRect) * DrawInfo.Matrix, bottomColor, Rectangle.One);
		coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, Quad.FromRectangle(leftRect) * DrawInfo.Matrix, leftColor, Rectangle.One);
	}
}
