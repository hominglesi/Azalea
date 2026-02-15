using Azalea.Numerics;
using Azalea.Utils;
using System;
using System.Numerics;
namespace Azalea.Graphics.Textures;

public class Texture : Disposable, ITexture
{
	private readonly INativeTexture _nativeTexture;

	private Rectangle _uvCoordinates = Rectangle.One;
	private RectangleInt _region = new(-1, -1, -1, -1);
	public RectangleInt Region
	{
		get => _region;
		set
		{
			if (value == _region) return;

			_region = value;

			_uvCoordinates = new Rectangle(
				_region.X / (float)_nativeTexture.Width,
				_region.Y / (float)_nativeTexture.Height,
				_region.Width / (float)_nativeTexture.Width,
				_region.Height / (float)_nativeTexture.Height);
		}
	}

	public int Width
	{
		get
		{
			if (_region.Width == -1)
				return _nativeTexture.Width;

			return _region.Width;
		}
	}

	public int Height
	{
		get
		{
			if (_region.Height == -1)
				return _nativeTexture.Height;

			return _region.Height;
		}
	}

	public Vector2 Size
	{
		get
		{
			if (_region.Width == -1)
				return new(_nativeTexture.Width, _nativeTexture.Height);

			return _region.Size;
		}
	}

	public Texture(ITexture other)
		: this(other.GetNativeTexture()) { }

	internal Texture(INativeTexture nativeTexture)
	{
		ArgumentNullException.ThrowIfNull(nativeTexture);

		_nativeTexture = nativeTexture;
	}

	public INativeTexture GetNativeTexture(float time) => _nativeTexture;
	public Rectangle GetUVCoordinates(float time) => _uvCoordinates;

	public void UploadImage(Image upload) => _nativeTexture.SetData(upload);

	public void SetFiltering(TextureFiltering minFilter, TextureFiltering magFilter)
		=> _nativeTexture.SetFiltering(minFilter, magFilter);

	protected override void OnDispose()
	{
		_nativeTexture.Dispose();
	}
}
