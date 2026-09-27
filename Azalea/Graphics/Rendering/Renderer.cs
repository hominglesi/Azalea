using Azalea.Graphics.Textures;
using Azalea.Platform;

namespace Azalea.Graphics.Rendering;
public static class Renderer
{
	private static IRenderer? _instance;
	public static IRenderer Instance => _instance ??= GameHost.Instance.Renderer;

	internal static Texture CreateTexture(Image image) => Instance.CreateTexture(image);
}
