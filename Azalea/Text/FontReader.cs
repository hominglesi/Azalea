using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace Azalea.Text;

/// <summary>
/// Binary reader specialized for reading True Type Font (.ttf) files and creating a <see cref="Font"/> file from them.
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

	/// <summary>
	/// Parses all the data of a font file from the current stream
	/// </summary>
	/// <returns>Font data from the current stream </returns>
	public Font ParseFont()
	{
		var fontTableOffsets = parseFontTableOffsets();

		var allGlyphs = parseAllGlyphs(fontTableOffsets);
		var unitsPerEm = parseUnitsPerEm(fontTableOffsets);

		return new Font(fontTableOffsets, allGlyphs, unitsPerEm);
	}

	private Dictionary<string, uint> parseFontTableOffsets()
	{
		GoTo(0); // Go to Font Directory
		SkipBytes(4); // Skip skaler type value
		var fontTableCount = ReadUInt16();

		GoTo(12); // Go to Table Directory
		var fontTableOffsets = new Dictionary<string, uint>();
		for (int i = 0; i < fontTableCount; i++)
		{
			var name = ReadTag();
			SkipBytes(4);
			var offset = ReadUInt32();
			SkipBytes(4);
			fontTableOffsets[name] = offset;
		}

		return fontTableOffsets;
	}

	private uint[] parseAllGlyphLocations(Dictionary<string, uint> fontTableOffsets)
	{
		GoTo(fontTableOffsets["maxp"] + 4);
		var glyphCount = ReadUInt16();

		GoTo(fontTableOffsets["head"] + 50);
		var isTwoBytes = ReadInt16() == 0;

		var locaTableOffset = fontTableOffsets["loca"];
		var glyfTableOffset = fontTableOffsets["glyf"];
		var allGlyphLocations = new uint[glyphCount];
		for (int i = 0; i < glyphCount; i++)
		{
			var glyphLocationOffset = i * (isTwoBytes ? 2 : 4);
			GoTo(locaTableOffset + glyphLocationOffset);

			var glyphOffset = isTwoBytes ? ReadUInt16() * 2u : ReadUInt32();
			allGlyphLocations[i] = glyfTableOffset + glyphOffset;
		}

		return allGlyphLocations;
	}

	public Glyph[] parseAllGlyphs(Dictionary<string, uint> fontTableOffsets)
	{
		var allGlyphLocations = parseAllGlyphLocations(fontTableOffsets);

		var glyphs = new Glyph[allGlyphLocations.Length];
		for (int i = 0; i < allGlyphLocations.Length; i++)
		{
			GoTo(allGlyphLocations[i]);
			var contourCount = ReadInt16();
			var isSimple = contourCount > 0;

			if (isSimple)
				glyphs[i] = readSimpleGlyph(contourCount);
		}

		return glyphs;
	}

	private uint parseUnitsPerEm(Dictionary<string, uint> fontTableOffsets)
	{
		GoTo(fontTableOffsets["head"] + 18);
		return ReadUInt16();
	}

	private Glyph readSimpleGlyph(int contourCount)
	{
		SkipBytes(8); // Skip glyph bounds

		int[] contourEndIndices = new int[contourCount];
		for (int i = 0; i < contourEndIndices.Length; i++)
			contourEndIndices[i] = ReadUInt16();

		var instructionLength = ReadUInt16();
		SkipBytes(instructionLength);

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

		//Flip the glyph to align it with our coordinate space
		for (int i = 0; i < coordsY.Length; i++)
			coordsY[i] *= -1;

		var coordinates = new Vector2Int[coordsX.Length];
		for (int i = 0; i < coordinates.Length; i++)
			coordinates[i] = new(coordsX[i], coordsY[i]);

		return new Glyph(coordinates, contourEndIndices);
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
