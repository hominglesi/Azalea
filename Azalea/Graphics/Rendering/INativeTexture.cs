using Azalea.Graphics.Textures;

namespace Azalea.Graphics.Rendering;

internal interface INativeTexture
{
    IRenderer Renderer { get; }
    int Width { get; }
    int Height { get; }

    void SetData(ITextureUpload upload);
}
