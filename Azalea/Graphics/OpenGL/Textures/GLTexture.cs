using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
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

		Native.OpenGL.glGenTextures(1, ref _handle);
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
		GL.TexParameteri(GLTextureType.Texture2D, GLTextureParameter.WrapS, (int)GLWrapFunction.Repeat);
		GL.TexParameteri(GLTextureType.Texture2D, GLTextureParameter.WrapT, (int)GLWrapFunction.Repeat);

		Native.OpenGL.glTexImage2D(Native.OpenGL.GL_TEXTURE_2D, 0, Native.OpenGL.GL_RGBA,
			_width, _height, 0, Native.OpenGL.GL_RGBA, Native.OpenGL.GL_UNSIGNED_BYTE, in image.Data[0]);

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
		Native.OpenGL.glBindTexture(Native.OpenGL.GL_TEXTURE_2D, _handle);
	}

	public void Unbind() => Native.OpenGL.glBindTexture(Native.OpenGL.GL_TEXTURE_2D, 0);

	public IRenderer Renderer => _renderer;
	public int Width => _width;
	public int Height => _height;


	void INativeTexture.SetData(Image upload) => SetData(upload);

	protected override void OnDispose()
	{
		GL.DeleteTexture(_handle);
	}
}
