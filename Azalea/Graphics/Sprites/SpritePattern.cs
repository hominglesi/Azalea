using Azalea.Graphics.Textures;
using Azalea.Numerics;
using Azalea.Platform;
using Azalea.Platform.Rendering.Coordination;
using System.Diagnostics;
using System.Numerics;

namespace Azalea.Graphics.Sprites;

public class SpritePattern : Sprite
{
	public override void Draw(RenderCoordinator coordinator)
	{
		Debug.Assert(Texture is Texture);

		var tex = (Texture)Texture;
		var patternSize = DrawSize / tex.Size;
		var textureUV = new Rectangle(Vector2.Zero, patternSize);

		coordinator.BindShader(Shader ?? GameHost.Instance.Loader.DefaultQuadShader);
		coordinator.BindTexture(tex.NativeTexture);
		coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, ScreenSpaceDrawQuad, DrawColorQuad, textureUV);
	}
}
