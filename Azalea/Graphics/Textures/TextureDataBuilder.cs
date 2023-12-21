using Azalea.Numerics;
using StbImageSharp;
using System;

namespace Azalea.Graphics.Textures;
public class TextureDataBuilder
{
	private byte[] _data;
	private int _width;
	private int _height;

	public TextureDataBuilder(Vector2Int size)
	{
		_width = size.X;
		_height = size.Y;
		_data = new byte[size.X * size.Y * 4];
	}

	internal void CopyFromTexture(ITextureData data, RectangleInt source, RectangleInt target)
	{
		if (source.Size != target.Size)
			throw new Exception("Source and target must be same size");

		for (int i = 0; i < source.Height; i++)
		{
			var verticalOffsetSource = (source.Y + i) * data.Width;
			var verticalOffsetTarget = (target.Y + i) * _width;

			for (int j = 0; j < source.Width; j++)
			{
				var offsetSource = verticalOffsetSource + source.X + j;
				var offsetTarget = verticalOffsetTarget + target.X + j;

				_data[offsetTarget * 4] = data.Data[offsetSource * 4];
				_data[offsetTarget * 4 + 1] = data.Data[offsetSource * 4 + 1];
				_data[offsetTarget * 4 + 2] = data.Data[offsetSource * 4 + 2];
				_data[offsetTarget * 4 + 3] = data.Data[offsetSource * 4 + 3];
			}
		}
	}

	public Texture Create()
	{
		return Texture.FromData(AzaleaGame.Main.Host.Renderer, new TextureData(new ImageResult
		{
			Width = _width,
			Height = _height,
			Data = _data,
			Comp = ColorComponents.RedGreenBlueAlpha
		}))!;
	}
}
