using Azalea.Numerics;
using System.Numerics;

namespace Azalea.Graphics.Textures;
public class TextureRegion : Texture
{
	private readonly RectangleInt _region;
	private readonly Texture _base;

	public TextureRegion(Texture texture, RectangleInt region)
		: base(texture.NativeTexture)
	{
		_base = texture;
		_region = region;
	}

	public override int Width => _region.Width;
	public override int Height => _region.Height;
	public override Vector2 Size => _region.Size;

	internal override Rectangle GetUVCoordinates()
	{
		var x = new Rectangle(
			_region.X / (float)_base.Width,
			_region.Y / (float)_base.Height,
			_region.Width / (float)_base.Width,
			_region.Height / (float)_base.Height);

		return x;
	}
}
