using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using Azalea.Numerics;
using System.Numerics;

namespace Azalea.Graphics.Sprites;
public class SpritePattern : Sprite
{
	protected override void DrawTexture(IRenderer renderer, ITexture texture)
	{
		var patternSize = DrawSize / texture.Size;
		var textureUV = new Rectangle(Vector2.Zero, patternSize);

		renderer.DrawQuad(texture.GetNativeTexture(), ScreenSpaceDrawQuad, DrawColorInfo, textureUV);
	}
}
