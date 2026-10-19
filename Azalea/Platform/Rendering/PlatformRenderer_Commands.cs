using Azalea.Graphics.Colors;
using Azalea.Numerics;
using Azalea.Platform.Rendering.Coordination;
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

[ThreadCommand(generateHandler: true)]
internal partial class BindBufferCommand : RenderCommand
{
	public int Type;
	public Buffer? Buffer;
}

[ThreadCommand(generateHandler: true)]
internal partial class BindBufferRangeCommand : RenderCommand
{
	public int Type;
	public uint Index;
	public Buffer Buffer;
	public nint Offset;
	public nint Size;
}

[ThreadCommand(generateHandler: true)]
internal partial class BindFramebufferCommand : RenderCommand
{
	public int Type;
	public Framebuffer Framebuffer;
}

[ThreadCommand(generateHandler: true)]
internal partial class BindTextureCommand : RenderCommand
{
	public int Type;
	public NativeTexture Texture;
}

[ThreadCommand(generateHandler: true)]
internal partial class BindVertexArrayCommand : RenderCommand
{
	public VertexArray? VertexArray;

	public override string ToString()
		=> $"BindVertexArray({(VertexArray is null ? 0 : VertexArray.Handle)})";
}

[ThreadCommand(generateHandler: true)]
internal partial class BlendFunctionCommand : RenderCommand
{
	public int SourceFactor;
	public int DestinationFactor;
}

[ThreadCommand(generateHandler: true)]
internal partial class BufferDataCommand : RenderCommand
{
	public int Type;
	public nint Size;
	public byte[] Data;
	public int Hint;
	public bool FreeData;

	protected override void Cleanup()
	{
		if (FreeData)
			ArrayPool<byte>.Shared.Return(Data);
	}
}

[ThreadCommand(displayName: "BufferData", generateHandler: true)]
internal partial class BufferDataEmptyCommand : RenderCommand
{
	public int Type;
	public nint Size;
	public int Hint;
}

[ThreadCommand(displayName: "BufferData", generateHandler: true)]
internal partial class BufferDataFloatCommand : RenderCommand
{
	public int Type;
	public nint Size;
	public float[] Data;
	public int Hint;
	public bool FreeData;

	protected override void Cleanup()
	{
		if (FreeData)
			DefaultQuadBatch.ArrayPool.Return(Data);
	}
}

[ThreadCommand(displayName: "BufferData", generateHandler: true)]
internal partial class BufferDataUIntCommand : RenderCommand
{
	public int Type;
	public nint Size;
	public uint[] Data;
	public int Hint;
	public bool FreeData;

	protected override void Cleanup()
	{
		if (FreeData)
			ArrayPool<uint>.Shared.Return(Data);
	}
}

[ThreadCommand(displayName: "BufferSubData", generateHandler: true)]
internal partial class BufferSubDataMatrix4x4Command : RenderCommand
{
	public int Type;
	public nint Offset;
	public Matrix4x4 Data;
}

[ThreadCommand(generateHandler: true)]
internal partial class DisableCommand : RenderCommand
{
	public int Capability;
}

[ThreadCommand(generateHandler: true)]
internal partial class DrawElementsCommand : RenderCommand
{
	public int Mode;
	public int Count;
	public int Type;
	public int Offset;

	public override string ToString() => $"DrawElements({Count})";
}

[ThreadCommand(generateHandler: true)]
internal partial class EnableCommand : RenderCommand
{
	public int Capability;
}

[ThreadCommand(generateHandler: true)]
internal partial class EnableVertexAttribArrayCommand : RenderCommand
{
	public uint Index;
}

[ThreadCommand(generateHandler: true)]
internal partial class FramebufferTexture2DCommand : RenderCommand
{
	public Framebuffer Framebuffer;
	public NativeTexture Texture;
	public int Target;
	public int Attachment;
	public int Textarget;
	public int Level;
}

[ThreadCommand]
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

[ThreadCommand]
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
internal partial class GenerateTextureCommand : RenderCommand
{
	public NativeTexture Texture;
}

internal static class GenerateTextureCommand_Handler
{
	public static NativeTexture GenerateTexture(this ICommandHandler<RenderCommand> consumer)
	{
		var texture = GameHost.Instance.Loader.CreateEmptyTexture();
		consumer.Enqueue(GenerateTextureCommand.Borrow(texture));
		return texture;
	}
}

[ThreadCommand]
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

[ThreadCommand(generateHandler: true)]
internal partial class PolygonModeCommand : RenderCommand
{
	public int Face;
	public int Mode;
}

[ThreadCommand(generateHandler: true)]
internal partial class PrepareRenderingCommand : RenderCommand
{
	public Vector2Int IntendedClientSize;
}

[ThreadCommand(generateHandler: true)]
internal partial class PrintErrorsCommand : RenderCommand { }

[ThreadCommand(generateHandler: true)]
internal partial class ScissorCommand : RenderCommand
{
	public RectangleInt? Rectangle;

	public override string ToString()
		=> Rectangle is null ? $"Scissor(0)" : $"Scissor({Rectangle})";
}


[ThreadCommand(generateHandler: true)]
internal partial class SetClearColorCommand : RenderCommand
{
	public Color Color;
}

[ThreadCommand(generateHandler: true)]
internal partial class SwapBuffersCommand : RenderCommand { }

[ThreadCommand(generateHandler: true)]
internal partial class Uniform1fCommand : RenderCommand
{
	public Program Program;
	public string UniformName;
	public float Float;
}

[ThreadCommand(generateHandler: true)]
internal partial class Uniform2fCommand : RenderCommand
{
	public Program Program;
	public string UniformName;
	public float Float1;
	public float Float2;
}

[ThreadCommand(generateHandler: true)]
internal partial class UseProgramCommand : RenderCommand
{
	public Program Program;

	public override string ToString() => $"UseProgram({Program.Handle})";
}

[ThreadCommand(generateHandler: true)]
internal partial class VertexAttribPointerCommand : RenderCommand
{
	public uint Index;
	public int Size;
	public int Type;
	public bool Normalized;
	public int Stride;
	public nint Pointer;
}
