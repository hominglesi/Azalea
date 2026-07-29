using Azalea.Threading;

namespace Azalea.Platform.Rendering;
public abstract partial class PlatformRenderer : IRenderCommandConsumer
{
	public void AttachShader(Program program, Shader shader, ICommandGroup? commandGroup = null)
		=> Thread.Enqueue(AttachShaderCommand.Borrow(program, shader), commandGroup);

	public void BlendFunction(int sourceFactor, int destinationFactor, ICommandGroup? commandGroup = null)
		=> Thread.Enqueue(BlendFunctionCommand.Borrow(sourceFactor, destinationFactor), commandGroup);

	public void CompileShader(Shader shader, ICommandGroup? commandGroup = null)
		=> Thread.Enqueue(CompileShaderCommand.Borrow(shader), commandGroup);

	public void DeleteShader(Shader shader, ICommandGroup? commandGroup = null)
		=> Thread.Enqueue(DeleteShaderCommand.Borrow(shader), commandGroup);

	public void Disable(int capability, ICommandGroup? commandGroup = null)
		=> Thread.Enqueue(DisableCommand.Borrow(capability), commandGroup);

	public void Enable(int capability, ICommandGroup? commandGroup = null)
		=> Thread.Enqueue(EnableCommand.Borrow(capability), commandGroup);

	public void EnableVertexAttribArray(uint index, ICommandGroup? commandGroup = null)
		=> Thread.Enqueue(EnableVertexAttribArrayCommand.Borrow(index), commandGroup);

	public Buffer GenerateBuffer(ICommandGroup? commandGroup = null)
	{
		var buffer = new Buffer();
		Thread.Enqueue(GenerateBufferCommand.Borrow(buffer), commandGroup);
		return buffer;
	}

	public Framebuffer GenerateFramebuffer(ICommandGroup? commandGroup = null)
	{
		var framebuffer = new Framebuffer();
		Thread.Enqueue(GenerateFramebufferCommand.Borrow(framebuffer), commandGroup);
		return framebuffer;
	}

	public void GenerateMipmap(int target, ICommandGroup? commandGroup = null)
		=> Thread.Enqueue(GenerateMipmapCommand.Borrow(target), commandGroup);

	public Program GenerateProgram(ICommandGroup? commandGroup = null)
	{
		var program = new Program();
		Thread.Enqueue(GenerateProgramCommand.Borrow(program), commandGroup);
		return program;
	}

	public Shader GenerateShader(int type, ICommandGroup? commandGroup = null)
	{
		var shader = new Shader();
		Thread.Enqueue(GenerateShaderCommand.Borrow(shader, type), commandGroup);
		return shader;
	}

	public Texture GenerateTexture(ICommandGroup? commandGroup = null)
	{
		var texture = new Texture();
		Thread.Enqueue(GenerateTextureCommand.Borrow(texture), commandGroup);
		return texture;
	}

	public VertexArray GenerateVertexArray(ICommandGroup? commandGroup = null)
	{
		var vertexArray = new VertexArray();
		Thread.Enqueue(GenerateVertexArrayCommand.Borrow(vertexArray), commandGroup);
		return vertexArray;
	}

	public UniformLocation GetUniformLocation(Program program, string name, ICommandGroup? commandGroup = null)
	{
		var uniformLocation = new UniformLocation();
		Thread.Enqueue(GetUniformLocationCommand.Borrow(uniformLocation, program, name), commandGroup);
		return uniformLocation;
	}

	public void LinkProgram(Program program, ICommandGroup? commandGroup = null)
		=> Thread.Enqueue(LinkProgramCommand.Borrow(program), commandGroup);

	public void PolygonMode(int face, int mode, ICommandGroup? commandGroup = null)
		=> Thread.Enqueue(PolygonModeCommand.Borrow(face, mode), commandGroup);

	public void PrintProgramCompileStatus(Program program, ICommandGroup? commandGroup = null)
		=> Thread.Enqueue(PrintProgramCompileStatusCommand.Borrow(program), commandGroup);

	public void PrintShaderCompileStatus(Shader shader, ICommandGroup? commandGroup = null)
		=> Thread.Enqueue(PrintShaderCompileStatusCommand.Borrow(shader), commandGroup);

	public void ShaderSource(Shader shader, string sourceCode, ICommandGroup? commandGroup = null)
		=> Thread.Enqueue(ShaderSourceCommand.Borrow(shader, sourceCode), commandGroup);

	public void TexImage2D(int target, int level, int internalFormat, int width, int height, int border, int format, int type, byte[]? pixels, ICommandGroup? commandGroup = null)
		=> Thread.Enqueue(TexImage2DCommand.Borrow(target, level, internalFormat, width, height, border, format, type, pixels), commandGroup);

	public void VertexAttribPointer(uint index, int size, int type, bool normalized, int stride, nint pointer, ICommandGroup? commandGroup = null)
		=> Thread.Enqueue(VertexAttribPointerCommand.Borrow(index, size, type, normalized, stride, pointer), commandGroup);
}
