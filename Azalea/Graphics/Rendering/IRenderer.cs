using Azalea.Graphics.Rendering.Vertices;

namespace Azalea.Graphics.Rendering;

public interface IRenderer
{
    public const int VERTICES_PER_QUAD = 4;
    public const int INDICES_PER_QUAD = VERTICES_PER_QUAD + 2;

    public Color ClearColor { get; set; }
    internal IVertexBatch<PositionColorVertex> DefaultQuadBatch { get; }

    internal void Initialize();
    public void Clear();
    internal void FlushCurrentBatch();

    internal IVertexBatch CreateQuadBatch(int size);
}
