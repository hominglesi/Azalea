using Azalea.Numerics;
using Azalea.Platform.Rendering;
using Azalea.Platform.Rendering.Coordination;
using System;
using System.Numerics;
namespace Azalea.Graphics.Textures;

public class Texture : ITexture
{
	private readonly NativeTexture _newTexture;

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
				_region.X / (float)_newTexture.Width,
				_region.Y / (float)_newTexture.Height,
				_region.Width / (float)_newTexture.Width,
				_region.Height / (float)_newTexture.Height);
		}
	}

	public int Width
	{
		get
		{
			if (_region.Width == -1)
				return _newTexture.Width;

			return _region.Width;
		}
	}

	public int Height
	{
		get
		{
			if (_region.Height == -1)
				return _newTexture.Height;

			return _region.Height;
		}
	}

	public Vector2 Size
	{
		get
		{
			if (_region.Width == -1)
				return new(_newTexture.Width, _newTexture.Height);

			return _region.Size;
		}
	}

	public Texture(ITexture other)
		: this(((Texture)other)._newTexture) { }

	internal Texture(NativeTexture newTexture)
	{
		_newTexture = newTexture;
	}

	public Rectangle GetUVCoordinates(float time) => _uvCoordinates;

	public void Bind(RenderCoordinator coordinator, float time = 0) => coordinator.BindTexture(_newTexture);
}
