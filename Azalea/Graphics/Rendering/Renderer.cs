using Azalea.Graphics.Colors;
using Azalea.Graphics.Shaders;
using Azalea.Graphics.Textures;
using Azalea.Platform;

namespace Azalea.Graphics.Rendering;
public static class Renderer
{
	private static IRenderer? _instance;
	public static IRenderer Instance => _instance ??= GameHost.Main.Renderer;

	public static Color ClearColor
	{
		get => Instance.ClearColor;
		set => Instance.ClearColor = value;
	}

	public static bool AutomaticallyClear
	{
		get => Instance.AutomaticallyClear;
		set => Instance.AutomaticallyClear = value;
	}

	public static Texture WhitePixel => Instance.WhitePixel;

	public static void Clear() => Instance.Clear();

	internal static Texture CreateTexture(Image image)
		=> Instance.CreateTexture(image);

	internal static Shader CreateShader(string vertexCode, string fragmentCode)
		=> Instance.CreateShader(vertexCode, fragmentCode);
}
