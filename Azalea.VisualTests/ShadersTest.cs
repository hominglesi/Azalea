using Azalea.Graphics;
using Azalea.Graphics.Sprites;
using Azalea.IO.Resources;

namespace Azalea.VisualTests;
internal class ShadersTest : TestScene
{
	public ShadersTest()
	{
		var quadVertexShader = Assets.GetText("Shaders/quad_vertexShader.glsl")!;
		var octagramsFragmentShader = Assets.GetText("Shaders/octagrams_fragmentShader.glsl")!;
		var octagons = AzaleaGame.Main.Host.Renderer.CreateShader(quadVertexShader, octagramsFragmentShader);

		Add(new Sprite()
		{
			Anchor = Anchor.Center,
			Origin = Anchor.Center,
			Size = new(800, 450),
			Shader = octagons
		});
	}
}
