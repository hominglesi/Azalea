using Azalea.Design.Containers;
using Azalea.Design.Tiled;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.IO.Resources;
using Azalea.Platform;
using System.Numerics;

namespace Azalea.VisualTests;
internal class CameraTest : TestScene
{
	private PannableContainer _worldContainer;
	private Sprite _player;

	public CameraTest()
	{
		var tilemap = Assets.MainStore.GetTilemap("MapForTiled/FirstMap.tmx");

		Add(_worldContainer = new PannableContainer());

		foreach (var layer in tilemap.Layers)
		{
			_worldContainer.Add(new TiledLayer(tilemap, layer));
		}

		_worldContainer.Add(_player = new Sprite()
		{
			Texture = Assets.GetTexture("Textures/azalea-icon.png"),
			Position = new(770, 525),
			Size = new(64, 64),
		});

		_worldContainer.Scale = new(2f);

		_worldContainer.SetBoundaries(new(Vector2.Zero, tilemap.PixelSize));
		_worldContainer.CenterOnObject(_player);
		_worldContainer.SetFollowedObject(_player);
	}

	protected override void Update()
	{
		var movement = Input.GetDirectionalMovement();
		_player.Position += movement * 3 * Time.DeltaTime * 60;
	}
}
