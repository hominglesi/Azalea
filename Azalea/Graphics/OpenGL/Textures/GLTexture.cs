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

		Native.OpenGL.GL.GenTextures(1, ref _handle);
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
		Native.OpenGL.GL.TexParameteri(Native.OpenGL.GL.TEXTURE_2D, Native.OpenGL.GL.TEXTURE_WRAP_S, Native.OpenGL.GL.REPEAT);
		Native.OpenGL.GL.TexParameteri(Native.OpenGL.GL.TEXTURE_2D, Native.OpenGL.GL.TEXTURE_WRAP_T, Native.OpenGL.GL.REPEAT);

		Native.OpenGL.GL.TexImage2D(Native.OpenGL.GL.TEXTURE_2D, 0, Native.OpenGL.GL.RGBA,
			_width, _height, 0, Native.OpenGL.GL.RGBA, Native.OpenGL.GL.UNSIGNED_BYTE, in image.Data[0]);

		GL.GenerateMipmap(GLTextureType.Texture2D);
	}

	public void SetFiltering(TextureFiltering minFilter, TextureFiltering magFilter)
	{
		_renderer.BindTexture(this, 0);
		Native.OpenGL.GL.TexParameteri(Native.OpenGL.GL.TEXTURE_2D, Native.OpenGL.GL.TEXTURE_MIN_FILTER,
			minFilter == TextureFiltering.Nearest ? Native.OpenGL.GL.NEAREST : Native.OpenGL.GL.LINEAR);
		Native.OpenGL.GL.TexParameteri(Native.OpenGL.GL.TEXTURE_2D, Native.OpenGL.GL.TEXTURE_MAG_FILTER,
			magFilter == TextureFiltering.Nearest ? Native.OpenGL.GL.NEAREST : Native.OpenGL.GL.LINEAR);
	}

	public void Bind(uint slot = 0)
	{
		GL.ActiveTexture(slot);
		Native.OpenGL.GL.BindTexture(Native.OpenGL.GL.TEXTURE_2D, _handle);
	}

	public void Unbind() => Native.OpenGL.GL.BindTexture(Native.OpenGL.GL.TEXTURE_2D, 0);

	public IRenderer Renderer => _renderer;
	public int Width => _width;
	public int Height => _height;


	void INativeTexture.SetData(Image upload) => SetData(upload);

	protected override void OnDispose()
	{
		GL.DeleteTexture(_handle);
	}
}
