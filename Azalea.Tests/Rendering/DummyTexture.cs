using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using System;

namespace Azalea.Tests.Rendering;

internal class DummyTexture : INativeTexture
{
	protected readonly DummyRenderer Renderer;
	IRenderer INativeTexture.Renderer => Renderer;

	public int Width { get; set; }
	public int Height { get; set; }

	public DummyTexture(DummyRenderer renderer)
	{
		Renderer = renderer;
	}

	public void SetData(ITextureUpload upload)
	{
		throw new NotImplementedException();
	}
}
