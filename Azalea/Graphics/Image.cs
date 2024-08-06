using Azalea.Numerics;
using StbImageSharp;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Azalea.Graphics;

public class Image
{
	private int _width;
	private int _height;
	private int _channelCount;
	private byte[] _data;

	public int Width => _width;
	public int Height => _height;
	public int ChannelCount => _channelCount;
	public byte[] Data => _data;

	public Image(int width, int height, int channelCount, byte[] data)
	{
		_width = width;
		_height = height;
		_channelCount = channelCount;
		_data = data;
	}

	public Image(int width, int height)
		: this(width, height, 4, new byte[width * height * 4])
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
		byte* ptr = null;
		try
		{
			int width = 0;
			int height = 0;
			int comp = 0;
			ptr = StbImage.stbi__load_and_postprocess_8bit(
				new StbImage.stbi__context(stream), &width, &height, &comp, (int)ColorComponents.RedGreenBlueAlpha);

			byte[] data = new byte[width * height * (int)ColorComponents.RedGreenBlueAlpha];
			Marshal.Copy(new IntPtr(ptr), data, 0, data.Length);
			return new Image(width, height, comp, data);
		}
		finally
		{
			if (ptr != null)
			{
				Marshal.FreeHGlobal(new IntPtr(ptr));
			}
		}
	}
}
