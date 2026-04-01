using Azalea.Graphics.Rendering;

namespace Azalea.Graphics.Shaders;
public class ShaderBuilder
{
	public static Shader FromShaderCode(string vertexCode, string fragmentCode)
		=> Renderer.CreateShader(vertexCode, fragmentCode);
}
