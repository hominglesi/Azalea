using Azalea.Graphics.OpenGL;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Shaders;
using Azalea.Graphics.Sprites;
using Azalea.IO.Resources;
using System.Numerics;

namespace Azalea.Design.Shapes;
public class Circle : Sprite
{
	internal static IShader CircleShader;

	static Circle()
	{
		var circleVertexShader = Assets.GetText("Shaders/circle_vertexShader.glsl")!;
		var circleFragmentShader = Assets.GetText("Shaders/circle_fragmentShader.glsl")!;
		CircleShader = new GLShader(circleVertexShader, circleFragmentShader);
	}

	public Circle()
	{
		Shader = CircleShader;
	}

	public override void Draw(IRenderer renderer)
	{
		SetupShader();

		base.Draw(renderer);
	}

	public static void SetupShader()
	{
		var clientSize = AzaleaGame.Main.Host.Window.ClientSize;
		var projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(0, clientSize.X, clientSize.Y, 0, 0.1f, 100);
		CircleShader.SetUniform("u_Projection", projectionMatrix);
		CircleShader.SetUniform("u_Texture", 0);

		//VERY DUMB WORKAROUND!!!!
		AzaleaGame.Main.Host.Renderer.QuadShader.Bind();
	}
}
