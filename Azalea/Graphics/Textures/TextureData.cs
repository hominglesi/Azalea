using StbImageSharp;
using System.IO;

namespace Azalea.Graphics.Textures;

public class TextureData : ITextureData
{
	public byte[] Data => _image.Data;

	public int Width => _image?.Width ?? 0;
	public int Height => _image?.Height ?? 0;

	private readonly ImageResult _image;

	public TextureData(ImageResult image)
	{
		_image = image;
	}

	public TextureData(Stream stream)
		: this(LoadFromStream(stream))
	{

	}

	internal static ImageResult LoadFromStream(Stream stream)
	{
		return ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
	}

	static TextureData()
	{
		StbImage.stbi_set_flip_vertically_on_load(1);
	}
}
