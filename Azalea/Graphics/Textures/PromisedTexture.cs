using Azalea.IO.Resources;
using Azalea.Numerics;
using Azalea.Threading;
using System;

namespace Azalea.Graphics.Textures;
public class PromisedTexture : ITexture
{
	private readonly ValuePromise<ITexture> _promise;
	private readonly ITexture _loadingTexture;

	public bool IsResolved => _promise.IsResolved;

	public PromisedTexture(ValuePromise<ITexture> promise, ITexture? loadingTexture = null)
	{
		_loadingTexture = loadingTexture ?? Assets.MissingTexture;
		_promise = promise;
	}

	public INativeTexture GetNativeTexture(float time)
	{
		if (IsResolved == false)
			return _loadingTexture.GetNativeTexture(time);

		return _promise.Value.GetNativeTexture(time);
	}

	public Rectangle GetUVCoordinates(float time)
	{
		if (IsResolved == false)
			return _loadingTexture.GetUVCoordinates(time);

		return _promise.Value.GetUVCoordinates(time);
	}

	public void UploadImage(Image image)
	{
		throw new NotImplementedException();
	}

	public void SetFiltering(TextureFiltering minFilter, TextureFiltering magFilter)
	{
		throw new NotImplementedException();
	}
}
