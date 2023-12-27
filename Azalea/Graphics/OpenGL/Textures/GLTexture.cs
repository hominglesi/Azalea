using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using Azalea.IO.Resources;
using Azalea.Utils;
using System;

namespace Azalea.Graphics.OpenGL.Textures;
internal class GLTexture : Disposable, INativeTexture
{
	private uint _handle;

	private int _width;
	private int _height;
	private GLRenderer _renderer;

	public GLTexture(GLRenderer renderer, int width, int height)
	{
		_renderer = renderer;
		_width = width;
		_height = height;

		_handle = GL.GenTexture();
	}

	internal void SetData(Image image)
	{
		if (image.Width != _width || image.Height != _height)
		{
			Console.WriteLine("Provided image was not the correct size");
			return;
		}

		_renderer.BindTexture(this, 0);

		SetFiltering(Assets.DefaultMinFiltering, Assets.DefaultMagFiltering);
		GL.TexParameteri(GLTextureType.Texture2D, GLTextureParameter.WrapS, (int)GLWrapFunction.ClampToEdge);
		GL.TexParameteri(GLTextureType.Texture2D, GLTextureParameter.WrapT, (int)GLWrapFunction.ClampToEdge);

		GL.TexImage2D(GLTextureType.Texture2D, 0, GLColorFormat.RGBA,
			_width, _height, 0, GLColorFormat.RGBA, GLDataType.UnsignedByte, image.Data);

		GL.GenerateMipmap(GLTextureType.Texture2D);
	}

	public void SetFiltering(TextureFiltering minFilter, TextureFiltering magFilter)
	{
		_renderer.BindTexture(this, 0);
		GL.TexParameteri(GLTextureType.Texture2D, GLTextureParameter.MinFilter,
			minFilter == TextureFiltering.Nearest ? (int)GLFunction.Nearest : (int)GLFunction.Linear);
		GL.TexParameteri(GLTextureType.Texture2D, GLTextureParameter.MagFilter,
			magFilter == TextureFiltering.Nearest ? (int)GLFunction.Nearest : (int)GLFunction.Linear);
	}

	public void Bind(uint slot = 0)
	{
		GL.ActiveTexture(slot);
		GL.BindTexture(GLTextureType.Texture2D, _handle);
	}

	public void Unbind() => GL.BindTexture(GLTextureType.Texture2D, 0);

	public IRenderer Renderer => _renderer;
	public int Width => _width;
	public int Height => _height;


	void INativeTexture.SetData(Image upload) => SetData(upload);

	protected override void OnDispose()
	{
		GL.DeleteTexture(_handle);
	}
}
