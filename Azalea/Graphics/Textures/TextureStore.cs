using Azalea.Graphics.Rendering;
using Azalea.IO.Stores;
using System.Collections.Generic;
using System.IO;

namespace Azalea.Graphics.Textures;

public class TextureStore : ITextureStore
{
    private readonly Dictionary<string, Texture> textureCache = new();

    private readonly ResourceStore<TextureUpload> _uploadStore = new();

    private readonly IRenderer _renderer;

    public TextureStore(IRenderer renderer, IResourceStore<TextureUpload>? store = null)
    {
        if (store is not null)
            AddTextureStore(store);

        _renderer = renderer;
    }

    public virtual void AddTextureStore(IResourceStore<TextureUpload> store) => _uploadStore.AddStore(store);

    public virtual Texture? Get(string name)
    {
        var texture = get(name);

        return texture;
    }

    public Stream? GetStream(string name)
    {
        var stream = _uploadStore.GetStream(name);

        return stream;
    }

    public IEnumerable<string> GetAvalibleResources()
    {
        return _uploadStore.GetAvalibleResources();
    }

    private Texture? get(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;

        if (TryGetCached(name, out Texture? cached))
            return cached;

        Texture? tex = null;

        tex = loadRaw(_uploadStore.Get(name));

        if (tex is not null) return CacheAndReturnTexture(name, tex);
        else return null;
    }

    protected virtual bool TryGetCached(string lookupKey, out Texture? texture)
    {
        return textureCache.TryGetValue(lookupKey, out texture);
    }

    private Texture? loadRaw(TextureUpload? upload)
    {
        if (upload is null) return null;

        Texture? tex = null;

        tex ??= _renderer.CreateTexture(upload.Width, upload.Height);
        tex.SetData(upload);

        return tex;
    }

    protected virtual Texture? CacheAndReturnTexture(string lookupKey, Texture texture)
    {
        return textureCache[lookupKey] = texture;
    }
}
