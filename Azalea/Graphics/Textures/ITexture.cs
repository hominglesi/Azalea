using Azalea.Graphics.Rendering;
using Azalea.Numerics;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Graphics.Textures;
public interface ITexture
{
	public Platform.Rendering.NativeTexture? NewTexture { get; }

	public INativeTexture GetNativeTexture(float time = 0);
	public Rectangle GetUVCoordinates(float time = 0);

	public virtual int Width => GetNativeTexture().Width;
	public virtual int Height => GetNativeTexture().Height;
	public virtual Vector2 Size
	{
		get
		{
			var nativeTexture = GetNativeTexture();
			return new Vector2(nativeTexture.Width, nativeTexture.Height);
		}
	}
}
