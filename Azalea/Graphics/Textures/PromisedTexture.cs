using Azalea.Numerics;
using Azalea.Platform.Rendering;
using Azalea.Platform.Rendering.Coordination;
using System;
using System.Numerics;

namespace Azalea.Graphics.Textures;

public class PromisedTexture : ITexture
{
	private readonly NativeTexture _texture;
	public bool IsLoaded => _texture.IsReady();

	public int Width => throw new NotImplementedException();

	public int Height => throw new NotImplementedException();

	public Vector2 Size => throw new NotImplementedException();

	public PromisedTexture(NativeTexture texture)
	{
		_texture = texture;
	}

	public Rectangle GetUVCoordinates(float time)
	{
		return Rectangle.One;
	}

	public void Bind(RenderCoordinator coordinator, float time)
	{
		if (IsLoaded)
			coordinator.BindTexture(_texture);
	}
}
