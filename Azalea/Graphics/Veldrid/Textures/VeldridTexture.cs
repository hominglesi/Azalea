using Azalea.Extentions.ImageExtentions;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using Veldrid;

using TextureVeldrid = Veldrid.Texture;

namespace Azalea.Graphics.Veldrid.Textures;

internal class VeldridTexture : INativeTexture
{
	protected readonly VeldridRenderer Renderer;
	IRenderer INativeTexture.Renderer => Renderer;

	public int Width { get; set; }
	public int Height { get; set; }

	public VeldridTexture(VeldridRenderer renderer, int width, int height)
	{
		Renderer = renderer;
		Width = width;
		Height = height;
	}

	public void SetData(ITextureUpload upload)
	{
		TextureVeldrid? texture = resources?.Texture;
		Sampler? sampler = resources?.Sampler;

		if (texture == null || texture.Width != Width || texture.Height != Height)
		{
			texture?.Dispose();

			uint mipmaps = Width * Height > 16 ? (uint)4 : 1;

			var textureDescription = TextureDescription.Texture2D((uint)Width, (uint)Height, mipmaps, 1,
				PixelFormat.R8_G8_B8_A8_UNorm, TextureUsage.Sampled | TextureUsage.RenderTarget);
			texture = Renderer.Factory.CreateTexture(textureDescription);

			using var image = new Image<Rgba32>(Width, Height);
			using var pixels = image.CreateReadOnlyPixelSpan();

			Renderer.UpdateTexture(texture, 0, 0, Width, Height, pixels.Span);
		}

		if (upload.Data.IsEmpty == false)
		{
			Renderer.UpdateTexture(texture, 0, 0, Width, Height, upload.Data);
		}

		if (sampler == null)
		{
			var samplerDescription = new SamplerDescription()
			{
				AddressModeU = SamplerAddressMode.Clamp,
				AddressModeV = SamplerAddressMode.Clamp,
				AddressModeW = SamplerAddressMode.Clamp,
				Filter = SamplerFilter.MinLinear_MagLinear_MipLinear,
				MinimumLod = 0,
				MaximumLod = IRenderer.MAX_MIPMAP_LEVELS,
				MaximumAnisotropy = 0
			};

			sampler = Renderer.Factory.CreateSampler(samplerDescription);
		}

		resources = new VeldridTextureResources(texture, sampler);
	}

	private readonly VeldridTextureResources?[] resourcesArray = new VeldridTextureResources?[1];

	private VeldridTextureResources? resources
	{
		get => resourcesArray[0];
		set => resourcesArray[0] = value;
	}

	public virtual IReadOnlyList<VeldridTextureResources> GetResourceList()
	{
		if (resources == null)
			return Array.Empty<VeldridTextureResources>();

		return resourcesArray!;
	}
}
