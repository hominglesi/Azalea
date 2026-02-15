using Azalea.IO.Resources;
using Azalea.Numerics;
using Azalea.Threading;

namespace Azalea.Graphics.Textures;
public class PromisedTexture : ITexture
{
	private ITexture? _resolvedTexture;
	private readonly ITexture _loadingTexture;

	public bool IsResolved => _resolvedTexture is not null;

	public PromisedTexture(Promise<ITexture> promise, ITexture? loadingTexture = null)
	{
		_loadingTexture = loadingTexture ?? Assets.MissingTexture;

		promise.Then(onTextureResolved);
	}

	public INativeTexture GetNativeTexture(float time)
	{
		if (_resolvedTexture is null)
			return _loadingTexture.GetNativeTexture(time);

		return _resolvedTexture.GetNativeTexture(time);
	}

	public Rectangle GetUVCoordinates(float time)
	{
		if (_resolvedTexture is null)
			return _loadingTexture.GetUVCoordinates(time);

		return _resolvedTexture.GetUVCoordinates(time);
	}

	private void onTextureResolved(ITexture resolvedTexture)
	{
		_resolvedTexture = resolvedTexture;

		if (_pendingImageUpload is not null)
			_resolvedTexture.UploadImage(_pendingImageUpload);
	}

	private Image? _pendingImageUpload = null;
	public void UploadImage(Image image)
	{
		if (_resolvedTexture is not null)
		{
			_resolvedTexture.UploadImage(image);
			return;
		}

		// If the image changes while the texture is resolving
		// this could lead to unexpected behaviour
		_pendingImageUpload = image;
	}

	private TextureFiltering? _pendingMinFilter = null;
	private TextureFiltering? _pendingMagFilter = null;
	public void SetFiltering(TextureFiltering minFilter, TextureFiltering magFilter)
	{
		if (_resolvedTexture is not null)
		{
			_resolvedTexture.SetFiltering(minFilter, magFilter);
			return;
		}

		_pendingMinFilter = minFilter;
		_pendingMagFilter = magFilter;
	}
}
