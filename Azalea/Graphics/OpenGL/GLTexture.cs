using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using StbImageSharp;
using System;
using System.IO;

namespace Azalea.Graphics.OpenGL;
internal class GLTexture : INativeTexture, IDisposable
{
	private uint _handle;

	private int _width;
	private int _height;
	private GLRenderer _renderer;

	public GLTexture(GLRenderer renderer, string filePath, int width, int height)
	{
		_renderer = renderer;

		StbImage.stbi_set_flip_vertically_on_load(1);
		using var stream = File.OpenRead(filePath);
		var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
		_width = width;
		_height = height;

		_handle = GL.GenTexture();
		Bind();

		GL.TexParameteri(GLTextureType.Texture2D, GLTextureParameter.MinFilter, (int)GLFunction.Linear);
		GL.TexParameteri(GLTextureType.Texture2D, GLTextureParameter.MagFilter, (int)GLFunction.Linear);
		GL.TexParameteri(GLTextureType.Texture2D, GLTextureParameter.WrapS, (int)GLWrapFunction.ClampToEdge);
		GL.TexParameteri(GLTextureType.Texture2D, GLTextureParameter.WrapT, (int)GLWrapFunction.ClampToEdge);

		GL.TexImage2D(GLTextureType.Texture2D, 0, GLColorFormat.RGBA,
			image.Width, image.Height, 0, GLColorFormat.RGBA, GLDataType.UnsignedByte, image.Data);

		Unbind();
	}

	internal void SetData(ITextureUpload upload)
	{

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


	void INativeTexture.SetData(ITextureUpload upload)
	{

	}

	#region Disposing
	private bool _disposed;

	~GLTexture() => Dispose(false);

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (_disposed) return;

		if (disposing)
		{
			GL.DeleteTexture(_handle);
		}

		_disposed = true;
	}
	#endregion
}
