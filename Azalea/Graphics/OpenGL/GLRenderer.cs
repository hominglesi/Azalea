using Azalea.Graphics.Colors;
using Azalea.Graphics.OpenGL.Batches;
using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.OpenGL.Textures;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Numerics;
using Azalea.Platform;
using Azalea.Platform.Desktop;

namespace Azalea.Graphics.OpenGL;
internal class GLRenderer : Renderer
{
	private readonly IWindow _window;

	public GLRenderer(IWindow window)
	{
		_window = window;
	}


	private bool _firstFrame = true;
	internal override void FinishFrame()
	{
		base.FinishFrame();

		//One the first frame the textures are not correct for some reason
		//so we hide it
		if (_firstFrame)
			_firstFrame = false;
		else
			((GLFWWindow)_window).SwapBuffers();

		GL.PrintErrors();
	}

	protected internal override void SetClearColor(Color value)
		=> GL.ClearColor(value);

	protected override void ClearImplementation(Color color)
		=> GL.Clear(GLBufferBit.Color);

	protected override IVertexBatch<TexturedVertex2D> CreateQuadBatch(int size)
		=> new GLVertexBatch<TexturedVertex2D>(_window, size);

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

		var framebufferHeight = _window.ClientSize.Y;

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
