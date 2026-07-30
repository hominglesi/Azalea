using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Primitives;
using Azalea.Graphics.Rendering;
using Azalea.Layout;
using Azalea.Numerics;
using Azalea.Platform.Rendering.Coordination;

namespace Azalea.Design.Containers;
public partial class Composition
{
	private DrawColorInfo _backgroundDrawColorInfo;
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

	public virtual void DrawBackground(IRenderer renderer, RenderCoordinator? coordinator)
	{
		if (BackgroundColor is null)
			return;

		if (_backgroundColorBacking.IsValid == false)
		{
			_backgroundDrawColorInfo = Parent?.DrawColorInfo ?? new DrawColorInfo(null);

			var colorInfo = BackgroundColor.Value;

			if (Alpha != 1) colorInfo = colorInfo.MultiplyAlpha(Alpha);
			if (BackgroundAlpha != 1) colorInfo = colorInfo.MultiplyAlpha(BackgroundAlpha);

			_backgroundDrawColorInfo.Color.ApplyChild(colorInfo);
			_backgroundColorBacking.Validate();
		}

		if (coordinator is not null)
		{
			coordinator.BindTexture(Platform.Rendering.OpenGL.GLRenderer.LoadingContext!.WhitePixel);
			coordinator.BindProgram(coordinator.DefaultQuadProgram);
			coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, ScreenSpaceDrawQuad, _backgroundDrawColorInfo.Color, Rectangle.One);
		}
		else
			renderer.DrawQuad(renderer.WhitePixel.GetNativeTexture(), ScreenSpaceDrawQuad,
				_backgroundDrawColorInfo);
	}

	private DrawColorInfo _borderDrawColorInfo;
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

	public virtual void DrawForeground(IRenderer renderer, RenderCoordinator? coordinator)
	{
		if (BorderColor is null && _borderThickness is null)
			return;

		if (_borderColorBacking.IsValid == false)
		{
			_borderDrawColorInfo = Parent?.DrawColorInfo ?? new DrawColorInfo(null);

			var colorInfo = BorderColor is not null
				? BorderColor.Value
				: ColorQuad.SolidColor(Palette.Black);

			if (Alpha != 1) colorInfo = colorInfo.MultiplyAlpha(Alpha);
			if (BorderAlpha != 1) colorInfo = colorInfo.MultiplyAlpha(BorderAlpha);

			_borderDrawColorInfo.Color.ApplyChild(colorInfo);
			_borderColorBacking.Validate();
		}

		if (coordinator is not null)
		{
			var rect = DrawRectangle;
			var thickness = BorderThickness;
			var color = _borderDrawColorInfo;
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

			coordinator.BindTexture(Platform.Rendering.OpenGL.GLRenderer.LoadingContext!.WhitePixel);
			coordinator.BindProgram(coordinator.DefaultQuadProgram);

			coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, Quad.FromRectangle(topRect) * DrawInfo.Matrix, topColor, Rectangle.One);
			coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, Quad.FromRectangle(rightRect) * DrawInfo.Matrix, rightColor, Rectangle.One);
			coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, Quad.FromRectangle(bottomRect) * DrawInfo.Matrix, bottomColor, Rectangle.One);
			coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, Quad.FromRectangle(leftRect) * DrawInfo.Matrix, leftColor, Rectangle.One);

			return;
		}
		else
			renderer.DrawRectangle(DrawRectangle, DrawInfo.Matrix, BorderThickness,
					_borderDrawColorInfo, BorderAlignment);

	}
}


