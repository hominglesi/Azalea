using Azalea.Numerics;
using StbImageSharp;
using System;
using System.IO;

namespace Azalea.Graphics;

public class Image
{
	private byte[] _data;
	private int _width;
	private int _height;

	public int Width => _width;
	public int Height => _height;
	public byte[] Data => _data;

	public Image(int width, int height, byte[] data)
	{
		_width = width;
		_height = height;
		_data = data;
	}

	public Image(int width, int height)
		: this(width, height, new byte[width * height * 4])
	{

	}

	public void CopyFromSource(Image source, RectangleInt sourceArea, RectangleInt targetArea)
	{
		if (sourceArea.Size != targetArea.Size)
			throw new Exception("Source and target sizes must be same size");

		for (int i = 0; i < sourceArea.Height; i++)
		{
			var verticalOffsetSource = (sourceArea.Y + i) * source.Width;
			var verticalOffsetTarget = (targetArea.Y + i) * _width;

			for (int j = 0; j < sourceArea.Width; j++)
			{
				var offsetSource = verticalOffsetSource + sourceArea.X + j;
				var offsetTarget = verticalOffsetTarget + targetArea.X + j;

				_data[offsetTarget * 4] = source.Data[offsetSource * 4];
				_data[offsetTarget * 4 + 1] = source.Data[offsetSource * 4 + 1];
				_data[offsetTarget * 4 + 2] = source.Data[offsetSource * 4 + 2];
				_data[offsetTarget * 4 + 3] = source.Data[offsetSource * 4 + 3];
			}
		}
	}

	public unsafe static Image FromStream(Stream stream)
	{
		ImageResult? image = null;

		if (stream.CanSeek)
			image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
		else
		{
			// StbImageSharp requires that all streams are seekable so
			// in case they arent we save the stream in memory and pass that
			using var memoryStream = new MemoryStream();
			stream.CopyTo(memoryStream);
			var data = memoryStream.ToArray();

			image = ImageResult.FromMemory(data, ColorComponents.RedGreenBlueAlpha);
		}

		return new Image(image.Width, image.Height, image.Data);
	}
}
