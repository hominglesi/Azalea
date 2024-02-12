using Azalea.Graphics.Shaders;
using Azalea.Graphics.Sprites;
using Azalea.IO.Resources;

namespace Azalea.Design.Shapes;
public class Circle : Sprite
{
	internal static IShader CircleShader;

	static Circle()
	{
		var circleVertexShader = Assets.GetText("Shaders/quad_vertexShader.glsl")!;
		var circleFragmentShader = Assets.GetText("Shaders/circle_fragmentShader.glsl")!;
		CircleShader = AzaleaGame.Main.Host.Renderer.CreateShader(circleVertexShader, circleFragmentShader);
	}

	public Circle()
	{
		Shader = CircleShader;
	}
}
