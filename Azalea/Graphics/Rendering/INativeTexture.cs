using Azalea.Graphics.Textures;
using System;

namespace Azalea.Graphics.Rendering;

internal interface INativeTexture : IDisposable
{
	IRenderer Renderer { get; }
	int Width { get; }
	int Height { get; }

	void SetFiltering(TextureFiltering minFilter, TextureFiltering magFilter);
}
