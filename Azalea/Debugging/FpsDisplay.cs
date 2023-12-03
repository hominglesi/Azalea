using Azalea.Graphics.Sprites;
using Azalea.Platform;

namespace Azalea.Debugging;
internal class FpsDisplay : SpriteText
{
	protected override void Update()
	{
		Text = Time.FpsCount.ToString();
	}
}
