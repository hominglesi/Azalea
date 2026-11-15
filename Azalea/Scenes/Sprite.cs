using Azalea.Graphics.Colors;
using Azalea.Graphics.Textures;
using Azalea.IO.Resources;
using Azalea.Numerics;
using Azalea.Platform;
using Azalea.Platform.Rendering.Coordination;

namespace Azalea.Scenes;

public class Sprite : Leaf
{
	public ITexture? Texture { get; set; }

	public override void Draw(RenderCoordinator coordinator)
	{
		if (Texture is Texture tex)
		{
			coordinator.BindShader(GameHost.Instance.Loader.DefaultQuadShader);
			coordinator.BindTexture(tex.NativeTexture);
			coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, WorldQuad, Palette.White, Rectangle.One);
		}
		else
		{
			coordinator.BindShader(GameHost.Instance.Loader.DefaultQuadShader);
			coordinator.BindTexture(Assets.MissingTexture.NativeTexture);
			coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, WorldQuad, Palette.White, Rectangle.One);
		}
	}
}
