using Azalea.Graphics.Rendering;
using Azalea.Platform.Rendering.OpenGL.LoadingContext;

namespace Azalea.Graphics.Shaders;
public class ShaderBuilder
{
	public static Shader FromShaderCode(string vertexCode, string fragmentCode)
	{
		var shader = Renderer.CreateShader(vertexCode, fragmentCode);

		var newProgram = new Platform.Rendering.Program();
		Platform.Rendering.OpenGL.GLRenderer.LoadingContext!.GenerateProgram(newProgram,
			vertexCode, fragmentCode);

		shader.NewProgram = newProgram;
		return shader;
	}
}
