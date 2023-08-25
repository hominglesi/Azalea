using Azalea.Graphics;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Platform;
using Azalea.Web.Graphics.Blazor.Batches;
using Azalea.Web.Graphics.Blazor.Textures;
using nkast.Wasm.Canvas.WebGL;

namespace Azalea.Web.Graphics.Blazor;

internal class BlazorRenderer : Renderer
{
	private readonly IWebGLRenderingContext _gl;
	private readonly IWindow _window;

	public BlazorRenderer(IWebGLRenderingContext gl, IWindow window)
	{
		_gl = gl;
		_window = window;
	}

	protected internal override void SetClearColor(Color value)
	{
		base.SetClearColor(value);

		_gl.ClearColor(value.RNormalized, value.GNormalized, value.BNormalized, value.ANormalized);
	}

	protected override void ClearImplementation(Color color)
	{
		_gl.Clear(WebGLBufferBits.COLOR);
	}

	protected override INativeTexture CreateNativeTexture(int width, int height)
		=> new BlazorTexture(this, _gl, width, height);

	protected override IVertexBatch<TexturedVertex2D> CreateQuadBatch(int size)
		=> new BlazorVertexBatch<TexturedVertex2D>(_gl, _window, size);

	protected override bool SetTextureImplementation(INativeTexture? texture, int unit)
	{
		if (texture is null || (texture is BlazorTexture blTexture && blTexture.Texture is null))
		{
			_gl.ActiveTexture(WebGLTextureUnit.TEXTURE0 + unit);
			_gl.BindTexture(WebGLTextureTarget.TEXTURE_2D, null);
			return true;
		}

		switch (texture)
		{
			case BlazorTexture blzTexture:
				_gl.ActiveTexture(WebGLTextureUnit.TEXTURE0 + unit);
				_gl.BindTexture(WebGLTextureTarget.TEXTURE_2D, blzTexture.Texture);
				break;
		}

		return true;
	}
}
