using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Primitives;
using Azalea.IO.Resources;
using Azalea.Layout;
using Azalea.Numerics;
using Azalea.Platform;
using Azalea.Platform.Rendering.Coordination;

namespace Azalea.Design.Containers;

public partial class Composition
{
	private ColorQuad _backgroundColorQuad;
	private readonly LayoutValue _backgroundColorBacking = new(Invalidation.Color);

	private ColorQuad? _backgroundColor;
	public ColorQuad? BackgroundColor
	{
		get => _backgroundColor;
		set
		{
			if (_backgroundColor == value) return;
			_backgroundColor = value;
			_backgroundColorBacking.Invalidate();
		}
	}

	private float _backgroundAlpha = 1.0f;
	public float BackgroundAlpha
	{
		get => _backgroundAlpha;
		set
		{
			if (_backgroundAlpha == value) return;
			_backgroundAlpha = value;
			_backgroundColorBacking.Invalidate();
		}
	}

	public virtual void DrawBackground(RenderCoordinator coordinator)
	{
		if (BackgroundColor is null)
			return;

		if (_backgroundColorBacking.IsValid == false)
		{
			_backgroundColorQuad = Parent?.DrawColorQuad ?? ColorQuad.SolidColor(Palette.White);

			var colorInfo = BackgroundColor.Value;

			if (Alpha != 1) colorInfo = colorInfo.MultiplyAlpha(Alpha);
			if (BackgroundAlpha != 1) colorInfo = colorInfo.MultiplyAlpha(BackgroundAlpha);

			_backgroundColorQuad.ApplyChild(colorInfo);
			_backgroundColorBacking.Validate();
		}

		coordinator.BindTexture(Assets.WhitePixelNative);
		coordinator.BindShader(GameHost.Instance.Loader.DefaultQuadShader);
		coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, ScreenSpaceDrawQuad, _backgroundColorQuad, Rectangle.One);
	}

	private ColorQuad _borderColorQuad;
	private readonly LayoutValue _borderColorBacking = new(Invalidation.Color);

	private ColorQuad? _borderColor;
	public ColorQuad? BorderColor
	{
		get => _borderColor;
		set
		{
			if (_borderColor == value) return;
			_borderColor = value;
			_borderColorBacking.Invalidate();
		}
	}

	private float _borderAlpha = 1.0f;
	public float BorderAlpha
	{
		get => _borderAlpha;
		set
		{
			if (_borderAlpha == value) return;
			_borderAlpha = value;
			_borderColorBacking.Invalidate();
		}
	}

	private const float _defaultBorderThickness = 3;
	private Boundary? _borderThickness;
	public Boundary BorderThickness
	{
		get => _borderThickness is not null ? _borderThickness.Value : _defaultBorderThickness;
		set => _borderThickness = value;
	}

	public BorderAlignment BorderAlignment { get; set; } = BorderAlignment.Outer;

	public virtual void DrawForeground(RenderCoordinator coordinator)
	{
		if (BorderColor is null && _borderThickness is null)
			return;

		if (_borderColorBacking.IsValid == false)
		{
			_borderColorQuad = Parent?.DrawColorQuad ?? ColorQuad.SolidColor(Palette.White);

			var colorInfo = BorderColor is not null
				? BorderColor.Value
				: ColorQuad.SolidColor(Palette.Black);

			if (Alpha != 1) colorInfo = colorInfo.MultiplyAlpha(Alpha);
			if (BorderAlpha != 1) colorInfo = colorInfo.MultiplyAlpha(BorderAlpha);

			_borderColorQuad.ApplyChild(colorInfo);
			_borderColorBacking.Validate();
		}

		var rect = DrawRectangle;
		var thickness = BorderThickness;
		var color = _borderColorQuad;
		var alignment = BorderAlignment;

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
			color.TopLeft,
			color.TopLeft,
			color.TopRight,
			color.TopRight);

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
			color.TopRight,
			color.BottomRight,
			color.BottomRight,
			color.TopRight);

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
			color.BottomLeft,
			color.BottomLeft,
			color.BottomRight,
			color.BottomRight);

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


