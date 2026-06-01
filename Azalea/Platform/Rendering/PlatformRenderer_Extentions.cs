namespace Azalea.Platform.Rendering;
public static class PlatformRenderer_Extentions
{
	public static void AttachShader(this PlatformRenderer renderer, Program program, Shader shader)
		=> renderer.Enqueue(AttachShaderCommand.Borrow(program, shader));

	public static void CompileShader(this PlatformRenderer renderer, Shader shader)
		=> renderer.Enqueue(CompileShaderCommand.Borrow(shader));

	public static void DeleteShader(this PlatformRenderer renderer, Shader shader)
		=> renderer.Enqueue(DeleteShaderCommand.Borrow(shader));

	public static void Disable(this PlatformRenderer renderer, int capability)
		=> renderer.Enqueue(DisableCommand.Borrow(capability));

	public static void Enable(this PlatformRenderer renderer, int capability)
		=> renderer.Enqueue(EnableCommand.Borrow(capability));

	public static void EnableVertexAttribArray(this PlatformRenderer renderer, uint index)
		=> renderer.Enqueue(EnableVertexAttribArrayCommand.Borrow(index));

	public static Buffer GenerateBuffer(this PlatformRenderer renderer)
	{
		var buffer = new Buffer();
		renderer.Enqueue(GenerateBufferCommand.Borrow(buffer));
		return buffer;
	}

	public static Framebuffer GenerateFramebuffer(this PlatformRenderer renderer)
	{
		var framebuffer = new Framebuffer();
		renderer.Enqueue(GenerateFramebufferCommand.Borrow(framebuffer));
		return framebuffer;
	}

	public static Program GenerateProgram(this PlatformRenderer renderer)
	{
		var program = new Program();
		renderer.Enqueue(GenerateProgramCommand.Borrow(program));
		return program;
	}

	public static Shader GenerateShader(this PlatformRenderer renderer, int type)
	{
		var shader = new Shader();
		renderer.Enqueue(GenerateShaderCommand.Borrow(shader, type));
		return shader;
	}

	public static Texture GenerateTexture(this PlatformRenderer renderer)
	{
		var texture = new Texture();
		renderer.Enqueue(GenerateTextureCommand.Borrow(texture));
		return texture;
	}

	public static VertexArray GenerateVertexArray(this PlatformRenderer renderer)
	{
		var vertexArray = new VertexArray();
		renderer.Enqueue(GenerateVertexArrayCommand.Borrow(vertexArray));
		return vertexArray;
	}

	public static void LinkProgram(this PlatformRenderer renderer, Program program)
		=> renderer.Enqueue(LinkProgramCommand.Borrow(program));

	public static void PrintProgramCompileStatus(this PlatformRenderer renderer, Program program)
		=> renderer.Enqueue(PrintProgramCompileStatusCommand.Borrow(program));

	public static void PrintShaderCompileStatus(this PlatformRenderer renderer, Shader shader)
		=> renderer.Enqueue(PrintShaderCompileStatusCommand.Borrow(shader));

	public static void ShaderSource(this PlatformRenderer renderer, Shader shader, string sourceCode)
		=> renderer.Enqueue(ShaderSourceCommand.Borrow(shader, sourceCode));

	public static void VertexAttribPointer(this PlatformRenderer renderer, uint index, int size, int type, bool normalized, int stride, nint pointer)
		=> renderer.Enqueue(VertexAttribPointerCommand.Borrow(index, size, type, normalized, stride, pointer));
}
