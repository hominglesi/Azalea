using Azalea.Graphics;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using Azalea.Utils;
using System;

namespace Azalea.Web.Rendering;

internal class WebGLTexture : Disposable, INativeTexture
{
	public object Handle { get; init; }

	private int _width;
	private int _height;
	private WebGLRenderer _renderer;

	public WebGLTexture(WebGLRenderer renderer, int width, int height)
	{
		_renderer = renderer;
		_width = width;
		_height = height;

		// TODO: Call implementation
		//_handle = 
	}

	// TODO: Create implementation
	internal void SetData(Image image)
	{

	}

	// TODO: Create implementation
	public void SetFiltering(TextureFiltering minFilter, TextureFiltering magFilter)
	{

	}

	// TODO: Create implementation
	public void Bind(uint slot = 0)
	{

	}

	// TODO: Call implementation
	public void Unbind() => throw new NotImplementedException();

	public IRenderer Renderer => _renderer;
	public int Width => _width;
	public int Height => _height;

	void INativeTexture.SetData(Image upload) => SetData(upload);

	protected override void OnDispose()
	{
		// TODO: Call implementation
		// 
	}
}
