using System;
using System.IO;

namespace Azalea.Extentions;

public static class StreamExtentions
{
	public static void ReadToFill(this Stream stream, Span<byte> buffer)
	{
		Span<byte> remainingBuffer = buffer;

		while (!remainingBuffer.IsEmpty)
		{
			int bytesRead = stream.Read(remainingBuffer);
			remainingBuffer = remainingBuffer[bytesRead..];

			if (bytesRead == 0)
				throw new EndOfStreamException();
		}
	}

	public static byte[] ReadBytesToArray(this Stream stream, int length)
	{
		byte[] bytes = new byte[length];
		stream.ReadToFill(bytes);
		return bytes;
	}

	public static byte[] ReadAllBytesToArray(this Stream stream)
	{
		if (stream.CanSeek == false)
			throw new ArgumentException("Stream must be seekable");

		if (stream.Length > Array.MaxLength)
			throw new ArgumentException("Stream is too big for an array");

		stream.Seek(0, SeekOrigin.Begin);
		return stream.ReadBytesToArray((int)stream.Length);
	}
}
