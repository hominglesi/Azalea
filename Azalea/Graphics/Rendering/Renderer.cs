using Azalea.Graphics.Rendering.Vertices;
using Azalea.Graphics.Textures;

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

    protected abstract bool SetTextureImplementation(INativeTexture? texture, int unit);

    protected abstract IVertexBatch<TexturedVertex2D> CreateQuadBatch(int size);
    protected abstract INativeTexture CreateNativeTexture(int width, int height);
    public Texture CreateTexture(int width, int height)
        => CreateTexture(CreateNativeTexture(width, height));

    internal Texture CreateTexture(INativeTexture nativeTexture)
        => new(nativeTexture);

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

    internal bool BindTexture(Texture texture)
    {
        return BindTexture(texture.NativeTexture);
    } 

    internal bool BindTexture(INativeTexture nativeTexture)
    {
        FlushCurrentBatch();
        SetTextureImplementation(nativeTexture, 0);
        return true;
    }

    void IRenderer.Initialize() => Initialize();
    IVertexBatch<TexturedVertex2D> IRenderer.DefaultQuadBatch => defaultQuadBatch ?? throw new Exception("Cannot call DefaultQuadBatch before Initialization");
    void IRenderer.FlushCurrentBatch() => FlushCurrentBatch();
    IVertexBatch IRenderer.CreateQuadBatch(int size) => CreateQuadBatch(size);
    bool IRenderer.BindTexture(Azalea.Graphics.Textures.Texture texture) => BindTexture(texture);
}
