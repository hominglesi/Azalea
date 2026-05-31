using Azalea.Graphics.Colors;

namespace Azalea.Platform.Rendering;
public static class RenderCommandQueue_Extentions
{
	public static void BindBuffer(this RenderCommandQueue queue, int type, Buffer? buffer)
		=> queue.Enqueue(BindBufferCommand.Borrow(type, buffer));

	public static void Clear(this RenderCommandQueue queue, Color color)
		=> queue.Enqueue(ClearCommand.Borrow(color));

	public static void SwapBuffers(this RenderCommandQueue queue)
		=> queue.Enqueue(SwapBuffersCommand.Borrow());
}
