using Azalea.Graphics;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Tests.Rendering.Batches;

namespace Azalea.Tests.Rendering;

internal class DummyRenderer : Renderer
{
    protected override void ClearImplementation(Color color)
    {

    }

    protected internal override IVertexBatch<TexturedVertex2D> CreateQuadBatch(int size)
        => new DummyVertexBatch<TexturedVertex2D>();
}
