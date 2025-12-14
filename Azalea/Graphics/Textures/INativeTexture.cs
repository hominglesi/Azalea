using Azalea.Graphics.Rendering;
using System;

namespace Azalea.Graphics.Textures;

internal interface INativeTexture : IDisposable
{
	IRenderer Renderer { get; }
	int Width { get; }
	int Height { get; }

	void SetData(Image upload);
	void SetFiltering(TextureFiltering minFilter, TextureFiltering magFilter);
}
