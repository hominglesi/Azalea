namespace Azalea.Platform.Rendering;
public static class PlatformRenderer_Extentions
{
	public static void CompileShader(this PlatformRenderer renderer, Shader shader)
		=> renderer.Enqueue(CompileShaderCommand.Borrow(shader));

	public static void DisplayShaderCompileStatus(this PlatformRenderer renderer, Shader shader)
		=> renderer.Enqueue(DisplayShaderCompileStatusCommand.Borrow(shader));

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

	public static void ShaderSource(this PlatformRenderer renderer, Shader shader, string sourceCode)
		=> renderer.Enqueue(ShaderSourceCommand.Borrow(shader, sourceCode));
}
