using Azalea.Graphics.Rendering;
using Azalea.Numerics;
using Azalea.Platform.Rendering.Coordination;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Graphics.Textures;
public interface ITexture
{
	public Rectangle GetUVCoordinates(float time = 0);

	public int Width { get; }
	public int Height { get; }
	public Vector2 Size { get; }

	public void Bind(RenderCoordinator coordinator, float time = 0);
}
