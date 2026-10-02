using Azalea.IO.Resources;
using Azalea.Numerics;
using Azalea.Platform.Rendering.Coordination;
using Azalea.Threading;
using System;
using System.Numerics;

namespace Azalea.Graphics.Textures;
public class PromisedTexture : ITexture
{
	private readonly ValuePromise<ITexture> _promise;
	private readonly ITexture _loadingTexture;

	public bool IsResolved => _promise.IsResolved;

	public Platform.Rendering.NativeTexture? NewTexture => null;

	public int Width => throw new NotImplementedException();

	public int Height => throw new NotImplementedException();

	public Vector2 Size => throw new NotImplementedException();

	public PromisedTexture(ValuePromise<ITexture> promise, ITexture? loadingTexture = null)
	{
		_loadingTexture = loadingTexture ?? Assets.GetTexture("Textures/azalea-icon.png");
		_promise = promise;
	}

	public Rectangle GetUVCoordinates(float time)
	{
		if (IsResolved == false)
			return _loadingTexture.GetUVCoordinates(time);

		return _promise.Value.GetUVCoordinates(time);
	}

	public void Bind(RenderCoordinator coordinator, float time)
	{
		if (IsResolved == false)
			_loadingTexture.Bind(coordinator, time);
		else
			_promise.Value.Bind(coordinator, time);
	}
}
