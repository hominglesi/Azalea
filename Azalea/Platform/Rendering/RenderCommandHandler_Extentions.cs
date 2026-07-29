using Azalea.Graphics.Colors;
using Azalea.Numerics;
using Azalea.Threading;
using System.Numerics;

namespace Azalea.Platform.Rendering;
public static class RenderCommandHandler_Extentions
{
	public static void Clear(this ICommandHandler<RenderCommand> queue, Color color)
		=> queue.Enqueue(ClearCommand.Borrow(color));

	public static void DrawArrays(this ICommandHandler<RenderCommand> queue, int mode, int first, int count)
		=> queue.Enqueue(DrawArraysCommand.Borrow(mode, first, count));

	public static void DrawElements(this ICommandHandler<RenderCommand> queue, int mode, int count, int type, int offset)
		=> queue.Enqueue(DrawElementsCommand.Borrow(mode, count, type, offset));

	public static void PrepareRendering(this ICommandHandler<RenderCommand> queue)
		=> queue.Enqueue(PrepareRenderingCommand.Borrow());

	public static void Scissor(this ICommandHandler<RenderCommand> queue, RectangleInt? rectangle)
		=> queue.Enqueue(ScissorCommand.Borrow(rectangle));

	public static void SwapBuffers(this ICommandHandler<RenderCommand> queue)
		=> queue.Enqueue(SwapBuffersCommand.Borrow());

	public static void BufferData(this ICommandHandler<RenderCommand> consumer, int type, nint size, byte[]? data, int hint, bool freeData)
		=> consumer.Enqueue(BufferDataCommand.Borrow(type, size, data, hint, freeData));

	public static void BufferData(this ICommandHandler<RenderCommand> consumer, int type, nint size, float[]? data, int hint, bool freeData)
		=> consumer.Enqueue(BufferDataFloatCommand.Borrow(type, size, data, hint, freeData));

	public static void BufferData(this ICommandHandler<RenderCommand> consumer, int type, nint size, uint[]? data, int hint, bool freeData)
		=> consumer.Enqueue(BufferDataUIntCommand.Borrow(type, size, data, hint, freeData));

	public static void BindBuffer(this ICommandHandler<RenderCommand> consumer, int type, Buffer? buffer)
		=> consumer.Enqueue(BindBufferCommand.Borrow(type, buffer));

	public static void BindTexture(this ICommandHandler<RenderCommand> consumer, int type, Texture texture)
		=> consumer.Enqueue(BindTextureCommand.Borrow(type, texture));

	public static void BindVertexArray(this ICommandHandler<RenderCommand> consumer, VertexArray? vertexArray)
		=> consumer.Enqueue(BindVertexArrayCommand.Borrow(vertexArray));

	public static void PrintErrors(this ICommandHandler<RenderCommand> consumer)
		=> consumer.Enqueue(PrintErrorsCommand.Borrow());

	public static void Uniform1i(this ICommandHandler<RenderCommand> consumer, UniformLocation uniformLocation, int int0)
		=> consumer.Enqueue(Uniform1iCommand.Borrow(uniformLocation, int0));

	public static void Uniform4f(this ICommandHandler<RenderCommand> consumer, UniformLocation uniformLocation, float float0, float float1, float float2, float float3)
		=> consumer.Enqueue(Uniform4fCommand.Borrow(uniformLocation, float0, float1, float2, float3));

	public static void UniformMatrix4fv(this ICommandHandler<RenderCommand> consumer, UniformLocation uniformLocation, int count, bool transpose, Matrix4x4 matrix)
		=> consumer.Enqueue(UniformMatrix4fvCommand.Borrow(uniformLocation, count, transpose, matrix));

	public static void UseProgram(this ICommandHandler<RenderCommand> consumer, Program program)
		=> consumer.Enqueue(UseProgramCommand.Borrow(program));

	public static void AttachShader(this ICommandHandler<RenderCommand> consumer, Program program, Shader shader)
		=> consumer.Enqueue(AttachShaderCommand.Borrow(program, shader));

	public static void BlendFunction(this ICommandHandler<RenderCommand> consumer, int sourceFactor, int destinationFactor)
		=> consumer.Enqueue(BlendFunctionCommand.Borrow(sourceFactor, destinationFactor));

	public static void CompileShader(this ICommandHandler<RenderCommand> consumer, Shader shader)
		=> consumer.Enqueue(CompileShaderCommand.Borrow(shader));

	public static void DeleteShader(this ICommandHandler<RenderCommand> consumer, Shader shader)
		=> consumer.Enqueue(DeleteShaderCommand.Borrow(shader));

	public static void Disable(this ICommandHandler<RenderCommand> consumer, int capability)
		=> consumer.Enqueue(DisableCommand.Borrow(capability));

	public static void Enable(this ICommandHandler<RenderCommand> consumer, int capability)
		=> consumer.Enqueue(EnableCommand.Borrow(capability));

	public static void EnableVertexAttribArray(this ICommandHandler<RenderCommand> consumer, uint index)
		=> consumer.Enqueue(EnableVertexAttribArrayCommand.Borrow(index));

	public static Buffer GenerateBuffer(this ICommandHandler<RenderCommand> consumer)
	{
		var buffer = new Buffer();
		consumer.Enqueue(GenerateBufferCommand.Borrow(buffer));
		return buffer;
	}

	public static Framebuffer GenerateFramebuffer(this ICommandHandler<RenderCommand> consumer)
	{
		var framebuffer = new Framebuffer();
		consumer.Enqueue(GenerateFramebufferCommand.Borrow(framebuffer));
		return framebuffer;
	}

	public static void GenerateMipmap(this ICommandHandler<RenderCommand> consumer, int target)
		=> consumer.Enqueue(GenerateMipmapCommand.Borrow(target));

	public static Program GenerateProgram(this ICommandHandler<RenderCommand> consumer)
	{
		var program = new Program();
		consumer.Enqueue(GenerateProgramCommand.Borrow(program));
		return program;
	}

	public static Shader GenerateShader(this ICommandHandler<RenderCommand> consumer, int type)
	{
		var shader = new Shader();
		consumer.Enqueue(GenerateShaderCommand.Borrow(shader, type));
		return shader;
	}

	public static Texture GenerateTexture(this ICommandHandler<RenderCommand> consumer)
	{
		var texture = new Texture();
		consumer.Enqueue(GenerateTextureCommand.Borrow(texture));
		return texture;
	}

	public static VertexArray GenerateVertexArray(this ICommandHandler<RenderCommand> consumer)
	{
		var vertexArray = new VertexArray();
		consumer.Enqueue(GenerateVertexArrayCommand.Borrow(vertexArray));
		return vertexArray;
	}

	public static UniformLocation GetUniformLocation(this ICommandHandler<RenderCommand> consumer, Program program, string name)
	{
		var uniformLocation = new UniformLocation();
		consumer.Enqueue(GetUniformLocationCommand.Borrow(uniformLocation, program, name));
		return uniformLocation;
	}

	public static void LinkProgram(this ICommandHandler<RenderCommand> consumer, Program program)
		=> consumer.Enqueue(LinkProgramCommand.Borrow(program));

	public static void PolygonMode(this ICommandHandler<RenderCommand> consumer, int face, int mode)
		=> consumer.Enqueue(PolygonModeCommand.Borrow(face, mode));

	public static void PrintProgramCompileStatus(this ICommandHandler<RenderCommand> consumer, Program program)
		=> consumer.Enqueue(PrintProgramCompileStatusCommand.Borrow(program));

	public static void PrintShaderCompileStatus(this ICommandHandler<RenderCommand> consumer, Shader shader)
		=> consumer.Enqueue(PrintShaderCompileStatusCommand.Borrow(shader));

	public static void ShaderSource(this ICommandHandler<RenderCommand> consumer, Shader shader, string sourceCode)
		=> consumer.Enqueue(ShaderSourceCommand.Borrow(shader, sourceCode));

	public static void TexImage2D(this ICommandHandler<RenderCommand> consumer, int target, int level, int internalFormat, int width, int height, int border, int format, int type, byte[]? pixels)
		=> consumer.Enqueue(TexImage2DCommand.Borrow(target, level, internalFormat, width, height, border, format, type, pixels));

	public static void VertexAttribPointer(this ICommandHandler<RenderCommand> consumer, uint index, int size, int type, bool normalized, int stride, nint pointer)
		=> consumer.Enqueue(VertexAttribPointerCommand.Borrow(index, size, type, normalized, stride, pointer));
}
