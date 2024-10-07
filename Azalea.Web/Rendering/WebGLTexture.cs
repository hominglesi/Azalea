using Azalea.Graphics;
using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using Azalea.Utils;
using System;

namespace Azalea.Web.Rendering;

internal class WebGLTexture : UnmanagedObject<object>, INativeTexture
{
	protected override object CreateObject() => WebGL.CreateTexture();

	private int _width;
	private int _height;
	private WebGLRenderer _renderer;

	public WebGLTexture(WebGLRenderer renderer, int width, int height)
	{
		_renderer = renderer;
		_width = width;
		_height = height;
	}

	internal void SetData(Image image)
	{
		if (image.Width != _width || image.Height != _height)
		{
			Console.WriteLine("Provided image was not the correct size");
			return;
		}

		_renderer.BindTexture(this, 0);

		SetFiltering(TextureFiltering.Nearest, TextureFiltering.Nearest);

		WebGL.TexParameteri(GLTextureType.Texture2D, GLTextureParameter.WrapS, (int)GLWrapFunction.Repeat);
		WebGL.TexParameteri(GLTextureType.Texture2D, GLTextureParameter.WrapT, (int)GLWrapFunction.Repeat);

		WebGL.TexImage2D(GLTextureType.Texture2D, 0, GLColorFormat.RGBA,
			_width, _height, 0, GLColorFormat.RGBA, GLDataType.UnsignedByte, image.Data);

		WebGL.GenerateMipmap(GLTextureType.Texture2D);
	}

	// TODO: Create implementation
	public void SetFiltering(TextureFiltering minFilter, TextureFiltering magFilter)
	{
		_renderer.BindTexture(this, 0);
		WebGL.TexParameteri(GLTextureType.Texture2D, GLTextureParameter.MinFilter,
			minFilter == TextureFiltering.Nearest ? (int)GLFunction.Nearest : (int)GLFunction.Linear);
		WebGL.TexParameteri(GLTextureType.Texture2D, GLTextureParameter.MagFilter,
			magFilter == TextureFiltering.Nearest ? (int)GLFunction.Nearest : (int)GLFunction.Linear);
	}

	public void Bind() => WebGL.BindTexture(GLTextureType.Texture2D, Handle);

	public IRenderer Renderer => _renderer;
	public int Width => _width;
	public int Height => _height;

	void INativeTexture.SetData(Image upload) => SetData(upload);

	protected override void OnDispose() => WebGL.DeleteTexture(Handle);
}
