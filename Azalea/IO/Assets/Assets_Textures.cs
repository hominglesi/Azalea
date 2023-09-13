using Azalea.Graphics.Textures;
using System;
using System.IO;

namespace Azalea.IO.Assets;

public static partial class Assets
{
	private const string missing_texture_path = "missing-texture.png";

	public static Texture GetTexture(string path)
	{
		return Textures.Get(path) ?? Textures.Get(missing_texture_path) ?? throw new Exception("Missing texture could not be found.");
	}

	public static Stream? GetTextureStream(string path) => Textures.GetStream(path);
}

