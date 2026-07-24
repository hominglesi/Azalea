using Azalea.Graphics.Colors;
using Azalea.Numerics;
using System;
using System.Buffers;
using System.Numerics;

namespace Azalea.Platform.Rendering;
internal abstract class RenderCommand
{
	internal static int TotalCreated = 0;
	internal static Action<RenderCommand>? OnCommandCreated;

	internal RenderCommand()
	{
		TotalCreated++;
		OnCommandCreated?.Invoke(this);
	}

	public override string ToString() => GetType().Name;

	public abstract void Return();
	protected virtual void Cleanup() { }
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class RenderCommandAttribute : Attribute { }

[RenderCommand]
internal partial class AttachShaderCommand : RenderCommand
{
	public Program Program;
	public Shader Shader;
}

[RenderCommand]
internal partial class BindBufferCommand : RenderCommand
{
	public int Type;
	public Buffer? Buffer;
}

[RenderCommand]
internal partial class BindTextureCommand : RenderCommand
{
	public int Type;
	public Texture Texture;
}

[RenderCommand]
internal partial class BindVertexArrayCommand : RenderCommand
{
	public VertexArray? VertexArray;

	public override string ToString()
		=> $"BindVertexArray({(VertexArray is null ? 0 : VertexArray.Handle)})";
}

[RenderCommand]
internal partial class BlendFunctionCommand : RenderCommand
{
	public int SourceFactor;
	public int DestinationFactor;
}

[RenderCommand]
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

[RenderCommand]
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

[RenderCommand]
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

[RenderCommand]
internal partial class ClearCommand : RenderCommand
{
	public Color Color;
}

[RenderCommand]
internal partial class CompileShaderCommand : RenderCommand
{
	public Shader Shader;
}

[RenderCommand]
internal partial class DeleteShaderCommand : RenderCommand
{
	public Shader Shader;
}

[RenderCommand]
internal partial class DisableCommand : RenderCommand
{
	public int Capability;
}

[RenderCommand]
internal partial class DrawArraysCommand : RenderCommand
{
	public int Mode;
	public int First;
	public int Count;
}

[RenderCommand]
internal partial class DrawElementsCommand : RenderCommand
{
	public int Mode;
	public int Count;
	public int Type;
	public int Offset;

	public override string ToString() => $"DrawElements({Count})";
}

[RenderCommand]
internal partial class EnableCommand : RenderCommand
{
	public int Capability;
}

[RenderCommand]
internal partial class EnableVertexAttribArrayCommand : RenderCommand
{
	public uint Index;
}

[RenderCommand]
internal partial class FramebufferTexture2DCommand : RenderCommand
{
	public Framebuffer Framebuffer;
	public Texture Texture;
	public int Target;
	public int Attachment;
	public int Textarget;
	public int Level;
}

[RenderCommand]
internal partial class GenerateBufferCommand : RenderCommand
{
	public Buffer Buffer;
}

[RenderCommand]
internal partial class GenerateFramebufferCommand : RenderCommand
{
	public Framebuffer Framebuffer;
}

[RenderCommand]
internal partial class GenerateMipmapCommand : RenderCommand
{
	public int Target;
}

[RenderCommand]
internal partial class GenerateProgramCommand : RenderCommand
{
	public Program Program;
}

[RenderCommand]
internal partial class GenerateShaderCommand : RenderCommand
{
	public Shader Shader;
	public int ShaderType;
}

[RenderCommand]
internal partial class GenerateTextureCommand : RenderCommand
{
	public Texture Texture;
}

[RenderCommand]
internal partial class GenerateVertexArrayCommand : RenderCommand
{
	public VertexArray VertexArray;
}

[RenderCommand]
internal partial class GetUniformLocationCommand : RenderCommand
{
	public UniformLocation UniformLocation;
	public Program Program;
	public string Name;
}

[RenderCommand]
internal partial class LinkProgramCommand : RenderCommand
{
	public Program Program;
}

[RenderCommand]
internal partial class PolygonModeCommand : RenderCommand
{
	public int Face;
	public int Mode;
}

[RenderCommand]
internal partial class PrepareRenderingCommand : RenderCommand { }

[RenderCommand]
internal partial class PrintErrorsCommand : RenderCommand { }

[RenderCommand]
internal partial class PrintProgramCompileStatusCommand : RenderCommand
{
	public Program Program;
}

[RenderCommand]
internal partial class PrintShaderCompileStatusCommand : RenderCommand
{
	public Shader Shader;
}

[RenderCommand]
internal partial class ScissorCommand : RenderCommand
{
	public RectangleInt? Rectangle;

	public override string ToString()
		=> Rectangle is null ? $"Scissor(0)" : $"Scissor({Rectangle})";
}

[RenderCommand]
internal partial class ShaderSourceCommand : RenderCommand
{
	public Shader Shader;
	public string SourceCode;
}

[RenderCommand]
internal partial class SwapBuffersCommand : RenderCommand { }

[RenderCommand]
internal partial class TexImage2DCommand : RenderCommand
{
	public int Target;
	public int Level;
	public int InternalFormat;
	public int Width;
	public int Height;
	public int Border;
	public int Format;
	public int Type;
	public byte[]? Pixels;
}

[RenderCommand]
internal partial class TexParameteriCommand : RenderCommand
{
	public Texture Texture;
	public int Target;
	public int Parameter;
	public int Value;
}

[RenderCommand]
internal partial class Uniform1iCommand : RenderCommand
{
	public UniformLocation UniformLocation;
	public int Int1;
}

[RenderCommand]
internal partial class Uniform4fCommand : RenderCommand
{
	public UniformLocation UniformLocation;
	public float Value0;
	public float Value1;
	public float Value2;
	public float Value3;
}

[RenderCommand]
internal partial class UniformMatrix4fvCommand : RenderCommand
{
	public UniformLocation UniformLocation;
	public int Count;
	public bool Transpose;
	public Matrix4x4 Value;
}

[RenderCommand]
internal partial class UseProgramCommand : RenderCommand
{
	public Program Program;

	public override string ToString() => $"UseProgram({Program.Handle})";
}

[RenderCommand]
internal partial class VertexAttribPointerCommand : RenderCommand
{
	public uint Index;
	public int Size;
	public int Type;
	public bool Normalized;
	public int Stride;
	public nint Pointer;
}


