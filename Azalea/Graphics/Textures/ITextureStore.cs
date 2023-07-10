using Azalea.IO.Stores;

namespace Azalea.Graphics.Textures;

public interface ITextureStore : IResourceStore<Texture>
{
    new Texture? Get(string name);
}
