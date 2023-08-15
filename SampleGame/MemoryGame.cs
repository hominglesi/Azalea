using Azalea;
using Azalea.Graphics;
using Azalea.Graphics.Textures;
using Azalea.IO.Stores;
using SampleGame.Elements;
using System.Numerics;

namespace SampleGame;

public class MemoryGame : AzaleaGame
{
    private MemoryField? _field;
    private MemoryLogic? _logic;
    private ImagePool? _images;

    private TextureStore? _tilesStore;

    protected override void OnInitialize()
    {
        Host.Renderer.ClearColor = new Color(189, 223, 214);

        Resources.AddStore(new NamespacedResourceStore<byte[]>(new DllResourceStore(typeof(MemoryGame).Assembly), "Resources"));
        _tilesStore = new TextureStore(Host.Renderer,
            Host.CreateTextureLoaderStore(new NamespacedResourceStore<byte[]>(Resources, "Textures/Tiles")));
        _images = new ImagePool(_tilesStore);

        AddFont(Resources, @"Fonts/Roboto-Medium");

        Add(_field = new MemoryField(new Vector2(0.7f, 0.95f), new Vector2Int(4, 4), _images)
        {
            Position = new Vector2(10, 10),
            RelativeSizeAxes = Axes.Both
        });

        _logic = new MemoryLogic(_field);
    }
}
