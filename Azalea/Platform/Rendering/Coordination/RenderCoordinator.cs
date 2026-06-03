using Azalea.Native.OpenGL;

namespace Azalea.Platform.Rendering.Coordination;
public class RenderCoordinator
{
	public PlatformRenderer Renderer { get; }

	public RenderCoordinator(PlatformRenderer renderer)
	{
		Renderer = renderer;
	}

	public Program CreateStandardProgram(string vertexShaderSource, string fragmentShaderSource)
	{
		var vertexShader = Renderer.GenerateShader(GL.VERTEX_SHADER);
		Renderer.ShaderSource(vertexShader, vertexShaderSource);
		Renderer.CompileShader(vertexShader);
		Renderer.PrintShaderCompileStatus(vertexShader);

		var fragmentShader = Renderer.GenerateShader(GL.FRAGMENT_SHADER);
		Renderer.ShaderSource(fragmentShader, fragmentShaderSource);
		Renderer.CompileShader(fragmentShader);
		Renderer.PrintShaderCompileStatus(fragmentShader);

		var program = Renderer.GenerateProgram();
		Renderer.AttachShader(program, vertexShader);
		Renderer.AttachShader(program, fragmentShader);
		Renderer.LinkProgram(program);
		Renderer.PrintProgramCompileStatus(program);

		Renderer.DeleteShader(vertexShader);
		Renderer.DeleteShader(fragmentShader);

		return program;
	}
}
