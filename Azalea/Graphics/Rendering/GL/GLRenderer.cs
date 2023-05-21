using Silk.NET.OpenGL;

namespace Azalea.Graphics.Rendering.Silk;

internal class GLRenderer : Renderer
{
    private readonly GL _gl;

    public GLRenderer(GL gl)
    {
        _gl = gl;
    }

    protected override void SetClearColor(Color value)
    {
        _gl.ClearColor(value.R, value.G, value.B, value.A);
    }

    protected override void ClearImplementation(Color color)
    {
        _gl.Clear(ClearBufferMask.ColorBufferBit);
    }
}
