using Azalea.Graphics.Rendering;
using System;
using System.IO;
using System.Numerics;

namespace Azalea.Graphics.Textures;

public class Texture : IDisposable
{
    internal virtual INativeTexture NativeTexture { get; }
    public string AssetName = string.Empty;

    public float ScaleAdjust = 1;

    public float DisplayWidth => Width / ScaleAdjust;
    public float DisplayHeight => Height / ScaleAdjust;

    internal Texture(INativeTexture nativeTexture)
    {
        NativeTexture = nativeTexture ?? throw new ArgumentNullException(nameof(nativeTexture));
    }

    public int Width => NativeTexture.Width;

    public int Height => NativeTexture.Height;

    public Vector2 Size => new(Width, Height);

    public static Texture? FromStream(IRenderer renderer, Stream? stream)
    {
        if (stream is null || stream.Length == 0)
            return null;

        var data = new TextureUpload(stream);
        Texture texture = renderer.CreateTexture(data.Width, data.Height);
        texture.SetData(data);
        return texture;
    }

    internal void SetData(ITextureUpload upload) => setData(upload);

    internal virtual void setData(ITextureUpload upload)
    {
        NativeTexture.SetData(upload);
    }

    public override string ToString() => $@"{AssetName} ({Width}, {Height})";

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool isDisposing) { }
}
