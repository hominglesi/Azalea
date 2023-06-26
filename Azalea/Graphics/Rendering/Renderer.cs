using Azalea.Graphics.Rendering.Vertices;

namespace Azalea.Graphics.Rendering;

internal abstract class Renderer : IRenderer
{
    private IVertexBatch<TexturedVertex2D>? defaultQuadBatch;
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

    public bool AutomaticallyClear { get; set; } = true;

    protected internal virtual void Initialize()
    {
        defaultQuadBatch = CreateQuadBatch(100);
        currentActiveBatch = defaultQuadBatch;
    }

    protected internal abstract IVertexBatch<TexturedVertex2D> CreateQuadBatch(int size);

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
    void IRenderer.Initialize() => Initialize();
    IVertexBatch<TexturedVertex2D> IRenderer.DefaultQuadBatch => defaultQuadBatch ?? throw new Exception("Cannot call DefaultQuadBatch before Initialization");
    void IRenderer.FlushCurrentBatch() => FlushCurrentBatch();
    IVertexBatch IRenderer.CreateQuadBatch(int size) => CreateQuadBatch(size);
}
