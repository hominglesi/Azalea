using Azalea.Graphics.OpenGL.Enums;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using Azalea.Utils;
using System;

namespace Azalea.Graphics.OpenGL.Textures;
internal class GLTexture : Disposable, INativeTexture
{
	public uint Handle { get; init; }
	public int Width { get; init; }
	public int Height { get; init; }
	public IRenderer Renderer => _renderer;

	private readonly GLRenderer _renderer;


	public GLTexture(GLRenderer renderer, int width, int height)
	{
		_renderer = renderer;
		Width = width;
		Height = height;

		Handle = GL.GenTexture();
	}

	internal void SetData(Image image)
	{
		if (image.Width != Width || image.Height != Height)
		{
			Console.WriteLine("Provided image was not the correct size");
			return;
		}

		_renderer.BindTexture(this, 0);

		SetFiltering(TextureFiltering.Nearest, TextureFiltering.Nearest);
		GL.TexParameteri(GLTextureType.Texture2D, GLTextureParameter.WrapS, (int)GLWrapFunction.ClampToEdge);
		GL.TexParameteri(GLTextureType.Texture2D, GLTextureParameter.WrapT, (int)GLWrapFunction.ClampToEdge);

		GL.TexImage2D(GLTextureType.Texture2D, 0, GLColorFormat.RGBA,
			Width, Height, 0, GLColorFormat.RGBA, GLDataType.UnsignedByte, image.Data);

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

	void INativeTexture.SetData(Image upload) => SetData(upload);

	protected override void OnDispose()
	{
		GL.DeleteTexture(Handle);
	}
}
