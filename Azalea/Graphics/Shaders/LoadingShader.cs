using Azalea.Graphics.Colors;
using Azalea.IO.Resources;

namespace Azalea.Graphics.Shaders;

public class LoadingShader : Shader
{
	public Color BackgroundColor { get; set; } = Palette.White;
	public Color RingColor { get; set; } = new Color(190);
	public Color CircleColor { get; set; } = new Color(57, 94, 163);

	public LoadingShader() : base(
			Assets.GetText("Shaders/DefaultQuadVertex.glsl")!,
			Assets.GetText("Shaders/LoadingFragment.glsl")!)
	{ }
}
