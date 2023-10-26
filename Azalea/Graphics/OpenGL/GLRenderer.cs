using Azalea.Graphics.Colors;
using Azalea.Graphics.OpenGL.Batches;
using Azalea.Graphics.OpenGL.Enums;
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

	internal override void FinishFrame()
	{
		base.FinishFrame();
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
		=> new GLTexture(this, "D:\\Programming\\Azalea\\Azalea\\Resources\\Textures\\azalea-icon.png", width, height);

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
				GL.ActiveTexture((uint)unit);
				glTexture.Bind((uint)unit);
				break;
		}

		return true;
	}

	protected override void SetScissorTestRectangle(RectangleInt scissorRectangle)
	{
		//throw new System.NotImplementedException();
	}

	protected override void SetScissorTestState(bool enabled)
	{
		//throw new System.NotImplementedException();
	}
}
