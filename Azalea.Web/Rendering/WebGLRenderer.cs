using Azalea.Graphics.Colors;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Numerics;
using Azalea.Platform;

namespace Azalea.Web.Rendering;

internal class WebGLRenderer(IWindow window) : Renderer(window)
{
	// TODO: Call implementation
	protected override void ClearImplementation(Color color)
	{
		throw new System.NotImplementedException();
	}

	//TODO
	protected override INativeTexture CreateNativeTexture(int width, int height)
	{
		throw new System.NotImplementedException();
	}

	protected override IVertexBatch<TexturedVertex2D> CreateQuadBatch(int size)
		=> new WebGLVertexBatch<TexturedVertex2D>(Window, size);

	// TODO: Call implementation
	protected override void SetScissorTestRectangle(RectangleInt scissorRectangle)
	{
		throw new System.NotImplementedException();
	}

	// TODO: Call implementation
	protected override void SetScissorTestState(bool enabled)
	{
		throw new System.NotImplementedException();
	}

	// TODO: Call implementation
	protected override bool SetTextureImplementation(INativeTexture texture, int unit)
	{
		throw new System.NotImplementedException();
	}

	// TODO: Call implementation
	protected override void SetViewportImplementation(Vector2Int size)
	{
		throw new System.NotImplementedException();
	}
}
