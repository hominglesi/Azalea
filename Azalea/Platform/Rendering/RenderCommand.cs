using Azalea.Graphics.Colors;
using System;

namespace Azalea.Platform.Rendering;
internal abstract class RenderCommand
{
	internal RenderCommand()
	{
		Console.WriteLine("Created " + GetType().Name);
	}

	public abstract void Return();
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class RenderCommandAttribute : Attribute { }

[RenderCommand]
internal partial class BindBufferCommand : RenderCommand
{
	public int Type;
	public Buffer? Buffer;
}

[RenderCommand]
internal partial class BufferDataCommand : RenderCommand
{
	public int Type;
	public nint Size;
	public byte[]? Data;
	public int Hint;
}

[RenderCommand]
internal partial class BufferDataFloatCommand : RenderCommand
{
	public int Type;
	public nint Size;
	public float[]? Data;
	public int Hint;
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
internal partial class DisplayShaderCompileStatusCommand : RenderCommand
{
	public Shader Shader;
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
	public Texture Texture;
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
