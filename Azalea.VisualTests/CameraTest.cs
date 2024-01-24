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
	private CameraContainer _worldContainer;
	private Sprite _player;

	public CameraTest()
	{
		var tilemap = Assets.MainStore.GetTilemap("MapForTiled/FirstMap.tmx");

		Add(_worldContainer = new CameraContainer());

		foreach (var layer in tilemap.Layers)
		{
			_worldContainer.Add(new TiledLayer(tilemap, layer));
		}

		foreach (var obj in tilemap.Objects)
		{
			if (obj.TileId != -1)
			{
				//var pos = new Vector2(obj.X, obj.Y) * zoomMultiplier / new Vector2(1280, 720);
				//var size = new Vector2(obj.Width, obj.Height) * zoomMultiplier / new Vector2(1280, 720);

				_worldContainer.Add(new Sprite()
				{
					Texture = tilemap.GetTextureById(obj.TileId),
					//RelativePositionAxes = Axes.Both,
					//RelativeSizeAxes = Axes.Both,
					Position = obj.Position,
					Size = obj.Size,
					Origin = Graphics.Anchor.BottomLeft,
					Depth = -obj.Y + 5
				});
			}
		}

		_worldContainer.Add(_player = new Sprite()
		{
			Texture = Assets.GetTexture("Textures/azalea-icon.png"),
			Position = new(770, 525),
			Size = new(64, 64),
			Origin = Graphics.Anchor.BottomCenter
		});

		_worldContainer.Scale = new(2f);

		_worldContainer.SetBoundaries(new(Vector2.Zero, tilemap.PixelSize));
		_worldContainer.CenterOnObject(_player);
		_worldContainer.SetFollowedObject(_player);
	}

	protected override void Update()
	{
		var movement = Input.GetDirectionalMovement();
		if (movement != Vector2.Zero)
		{
			_player.Position += movement * 3 * Time.DeltaTime * 60;
			_worldContainer.ChangeChildDepth(_player, -_player.Y);
		}
	}
}
