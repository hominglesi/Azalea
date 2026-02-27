using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Rendering;
using Azalea.Layout;

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

	public virtual void DrawBackground(IRenderer renderer)
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

	public virtual void DrawForeground(IRenderer renderer)
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

		renderer.DrawRectangle(DrawRectangle, DrawInfo.Matrix, BorderThickness,
				_borderDrawColorInfo, BorderAlignment);
	}
}


