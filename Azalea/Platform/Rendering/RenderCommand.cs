using Azalea.Graphics.Colors;
using Azalea.Numerics;
using Azalea.Threading;
using System.Buffers;
using System.Numerics;

namespace Azalea.Platform.Rendering;
public abstract class RenderCommand : ThreadCommand
{
	internal static volatile new int TotalCreated = 0;

	internal RenderCommand()
	{
		TotalCreated++;
	}
}

[ThreadCommand]
internal partial class AttachShaderCommand : RenderCommand
{
	public Program Program;
	public Shader Shader;
}

[ThreadCommand]
internal partial class BindBufferCommand : RenderCommand
{
	public int Type;
	public Buffer? Buffer;
}

[ThreadCommand]
internal partial class BindTextureCommand : RenderCommand
{
	public int Type;
	public Texture Texture;
}

[ThreadCommand]
internal partial class BindVertexArrayCommand : RenderCommand
{
	public VertexArray? VertexArray;

	public override string ToString()
		=> $"BindVertexArray({(VertexArray is null ? 0 : VertexArray.Handle)})";
}

[ThreadCommand]
internal partial class BlendFunctionCommand : RenderCommand
{
	public int SourceFactor;
	public int DestinationFactor;
}

[ThreadCommand]
internal partial class BufferDataCommand : RenderCommand
{
	public int Type;
	public nint Size;
	public byte[]? Data;
	public int Hint;
	public bool FreeData;

	protected override void Cleanup()
	{
		if (FreeData && Data is not null)
			ArrayPool<byte>.Shared.Return(Data);
	}
}

[ThreadCommand(displayName: "BufferData")]
internal partial class BufferDataFloatCommand : RenderCommand
{
	public int Type;
	public nint Size;
	public float[]? Data;
	public int Hint;
	public bool FreeData;

	protected override void Cleanup()
	{
		if (FreeData && Data is not null)
			ArrayPool<float>.Shared.Return(Data);
	}
}

[ThreadCommand(displayName: "BufferData")]
internal partial class BufferDataUIntCommand : RenderCommand
{
	public int Type;
	public nint Size;
	public uint[]? Data;
	public int Hint;
	public bool FreeData;

	protected override void Cleanup()
	{
		if (FreeData && Data is not null)
			ArrayPool<uint>.Shared.Return(Data);
	}
}

[ThreadCommand]
internal partial class ClearCommand : RenderCommand
{
	public Color Color;
}

[ThreadCommand]
internal partial class CompileShaderCommand : RenderCommand
{
	public Shader Shader;
}

[ThreadCommand]
internal partial class DeleteShaderCommand : RenderCommand
{
	public Shader Shader;
}

[ThreadCommand]
internal partial class DisableCommand : RenderCommand
{
	public int Capability;
}

[ThreadCommand]
internal partial class DrawArraysCommand : RenderCommand
{
	public int Mode;
	public int First;
	public int Count;
}

[ThreadCommand]
internal partial class DrawElementsCommand : RenderCommand
{
	public int Mode;
	public int Count;
	public int Type;
	public int Offset;

	public override string ToString() => $"DrawElements({Count})";
}

[ThreadCommand]
internal partial class EnableCommand : RenderCommand
{
	public int Capability;
}

[ThreadCommand]
internal partial class EnableVertexAttribArrayCommand : RenderCommand
{
	public uint Index;
}

[ThreadCommand]
internal partial class FramebufferTexture2DCommand : RenderCommand
{
	public Framebuffer Framebuffer;
	public Texture Texture;
	public int Target;
	public int Attachment;
	public int Textarget;
	public int Level;
}

[ThreadCommand(generateHandler: false)]
internal partial class GenerateBufferCommand : RenderCommand
{
	public Buffer Buffer;
}

internal static class GenerateBufferComand_Handler
{
	public static Buffer GenerateBuffer(this ICommandHandler<RenderCommand> consumer)
	{
		var buffer = new Buffer();
		consumer.Enqueue(GenerateBufferCommand.Borrow(buffer));
		return buffer;
	}
}

[ThreadCommand(generateHandler: false)]
internal partial class GenerateFramebufferCommand : RenderCommand
{
	public Framebuffer Framebuffer;
}

internal static class GenerateFramebufferCommand_Handler
{
	public static Framebuffer GenerateFramebuffer(this ICommandHandler<RenderCommand> consumer)
	{
		var framebuffer = new Framebuffer();
		consumer.Enqueue(GenerateFramebufferCommand.Borrow(framebuffer));
		return framebuffer;
	}
}

[ThreadCommand]
internal partial class GenerateMipmapCommand : RenderCommand
{
	public int Target;
}

[ThreadCommand(generateHandler: false)]
internal partial class GenerateProgramCommand : RenderCommand
{
	public Program Program;
}

internal static class GenerateProgramCommand_Handler
{
	public static Program GenerateProgram(this ICommandHandler<RenderCommand> consumer)
	{
		var program = new Program();
		consumer.Enqueue(GenerateProgramCommand.Borrow(program));
		return program;
	}
}

[ThreadCommand(generateHandler: false)]
internal partial class GenerateShaderCommand : RenderCommand
{
	public Shader Shader;
	public int ShaderType;
}

internal static class GenerateShaderCommand_Handler
{
	public static Shader GenerateShader(this ICommandHandler<RenderCommand> consumer, int type)
	{
		var shader = new Shader();
		consumer.Enqueue(GenerateShaderCommand.Borrow(shader, type));
		return shader;
	}
}

[ThreadCommand(generateHandler: false)]
internal partial class GenerateTextureCommand : RenderCommand
{
	public Texture Texture;
}

internal static class GenerateTextureCommand_Handler
{
	public static Texture GenerateTexture(this ICommandHandler<RenderCommand> consumer)
	{
		var texture = new Texture();
		consumer.Enqueue(GenerateTextureCommand.Borrow(texture));
		return texture;
	}
}

[ThreadCommand(generateHandler: false)]
internal partial class GenerateVertexArrayCommand : RenderCommand
{
	public VertexArray VertexArray;
}

internal static class GenerateVertexArrayCommand_Handler
{
	public static VertexArray GenerateVertexArray(this ICommandHandler<RenderCommand> consumer)
	{
		var vertexArray = new VertexArray();
		consumer.Enqueue(GenerateVertexArrayCommand.Borrow(vertexArray));
		return vertexArray;
	}
}

[ThreadCommand(generateHandler: false)]
internal partial class GetUniformLocationCommand : RenderCommand
{
	public UniformLocation UniformLocation;
	public Program Program;
	public string Name;
}

internal static class GetUniformLocationCommand_Handler
{
	public static UniformLocation GetUniformLocation(this ICommandHandler<RenderCommand> consumer, Program program, string name)
	{
		var uniformLocation = new UniformLocation();
		consumer.Enqueue(GetUniformLocationCommand.Borrow(uniformLocation, program, name));
		return uniformLocation;
	}
}

[ThreadCommand]
internal partial class LinkProgramCommand : RenderCommand
{
	public Program Program;
}

[ThreadCommand]
internal partial class PolygonModeCommand : RenderCommand
{
	public int Face;
	public int Mode;
}

[ThreadCommand]
internal partial class PrepareRenderingCommand : RenderCommand { }

[ThreadCommand]
internal partial class PrintErrorsCommand : RenderCommand { }

[ThreadCommand]
internal partial class PrintProgramCompileStatusCommand : RenderCommand
{
	public Program Program;
}

[ThreadCommand]
internal partial class PrintShaderCompileStatusCommand : RenderCommand
{
	public Shader Shader;
}

[ThreadCommand]
internal partial class ScissorCommand : RenderCommand
{
	public RectangleInt? Rectangle;

	public override string ToString()
		=> Rectangle is null ? $"Scissor(0)" : $"Scissor({Rectangle})";
}

[ThreadCommand]
internal partial class ShaderSourceCommand : RenderCommand
{
	public Shader Shader;
	public string SourceCode;
}

[ThreadCommand]
internal partial class SwapBuffersCommand : RenderCommand { }

[ThreadCommand]
internal partial class TexImage2DCommand : RenderCommand
{
	public Texture Texture;
	public int Width;
	public int Height;
	public byte[]? Pixels;
	public bool GenerateMipmap;
}

[ThreadCommand]
internal partial class TexParameteriCommand : RenderCommand
{
	public Texture Texture;
	public int Target;
	public int Parameter;
	public int Value;
}

[ThreadCommand]
internal partial class Uniform1iCommand : RenderCommand
{
	public UniformLocation UniformLocation;
	public int Int1;
}

[ThreadCommand]
internal partial class Uniform4fCommand : RenderCommand
{
	public UniformLocation UniformLocation;
	public float Value0;
	public float Value1;
	public float Value2;
	public float Value3;
}

[ThreadCommand]
internal partial class UniformMatrix4fvCommand : RenderCommand
{
	public UniformLocation UniformLocation;
	public int Count;
	public bool Transpose;
	public Matrix4x4 Value;
}

[ThreadCommand]
internal partial class UseProgramCommand : RenderCommand
{
	public Program Program;

	public override string ToString() => $"UseProgram({Program.Handle})";
}

[ThreadCommand]
internal partial class VertexAttribPointerCommand : RenderCommand
{
	public uint Index;
	public int Size;
	public int Type;
	public bool Normalized;
	public int Stride;
	public nint Pointer;
}


