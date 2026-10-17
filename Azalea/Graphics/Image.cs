using Azalea.IO;
using Azalea.Numerics;
using StbImageSharp;
using System;
using System.IO;

namespace Azalea.Graphics;

public class Image(int width, int height, byte[] data)
{
	public byte[] Data { get; private init; } = data;
	public int Width { get; private init; } = width;
	public int Height { get; private init; } = height;

	public Image(int width, int height) : this(width, height, new byte[width * height * 4]) { }

	public void CopyFromSource(Image source, RectangleInt sourceArea, RectangleInt targetArea)
	{
		if (sourceArea.Size != targetArea.Size)
			throw new Exception("Source and target sizes must be same size");

		for (int i = 0; i < sourceArea.Height; i++)
		{
			var verticalOffsetSource = (sourceArea.Y + i) * source.Width;
			var verticalOffsetTarget = (targetArea.Y + i) * Width;

			for (int j = 0; j < sourceArea.Width; j++)
			{
				var offsetSource = verticalOffsetSource + sourceArea.X + j;
				var offsetTarget = verticalOffsetTarget + targetArea.X + j;

				Data[offsetTarget * 4] = source.Data[offsetSource * 4];
				Data[offsetTarget * 4 + 1] = source.Data[offsetSource * 4 + 1];
				Data[offsetTarget * 4 + 2] = source.Data[offsetSource * 4 + 2];
				Data[offsetTarget * 4 + 3] = source.Data[offsetSource * 4 + 3];
			}
		}
	}

	public unsafe static Image FromStream(Stream stream)
	{
		ImageResult? image = null;

		if (stream.CanSeek)
		{
			var origin = stream.Position;
			var isWebP = FFmpegImageReader.IsWebP(stream);

			stream.Position = origin;

			if (isWebP)
				return FFmpegImageReader.ReadWebP(stream);
			else
				image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
		}
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
