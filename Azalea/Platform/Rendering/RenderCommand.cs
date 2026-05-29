namespace Azalea.Platform.Rendering;
internal class RenderCommand { }

[RenderCommand]
internal partial class FramebufferTexture2DCommand : RenderCommand
{
	public Framebuffer Framebuffer;
	public Texture2D Texture;
	public int Target;
	public int Attachment;
	public int Textarget;
	public int Level;
}

[RenderCommand]
internal partial class TexImage2DCommand : RenderCommand
{
	public Texture2D Texture;
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
	public Texture2D Texture;
	public int Target;
	public int Parameter;
	public int Value;
}
