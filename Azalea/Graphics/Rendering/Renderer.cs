namespace Azalea.Graphics.Rendering;

internal abstract class Renderer : IRenderer
{
    private IVertexBatch? currentActiveBatch;

    private Color _clearColor;
    public Color ClearColor
    {
        get => _clearColor;
        set
        {
            if (_clearColor == value) return;
            _clearColor = value;
            SetClearColor(value);
        }
    }

    protected internal virtual void SetClearColor(Color value) { }

    public void Clear()
    {
        ClearImplementation(ClearColor);
    }

    protected abstract void ClearImplementation(Color color);

    protected internal void FlushCurrentBatch()
    {
        currentActiveBatch?.Draw();
    }

    void IRenderer.FlushCurrentBatch() => FlushCurrentBatch();
}
