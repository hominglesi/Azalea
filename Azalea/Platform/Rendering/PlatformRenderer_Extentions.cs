namespace Azalea.Platform.Rendering;
public static class PlatformRenderer_Extentions
{
	public static void BufferData(this RenderCommandQueue queue, int type, nint size, byte[]? data, int hint)
		=> queue.Enqueue(BufferDataCommand.Borrow(type, size, data, hint));

	public static Buffer GenerateBuffer(this PlatformRenderer renderer)
	{
		var buffer = new Buffer();
		renderer.IssuePriorityCommand(GenerateBufferCommand.Borrow(buffer));
		return buffer;
	}

	public static Framebuffer GenerateFramebuffer(this PlatformRenderer renderer)
	{
		var framebuffer = new Framebuffer();
		renderer.IssuePriorityCommand(GenerateFramebufferCommand.Borrow(framebuffer));
		return framebuffer;
	}

	public static Texture GenerateTexture(this PlatformRenderer renderer)
	{
		var texture = new Texture();
		renderer.IssuePriorityCommand(GenerateTextureCommand.Borrow(texture));
		return texture;
	}
}
