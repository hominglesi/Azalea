using Azalea.Graphics.Textures;
using Azalea.Utils;
using System.Diagnostics;

namespace SampleGame.Elements;

public class ImagePool
{
    private readonly string[] _images;
    private ITextureStore _store;
    public ImagePool(ITextureStore tilesStore)
    {
        _store = tilesStore;
        _images = (string[])tilesStore.GetAvalibleResources();
    }

    public List<Texture> GenerateTiles(int count)
    {
        if (count % 2 != 0) throw new ArgumentException($"There must be an even number of tiles");
        if (count / 2 > _images.Length) throw new ArgumentException($"There are not enough images to generate enough tiles");

        count /= 2;

        var remainingImages = new List<string>(_images);
        var tiles = new List<Texture>();

        while (count > 0)
        {
            var tile = remainingImages.Random();
            var tileTexture = _store.Get(tile);
            Debug.Assert(tileTexture != null);

            tileTexture.AssetName = tile;
            remainingImages.Remove(tile);
            tiles.Add(tileTexture);
            tiles.Add(tileTexture);

            count--;
        }

        return tiles;
    }
}
