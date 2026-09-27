using Azalea;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Rendering;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using Azalea.IO.Resources;
using Azalea.Platform;
using Azalea.Platform.Rendering;
using Azalea.Platform.Windowing;
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
		var assemblyStore = new NamespacedResourceStore(new EmbeddedResourceStore(typeof(MemoryGame).Assembly), "Resources");

		Assets.AddToMainStore(assemblyStore);
		_tilesStore = new NamespacedResourceStore(assemblyStore, "Textures/Tiles");
		_images = new ImagePool(_tilesStore);

		Assets.MainStore.AddMsdfFont("Roboto-Medium", "Fonts/Roboto-Medium.csv", "Fonts/Roboto-Medium.bmp");

		Add(_field = new MemoryField(new Vector2(0.72f, 0.98f), new Vector2Int(4, 4), _images)
		{
			Position = new Vector2(0.01f, 0.5f),
			RelativePositionAxes = Axes.Both,
			Origin = Anchor.CenterLeft,
			RelativeSizeAxes = Axes.Both
		});

		_logic = new MemoryLogic(_field);
	}

	protected override void Initialize()
	{
		base.Initialize();
		App.Renderer.SetClearColor(new Color(189, 223, 214));
		App.Window.SetResizable(true);
	}

	protected override bool OnKeyDown(KeyDownEvent e)
	{
		if(e.Key == Keys.P)
		{
			_logic?.Solve();
			return true;
		}

		return false;
	}
}
