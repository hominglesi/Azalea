using Azalea.Platform.XNA;

namespace Azalea.Graphics.Rendering.XNA;

internal class XNARenderer : Renderer
{
    private readonly GameWrapper _gameWrapper;

    public XNARenderer(GameWrapper gameWrapper)
    {
        _gameWrapper = gameWrapper;
    }

    protected override void ClearImplementation(Color color)
    {
        _gameWrapper.GraphicsDevice.Clear(color.ToXNAColor());
    }
}
