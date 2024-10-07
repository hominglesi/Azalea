using Azalea.Graphics.Colors;
using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Numerics;
using Azalea.Platform;

namespace Azalea.Web.Rendering;

internal class WebGLRenderer : RendererBase
{
	public WebGLRenderer(IWindow window)
		: base(window)
	{
		//We need to update the viewport when starting because we don't set its size
		SetViewport(Window.ClientSize);

		WebGL.Enable(GLCapability.Blend);
		WebGL.BlendFuncSeparate(GLBlendFunction.SrcAlpha, GLBlendFunction.OneMinusSrcAlpha, GLBlendFunction.SrcAlpha, GLBlendFunction.OneMinusSrcAlpha);
	}
	protected override void SetViewportImplementation(Vector2Int size)
		=> WebGL.Viewport(0, 0, size.X, size.Y);

	protected internal override void SetClearColor(Color value)
		=> WebGL.ClearColor(value.RNormalized, value.GNormalized, value.BNormalized, value.ANormalized);

	protected override void ClearImplementation()
		=> WebGL.Clear(GLBufferBit.Color);

	protected override IVertexBatch<TexturedVertex2D> CreateQuadBatch(int size)
		=> new WebGLVertexBatch<TexturedVertex2D>(Window, size);

	protected override INativeTexture CreateNativeTexture(int width, int height)
		=> new WebGLTexture(this, width, height);

	protected override bool SetTextureImplementation(INativeTexture? texture, int unit)
	{
		if (texture is null)
		{
			WebGL.ActiveTexture(unit);
			WebGL.BindTexture(GLTextureType.Texture2D, 0);
			return true;
		}

		switch (texture)
		{
			case WebGLTexture webGLTexture:
				webGLTexture.Bind();
				break;
		}

		return true;
	}

	protected override void SetScissorTestRectangle(RectangleInt scissorRectangle)
	{
		if (scissorRectangle.Width < 0) scissorRectangle.Width = 0;
		if (scissorRectangle.Height < 0) scissorRectangle.Height = 0;

		var framebufferHeight = Window.ClientSize.Y;

		WebGL.Scissor(scissorRectangle.X, framebufferHeight - scissorRectangle.Y - scissorRectangle.Height, scissorRectangle.Width, scissorRectangle.Height);
	}

	protected override void SetScissorTestState(bool enabled)
	{
		if (enabled)
			WebGL.Enable(GLCapability.ScissorTest);
		else
			WebGL.Disable(GLCapability.ScissorTest);
	}
}
