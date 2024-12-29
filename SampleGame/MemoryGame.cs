using Azalea;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Rendering;
using Azalea.Inputs;
using Azalea.IO.Resources;
using Azalea.Platform;
using SampleGame.Elements;
using System.Numerics;

namespace SampleGame;

public class MemoryGame : AzaleaGame
{
	private MemoryField? _field;
	private MemoryLogic? _logic;
	private ImagePool? _images;

	private IResourceStore? _tilesStore;

	public MemoryGame()
	{
		Renderer.ClearColor = new Color(189, 223, 214);
		Window.Resizable = true;

		var assemblyStore = new NamespacedResourceStore(new EmbeddedResourceStore(typeof(MemoryGame).Assembly), "Resources");

		Assets.AddToMainStore(assemblyStore);
		_tilesStore = new NamespacedResourceStore(assemblyStore, "Textures/Tiles");
		_images = new ImagePool(_tilesStore);

		Assets.AddFont("Fonts/Roboto-Medium.fnt", "Roboto-Medium");

		Add(_field = new MemoryField(new Vector2(0.72f, 0.98f), new Vector2Int(4, 4), _images)
		{
			Position = new Vector2(0.01f, 0.5f),
			RelativePositionAxes = Axes.Both,
			Origin = Anchor.CenterLeft,
			RelativeSizeAxes = Axes.Both
		});

		_logic = new MemoryLogic(_field);
	}

	protected override void Update()
	{
		if (Input.GetKey(Keys.P).Down) _logic?.Solve();
	}
}
