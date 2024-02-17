using Azalea.Graphics.Textures;
using System;

namespace Azalea.Graphics.Rendering;

internal interface INativeTexture : IDisposable
{
	IRenderer Renderer { get; }
	uint Handle { get; }
	int Width { get; }
	int Height { get; }

	void SetData(Image upload);
	void SetFiltering(TextureFiltering minFilter, TextureFiltering magFilter);
}
