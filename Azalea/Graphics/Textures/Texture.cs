using Azalea.Numerics;
using Azalea.Platform.Rendering;
using System.Numerics;
namespace Azalea.Graphics.Textures;

public class Texture : ITexture
{
	internal readonly NativeTexture NativeTexture;
	internal bool IsLoaded => NativeTexture.IsReady();

	public Texture(ITexture other)
		: this(((Texture)other).NativeTexture) { }

	internal Texture(NativeTexture newTexture) => NativeTexture = newTexture;

	internal Rectangle UVCoordinates { get; private set; } = Rectangle.One;
	private RectangleInt _region = new(-1, -1, -1, -1);
	public RectangleInt Region
	{
		get => _region;
		set
		{
			if (value == _region) return;

			_region = value;

			UVCoordinates = new Rectangle(
				_region.X / (float)NativeTexture.Width,
				_region.Y / (float)NativeTexture.Height,
				_region.Width / (float)NativeTexture.Width,
				_region.Height / (float)NativeTexture.Height);
		}
	}

	public int Width
	{
		get
		{
			if (_region.Width == -1)
				return NativeTexture.Width;

			return _region.Width;
		}
	}

	public int Height
	{
		get
		{
			if (_region.Height == -1)
				return NativeTexture.Height;

			return _region.Height;
		}
	}

	public Vector2 Size
	{
		get
		{
			if (_region.Width == -1)
				return new(NativeTexture.Width, NativeTexture.Height);

			return _region.Size;
		}
	}
}
