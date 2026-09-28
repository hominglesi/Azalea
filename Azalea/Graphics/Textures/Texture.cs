using Azalea.Numerics;
using Azalea.Platform.Rendering;
using Azalea.Platform.Rendering.Coordination;
using Azalea.Utils;
using System;
using System.Numerics;
namespace Azalea.Graphics.Textures;

public class Texture : ITexture
{
	private readonly Platform.Rendering.NativeTexture _newTexture;

	private readonly INativeTexture _nativeTexture;
	private readonly int _nativeWidth;
	private readonly int _nativeHeight;

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
		: this(other.GetNativeTexture(), ((Texture)other)._newTexture) { }

	internal Texture(INativeTexture nativeTexture, NativeTexture newTexture)
	{
		ArgumentNullException.ThrowIfNull(nativeTexture);

		_nativeWidth = nativeTexture.Width;
		_nativeHeight = nativeTexture.Height;
		_nativeTexture = nativeTexture;
		_newTexture = newTexture;
	}

	public INativeTexture GetNativeTexture(float time) => _nativeTexture;
	public Rectangle GetUVCoordinates(float time) => _uvCoordinates;

	public void Bind(RenderCoordinator coordinator, float time = 0) => coordinator.BindTexture(_newTexture);
}
