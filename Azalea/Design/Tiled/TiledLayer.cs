using Azalea.Graphics;
using Azalea.Graphics.Primitives;
using Azalea.Graphics.Rendering;
using Azalea.IO.Tiled;
using System.Numerics;
using static Azalea.IO.Tiled.Tilemap;

namespace Azalea.Design.Tiled;
public class TiledLayer : GameObject
{
	protected Tilemap Tilemap { get; init; }
	protected TilemapLayer Layer { get; init; }

	public TiledLayer(Tilemap tilemap, TilemapLayer layer)
	{
		Tilemap = tilemap;
		Layer = layer;

		Width = tilemap.Width * tilemap.TileWidth;
		Height = tilemap.Height * tilemap.TileHeight;
	}

	public TiledLayer(Tilemap tilemap, int layer)
		: this(tilemap, tilemap.Layers[layer]) { }

	protected override DrawNode CreateDrawNode()
		=> new TiledLayerDrawNode(this);

	private class TiledLayerDrawNode : DrawNode
	{
		protected Quad ScreenSpaceDrawQuad { get; set; }
		protected Tilemap Tilemap { get; set; }
		protected TilemapLayer Layer { get; set; }

		protected new TiledLayer Source => (TiledLayer)base.Source;

		public TiledLayerDrawNode(IGameObject source)
			: base(source) { }

		public override void ApplyState()
		{
			base.ApplyState();

			Tilemap = Source.Tilemap;
			Layer = Source.Layer;
			ScreenSpaceDrawQuad = Source.ScreenSpaceDrawQuad;
		}

		public override void Draw(IRenderer renderer)
		{
			base.Draw(renderer);

			var startPosition = ScreenSpaceDrawQuad.TopLeft;
			var tileSize = ScreenSpaceDrawQuad.Size / new Vector2(Tilemap.Width, Tilemap.Height);

			for (int i = 0; i < Tilemap.Height; i++)
			{
				for (int j = 0; j < Tilemap.Width; j++)
				{
					var tileValue = Layer.Data[i, j];
					if (tileValue == 0) continue;

					var texture = Tilemap.GetTextureById(tileValue);
					var offset = new Vector2(tileSize.X * j, tileSize.Y * i);

					if (texture.Height > Tilemap.TileHeight)
					{
						var difference = texture.Height / (float)Tilemap.TileHeight;
						difference--;

						offset.Y -= difference * tileSize.Y;
					}

					var allowedPadding = new Vector2(1);

					var sizeRatio = texture.Size / Tilemap.TileSize;
					var quad = new Quad(startPosition + offset - allowedPadding, tileSize * sizeRatio + allowedPadding);

					renderer.DrawQuad(
						texture,
						quad,
						DrawColorInfo);
				}
			}
		}
	}

}
