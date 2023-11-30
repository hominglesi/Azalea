using Azalea.Graphics.Colors;
using Azalea.Graphics.OpenGL.Batches;
using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.OpenGL.Textures;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Numerics;
using Azalea.Platform;

namespace Azalea.Graphics.OpenGL;
internal class GLRenderer : Renderer
{
	public GLRenderer(IWindow window)
		: base(window) { }

	internal override void FinishFrame()
	{
		base.FinishFrame();

		Window.SwapBuffers();

		GL.PrintErrors();
	}

	protected override void SetViewportImplementation(Vector2Int size)
	{
		GL.Viewport(0, 0, size.X, size.Y);
	}

	protected internal override void SetClearColor(Color value)
		=> GL.ClearColor(value);

	protected override void ClearImplementation(Color color)
		=> GL.Clear(GLBufferBit.Color);

	protected override IVertexBatch<TexturedVertex2D> CreateQuadBatch(int size)
		=> new GLVertexBatch<TexturedVertex2D>(Window, size);

	protected override INativeTexture CreateNativeTexture(int width, int height)
		=> new GLTexture(this, width, height);

	protected override bool SetTextureImplementation(INativeTexture? texture, int unit)
	{
		if (texture is null)
		{
			GL.ActiveTexture((uint)unit);
			GL.BindTexture(GLTextureType.Texture2D, 0);
			return true;
		}

		switch (texture)
		{
			case GLTexture glTexture:
				glTexture.Bind((uint)unit);
				break;
		}

		return true;
	}

	protected override void SetScissorTestRectangle(RectangleInt scissorRectangle)
	{
		if (scissorRectangle.Width < 0) scissorRectangle.Width = 0;
		if (scissorRectangle.Height < 0) scissorRectangle.Height = 0;

		var framebufferHeight = Window.ClientSize.Y;

		GL.Scissor(scissorRectangle.X, framebufferHeight - scissorRectangle.Y - scissorRectangle.Height, scissorRectangle.Width, scissorRectangle.Height);
	}

	protected override void SetScissorTestState(bool enabled)
	{
		if (enabled)
			GL.Enable(GLCapability.ScissorTest);
		else
			GL.Disable(GLCapability.ScissorTest);
	}
}
