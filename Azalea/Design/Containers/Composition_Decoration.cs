using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Rendering;

namespace Azalea.Design.Containers;
public partial class Composition
{
	public ColorQuad? BackgroundColor { get; set; }
	public float BackgroundAlpha { get; set; } = 1.0f;

	public virtual void DrawBackground(IRenderer renderer)
	{
		if (BackgroundColor is not null)
		{
			var colorInfo = new DrawColorInfo(BackgroundColor.Value
				.MultiplyAlpha(BackgroundAlpha)
				.MultiplyAlpha(Alpha));

			renderer.DrawQuad(renderer.WhitePixel.GetNativeTexture(), ScreenSpaceDrawQuad, colorInfo);
		}
	}

	public ColorQuad? BorderColor { get; set; }
	public float BorderAlpha { get; set; } = 1.0f;
	public Boundary BorderThickness { get; set; } = 3;
	public BorderAlignment BorderAlignment { get; set; } = BorderAlignment.Outer;

	public virtual void DrawForeground(IRenderer renderer)
	{
		if (BorderColor is not null && BorderThickness != Boundary.Zero)
		{
			var colorInfo = new DrawColorInfo(BorderColor.Value
				.MultiplyAlpha(BorderAlpha)
				.MultiplyAlpha(Alpha));

			renderer.DrawRectangle(DrawRectangle, DrawInfo.Matrix, BorderThickness,
				colorInfo, BorderAlignment);
		}
	}
}


