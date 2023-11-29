using Azalea.Graphics.Rendering;
using Azalea.Utils;
using System;
using System.IO;
using System.Numerics;

namespace Azalea.Graphics.Textures;

public class Texture : Disposable
{
	internal virtual INativeTexture NativeTexture { get; }
	public int Width => NativeTexture.Width;
	public int Height => NativeTexture.Height;
	public Vector2 Size => new(Width, Height);

	public string AssetName { get; set; }

	internal Texture(INativeTexture nativeTexture)
	{
		NativeTexture = nativeTexture ?? throw new ArgumentNullException(nameof(nativeTexture));
	}

	public static Texture? FromStream(IRenderer renderer, Stream? stream)
	{
		if (stream is null || stream.Length == 0)
			return null;

		var data = new TextureData(stream);
		return FromData(renderer, data);
	}

	public static Texture? FromData(IRenderer renderer, TextureData data)
	{
		Texture texture = renderer.CreateTexture(data.Width, data.Height);
		texture.SetData(data);
		return texture;
	}

	internal void SetData(ITextureData upload)
	{
		NativeTexture.SetData(upload);
	}

	protected override void OnDispose()
	{
		NativeTexture.Dispose();
	}
}
