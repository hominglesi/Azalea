using Azalea.Graphics.OpenGL.Batches;
using Azalea.Graphics.OpenGL.Textures;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Platform;
using Silk.NET.OpenGL;

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
        _gl.ClearColor(value.RNormalized, value.GNormalized, value.BNormalized, value.ANormalized);
    }

    protected override void ClearImplementation(Color color)
    {
        _gl.Clear(ClearBufferMask.ColorBufferBit);
    }

    protected override IVertexBatch<TexturedVertex2D> CreateQuadBatch(int size)
        => new GLVertexBatch<TexturedVertex2D>(this, _gl, _window, size);

    protected override INativeTexture CreateNativeTexture(int width, int height)
        => new GLTexture(this, _gl, width, height);

    protected override bool SetTextureImplementation(INativeTexture? texture, int unit)
    {
        if (texture is null)
        {
            _gl.ActiveTexture(TextureUnit.Texture0 + unit);
            _gl.BindTexture(TextureTarget.Texture2D, 0);
            return true;
        }

        switch (texture)
        {
            case GLTexture glTexture:
                if (glTexture.TextureId <= 0)
                    return false;

                _gl.ActiveTexture(TextureUnit.Texture0 + unit);
                _gl.BindTexture(TextureTarget.Texture2D, glTexture.TextureId);
                break;
        }

        return true;
    }
}
