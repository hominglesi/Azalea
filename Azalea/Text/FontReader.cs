using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace Azalea.Text;

/// <summary>
/// Binary reader specialized for reading True Type Font (.ttf) files
/// </summary>
public class FontReader : BinaryReader
{
	public FontReader(Stream stream)
		: base(stream) { }

	/// <summary>
	/// Seeks forward by the specified amount of bytes
	/// </summary>
	/// <param name="bytes">Specifies the number of bytes to skip</param>
	public void SkipBytes(uint bytes)
		=> BaseStream.Position += bytes;

	public void GoTo(uint bytes)
		=> BaseStream.Position = bytes;

	public void GoTo(long bytes)
		=> BaseStream.Position = bytes;

	/// <summary>
	/// Reads a 2-byte unsigned integer from the current stream using big-endian encoding 
	/// and advances the position of the stream by two bytes.
	/// </summary>
	/// <returns>A 2-byte unsigned integer read from this stream.</returns>
	public override ushort ReadUInt16()
	{
		var value = base.ReadUInt16();

		if (BitConverter.IsLittleEndian)
			value = (ushort)(value >> 8 | value << 8);

		return value;
	}

	/// <summary>
	/// Reads a 4-byte unsigned integer from the current stream using big-endian encoding 
	/// and advances the position of the stream by four bytes.
	/// </summary>
	/// <returns>A 4-byte unsigned integer read from this stream.</returns>
	public override uint ReadUInt32()
	{
		var value = base.ReadUInt32();

		if (BitConverter.IsLittleEndian)
		{
			var b1 = (value >> 0) & 0xff;
			var b2 = (value >> 8) & 0xff;
			var b3 = (value >> 16) & 0xff;
			var b4 = (value >> 24) & 0xff;

			value = b1 << 24 | b2 << 16 | b3 << 8 | b4 << 0;
		}

		return value;
	}

	public override short ReadInt16() => (short)ReadUInt16();

	/// <summary>
	/// Reads a 4 character string from the current stream
	/// and advances the position of the stream by 4 bytes.
	/// </summary>
	/// <returns>A 4 character string read from the stream.</returns>
	public string ReadTag()
	{
		Span<char> tag = stackalloc char[4];

		for (int i = 0; i < tag.Length; i++)
			tag[i] = (char)base.ReadByte();

		return tag.ToString();
	}

	public Dictionary<string, uint> ReadFontTableOffsets(int numberOfTables)
	{
		var table = new Dictionary<string, uint>();

		for (int i = 0; i < numberOfTables; i++)
		{
			var tag = ReadTag();
			var checksum = ReadUInt32();
			var offset = ReadUInt32();
			var length = ReadUInt32();
			table.Add(tag, offset);
		}

		return table;
	}

	public Glyph ReadSimpleGlyph()
	{
		var numberOfContours = ReadInt16();

		var xMin = ReadInt16();
		var yMin = ReadInt16();
		var xMax = ReadInt16();
		var yMax = ReadInt16();

		int[] contourEndIndices = new int[numberOfContours];
		for (int i = 0; i < contourEndIndices.Length; i++)
			contourEndIndices[i] = ReadUInt16();

		SkipBytes(ReadUInt16()); // Skip Instructions

		var indexCount = contourEndIndices[^1] + 1;
		var flags = new byte[indexCount];

		for (int i = 0; i < indexCount; i++)
		{
			byte flag = ReadByte();
			flags[i] = flag;

			// If REPEAT bit is set, read next byte to detirmine num copies
			if (BitwiseUtils.GetSpecificBit(flag, 3))
			{
				var copies = ReadByte();

				for (int j = 0; j < copies; j++)
					flags[++i] = flag;
			}
		}

		var coordsX = readCoordinates(flags, readingX: true);
		var coordsY = readCoordinates(flags, readingX: false);

		for (int i = 0; i < coordsY.Length; i++)
		{
			coordsY[i] *= -1;
		}

		return new Glyph(contourEndIndices, coordsX, coordsY);
	}

	private int[] readCoordinates(byte[] flags, bool readingX)
	{
		int offsetSizeFlagBit = readingX ? 1 : 2;
		int offsetSignOrSkipBit = readingX ? 4 : 5;
		int[] coordinates = new int[flags.Length];

		for (int i = 0; i < coordinates.Length; i++)
		{
			// Coordinate starts at previous value
			coordinates[i] = coordinates[Math.Max(0, i - 1)];
			byte flag = flags[i];
			bool onCurve = BitwiseUtils.GetSpecificBit(flag, 0);

			// Offset value is represented with 1 byte (unsigned)
			if (BitwiseUtils.GetSpecificBit(flag, offsetSizeFlagBit))
			{
				byte offset = ReadByte();
				int sign = BitwiseUtils.GetSpecificBit(flag, offsetSignOrSkipBit) ? 1 : -1;
				coordinates[i] += offset * sign;
			}

			// Offset value is represented with 2 bytes (signed)
			// (Unless flag tells us to skip it and just keep the coordinate the same)
			else if (BitwiseUtils.GetSpecificBit(flag, offsetSignOrSkipBit) == false)
				coordinates[i] += ReadInt16();
		}

		return coordinates;
	}
}
