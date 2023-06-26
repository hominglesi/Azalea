using Azalea.Graphics.OpenGL.Batches;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Rendering.Vertices;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Azalea.Graphics.OpenGL;

internal class GLRenderer : Renderer
{
    private readonly GL _gl;
    private readonly IWindow _window;

    public GLRenderer(GL gl, IWindow window)
    {
        _gl = gl;
        _window = window;
    }

    protected internal override void SetClearColor(Color value)
    {
        _gl.ClearColor(value.RNormalized, value.GNormalized, value.BNormalized, value.ANormalized );
    }

    protected override void ClearImplementation(Color color)
    {
        _gl.Clear(ClearBufferMask.ColorBufferBit);
    }

    protected internal override IVertexBatch<TexturedVertex2D> CreateQuadBatch(int size)
        => new GLVertexBatch<TexturedVertex2D>(this, _gl, _window, size);
}
