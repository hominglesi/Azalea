using Azalea.IO.Resources;
using Azalea.Numerics;
using Azalea.Platform.Rendering.Coordination;
using Azalea.Threading;
using System;

namespace Azalea.Graphics.Textures;
public class PromisedTexture : ITexture
{
	private readonly ValuePromise<ITexture> _promise;
	private readonly ITexture _loadingTexture;

	public bool IsResolved => _promise.IsResolved;

	public Platform.Rendering.NativeTexture? NewTexture => null;

	public PromisedTexture(ValuePromise<ITexture> promise, ITexture? loadingTexture = null)
	{
		_loadingTexture = loadingTexture ?? Assets.GetTexture("Textures/azalea-icon.png");
		_promise = promise;
	}

	public INativeTexture GetNativeTexture(float time)
	{
		if (IsResolved == false)
			return null; //_loadingTexture.GetNativeTexture(time);

		return null; //_promise.Value.GetNativeTexture(time);
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
