using Azalea.Graphics.Rendering;
using Azalea.Utils;
using System;
using System.Numerics;
using Rect = Azalea.Numerics.Rectangle;

namespace Azalea.Graphics.Textures;

public class Texture : Disposable
{
	internal virtual INativeTexture NativeTexture { get; }
	public virtual int Width => NativeTexture.Width;
	public virtual int Height => NativeTexture.Height;
	public virtual Vector2 Size => new(Width, Height);

	public string AssetName { get; set; }

	internal Texture(INativeTexture nativeTexture)
	{
		NativeTexture = nativeTexture ?? throw new ArgumentNullException(nameof(nativeTexture));
	}

	internal void SetData(Image upload)
	{
		NativeTexture.SetData(upload);
	}

	internal virtual Rect GetUVCoordinates() => Rect.One;

	protected override void OnDispose()
	{
		NativeTexture.Dispose();
	}
}
