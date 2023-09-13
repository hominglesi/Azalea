using Azalea.Graphics.Textures;
using System.IO;

namespace Azalea.IO.Assets;

public static partial class Assets
{
	public static Texture? GetTexture(string path) => Textures.Get(path);

	public static Stream? GetTextureStream(string path) => Textures.GetStream(path);
}

