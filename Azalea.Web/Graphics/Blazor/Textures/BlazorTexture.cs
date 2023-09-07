using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using Azalea.Utils;
using nkast.Wasm.Canvas.WebGL;

namespace Azalea.Web.Graphics.Blazor.Textures;

internal class BlazorTexture : INativeTexture
{
	protected readonly BlazorRenderer Renderer;
	private readonly IWebGLRenderingContext _gl;

	IRenderer INativeTexture.Renderer => Renderer;

	public WebGLTexture? Texture;

	public int Width { get; set; }
	public int Height { get; set; }

	public BlazorTexture(BlazorRenderer renderer, IWebGLRenderingContext gl, int width, int height)
	{
		Renderer = renderer;
		_gl = gl;

		Width = width;
		Height = height;
	}

	public void SetData(ITextureUpload upload)
	{
		Texture = _gl.CreateTexture();
		_gl.ActiveTexture(WebGLTextureUnit.TEXTURE0);
		_gl.BindTexture(WebGLTextureTarget.TEXTURE_2D, Texture);

		_gl.TexParameter(WebGLTextureTarget.TEXTURE_2D, WebGLTexParamName.TEXTURE_MIN_FILTER, WebGLTexParam.LINEAR);
		_gl.TexParameter(WebGLTextureTarget.TEXTURE_2D, WebGLTexParamName.TEXTURE_MAG_FILTER, WebGLTexParam.LINEAR);

		_gl.TexParameter(WebGLTextureTarget.TEXTURE_2D, WebGLTexParamName.TEXTURE_WRAP_S, WebGLTexParam.REPEAT);
		_gl.TexParameter(WebGLTextureTarget.TEXTURE_2D, WebGLTexParamName.TEXTURE_WRAP_T, WebGLTexParam.REPEAT);

		_gl.TexImage2D(WebGLTextureTarget.TEXTURE_2D, 0, WebGLInternalFormat.RGBA, Width, Height,
			WebGLFormat.RGBA, WebGLTexelType.UNSIGNED_BYTE, upload.Data.ToArray());

		if (MathUtils.IsPowerOfTwo(Width) && MathUtils.IsPowerOfTwo(Height))
			_gl.GenerateMipmap(WebGLTextureTarget.TEXTURE_2D);
	}
}
