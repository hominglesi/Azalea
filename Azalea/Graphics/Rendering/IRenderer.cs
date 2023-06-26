using Azalea.Graphics.Rendering.Vertices;
using Azalea.Graphics.Textures;

namespace Azalea.Graphics.Rendering;

public interface IRenderer
{
    public const int VERTICES_PER_QUAD = 4;
    public const int INDICES_PER_QUAD = VERTICES_PER_QUAD + 2;

    public Color ClearColor { get; set; }
    public bool AutomaticallyClear { get; set; }
    internal IVertexBatch<TexturedVertex2D> DefaultQuadBatch { get; }

    internal void Initialize();
    public void Clear();
    internal void FlushCurrentBatch();

    internal IVertexBatch CreateQuadBatch(int size);
    internal Texture CreateTexture(int width, int height);
}
