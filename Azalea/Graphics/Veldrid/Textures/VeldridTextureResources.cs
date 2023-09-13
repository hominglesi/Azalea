using System;
using Veldrid;

namespace Azalea.Graphics.Veldrid.Textures;

internal class VeldridTextureResources : IDisposable
{
	public readonly Texture Texture;

	private Sampler? _sampler;

	public Sampler? Sampler
	{
		get => _sampler;
		set
		{
			_sampler?.Dispose();
			_sampler = value;

			Set?.Dispose();
			Set = null;
		}
	}

	public ResourceSet? Set { get; private set; }

	public VeldridTextureResources(Texture texture, Sampler sampler)
	{
		Texture = texture;
		Sampler = sampler;
	}

	public ResourceSet GetResourceSet(VeldridRenderer renderer, ResourceLayout layout)
	{
		if (Sampler == null)
			throw new InvalidOperationException("Attempting to create resource set without a sampler attached to the resources.");

		return Set ??= renderer.Factory.CreateResourceSet(new ResourceSetDescription(layout, Texture, Sampler));
	}

	public void Dispose()
	{
		Texture.Dispose();
		Sampler?.Dispose();
		Set?.Dispose();
	}
}
