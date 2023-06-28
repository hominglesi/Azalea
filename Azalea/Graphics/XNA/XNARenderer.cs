using Azalea.Graphics.Rendering;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Graphics.XNA.Batches;
using Azalea.Platform.XNA;
using Microsoft.Xna.Framework.Graphics;

namespace Azalea.Graphics.XNA;

internal class XNARenderer : Renderer
{
    private readonly GameWrapper _gameWrapper;

    public XNARenderer(GameWrapper gameWrapper)
    {
        _gameWrapper = gameWrapper;
    }

    protected internal override void Initialize()
    {
        base.Initialize();
        _gameWrapper.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
    }

    protected override void ClearImplementation(Color color)
    {
        _gameWrapper.GraphicsDevice.Clear(color.ToXNAColor());
    }

    protected override IVertexBatch<TexturedVertex2D> CreateQuadBatch(int size)
        => new XNAVertexBatch<TexturedVertex2D>(this, _gameWrapper, size);

    protected override INativeTexture CreateNativeTexture(int width, int height)
        => throw new NotImplementedException();

    protected override bool SetTextureImplementation(INativeTexture? texture, int unit)
    {
        throw new NotImplementedException();
    }
}
