using Azalea.Graphics.Rendering;
using Azalea.IO.Stores;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Azalea.Graphics.Textures;

public class TextureStore : ITextureStore
{
    private readonly Dictionary<string, Texture> textureCache = new();

    private readonly ResourceStore<TextureUpload> _uploadStore = new();
    private readonly List<ITextureStore> _nestedStores = new();

    private readonly IRenderer _renderer;

    public readonly float ScaleAdjust;

    public TextureStore(IRenderer renderer, IResourceStore<TextureUpload>? store = null, float scaleAdjust = 2)
    {
        if (store is not null)
            AddTextureSource(store);

        _renderer = renderer;
        ScaleAdjust = scaleAdjust;
    }

    public virtual void AddTextureSource(IResourceStore<TextureUpload> store) => _uploadStore.AddStore(store);

    public virtual void AddStore(ITextureStore store) => _nestedStores.Add(store);
    public virtual void RemoveStore(ITextureStore store) => _nestedStores.Remove(store);

    public virtual Texture? Get(string name)
    {
        var texture = get(name);

        if (texture is null)
        {
            foreach (var nested in _nestedStores)
            {
                if ((texture = nested.Get(name)) != null)
                    break;
            }
        }

        return texture;
    }

    public Stream? GetStream(string name)
    {
        var stream = _uploadStore.GetStream(name);

        if (stream is null)
        {
            foreach (var nested in _nestedStores)
            {
                if ((stream = nested.GetStream(name)) != null)
                    break;
            }
        }

        return stream;
    }

    public IEnumerable<string> GetAvalibleResources()
    {
        return _uploadStore.GetAvalibleResources().Concat(_nestedStores.SelectMany(s => s.GetAvalibleResources()).ExcludeSystemFileNames()).ToArray();
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
        tex.ScaleAdjust = ScaleAdjust;
        tex.SetData(upload);

        return tex;
    }

    protected virtual Texture? CacheAndReturnTexture(string lookupKey, Texture texture)
    {
        return textureCache[lookupKey] = texture;
    }
}
