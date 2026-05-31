using Azalea.Graphics.Colors;

namespace Azalea.Platform.Rendering;
internal class RenderCommand { }

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
internal partial class ClearCommand : RenderCommand
{
	public Color Color;
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
internal partial class GenerateTextureCommand : RenderCommand
{
	public Texture Texture;
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
