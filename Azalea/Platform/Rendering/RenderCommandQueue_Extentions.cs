using Azalea.Graphics.Colors;
using Azalea.Numerics;

namespace Azalea.Platform.Rendering;
public static class RenderCommandQueue_Extentions
{
	public static void Clear(this RenderCommandQueue queue, Color color)
		=> queue.Enqueue(ClearCommand.Borrow(color));

	public static void DrawArrays(this RenderCommandQueue queue, int mode, int first, int count)
		=> queue.Enqueue(DrawArraysCommand.Borrow(mode, first, count));

	public static void DrawElements(this RenderCommandQueue queue, int mode, int count, int type, int offset)
		=> queue.Enqueue(DrawElementsCommand.Borrow(mode, count, type, offset));

	public static void PrepareRendering(this RenderCommandQueue queue)
		=> queue.Enqueue(PrepareRenderingCommand.Borrow());

	public static void Scissor(this RenderCommandQueue queue, RectangleInt? rectangle)
		=> queue.Enqueue(ScissorCommand.Borrow(rectangle));

	public static void SwapBuffers(this RenderCommandQueue queue)
		=> queue.Enqueue(SwapBuffersCommand.Borrow());
}
