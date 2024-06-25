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
	public void SkipBytes(uint bytes) => BaseStream.Position += bytes;

	public void GoTo(uint bytes) => BaseStream.Position = bytes;

	public void GoTo(long bytes) => BaseStream.Position = bytes;

	public uint GetLocation() => (uint)BaseStream.Position;

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

		var unicodeToGlyphMappings = parseUnicodeToGlyphMappings(fontTableOffsets);
		var glyphLocations = parseAllGlyphLocations(fontTableOffsets);
		var allGlyphs = parseAllGlyphs(glyphLocations, unicodeToGlyphMappings);
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

	private (uint, uint)[] parseUnicodeToGlyphMappings(Dictionary<string, uint> fontTableOffsets)
	{
		var mappings = new List<(uint, uint)>();
		uint cmapTableOffset = fontTableOffsets["cmap"];
		GoTo(cmapTableOffset + 2);

		var subtableCount = ReadUInt16();

		var cmapSubtableOffset = uint.MaxValue;
		for (int i = 0; i < subtableCount; i++)
		{
			var platformID = ReadUInt16();
			var platformSpecificID = ReadUInt16();
			var offset = ReadUInt32();

			// Platform ID 0 is Unicode
			if (platformID == 0)
			{
				if (platformSpecificID == 4)
					cmapSubtableOffset = offset;

				if (platformSpecificID == 3 && cmapSubtableOffset == uint.MaxValue)
					cmapSubtableOffset = offset;
			}
		}

		if (cmapSubtableOffset == uint.MaxValue)
			throw new Exception("Font doesn't have a supported Unicode subtable");

		GoTo(cmapTableOffset + cmapSubtableOffset);

		var format = ReadUInt16();

		if (format == 4)
		{
			SkipBytes(4); // Skip byteLength and languageCode

			var segmentCountX2 = ReadUInt16();
			var segmentCount = segmentCountX2 / 2;

			SkipBytes(6); // Skip searchRange, entrySelector, and rangeShift

			var endCodes = new int[segmentCount];
			for (int i = 0; i < segmentCount; i++)
				endCodes[i] = ReadUInt16();

			SkipBytes(2); // Skip reserved

			var startCodes = new int[segmentCount];
			for (int i = 0; i < segmentCount; i++)
				startCodes[i] = ReadUInt16();

			var idDeltas = new int[segmentCount];
			for (int i = 0; i < segmentCount; i++)
				idDeltas[i] = ReadUInt16();

			(int offset, int readLocation)[] idRangeOffsets = new (int, int)[segmentCount];
			for (int i = 0; i < segmentCount; i++)
			{
				var readLocation = (int)GetLocation();
				var offset = ReadUInt16();
				idRangeOffsets[i] = (offset, readLocation);
			}

			for (int i = 0; i < segmentCount; i++)
			{
				var endCode = endCodes[i];
				var currentCode = startCodes[i];

				while (currentCode <= endCode)
				{
					int glyphIndex = 0;

					if (idRangeOffsets[i].offset == 0)
					{
						glyphIndex = (currentCode + idDeltas[i]) % 65536;
					}
					else
					{
						var rangeOffsetLocation = idRangeOffsets[i].readLocation + idRangeOffsets[i].offset;
						var glyphIndexArrayLocation = 2 * (currentCode - startCodes[i]) + rangeOffsetLocation;

						var readerLocationOld = GetLocation();
						GoTo(glyphIndexArrayLocation);
						var glyphIndexOffset = ReadUInt16();
						GoTo(readerLocationOld);

						if (glyphIndexOffset != 0)
							glyphIndex = (glyphIndexOffset + idDeltas[i]) % 65536;
					}

					mappings.Add(((uint)glyphIndex, (uint)currentCode));
					currentCode++;
				}
			}
		}
		else if (format == 12)
		{
			SkipBytes(10); // Skip reserved, byteLength, and languageCode
			var groupsCount = ReadUInt32();

			for (int i = 0; i < groupsCount; i++)
			{
				var startCharCode = ReadUInt32();
				var endCharCode = ReadUInt32();
				var startGlyphIndex = ReadUInt32();

				var charCount = endCharCode - startCharCode + 1;
				for (int charCodeOffset = 0; charCodeOffset < charCount; charCodeOffset++)
				{
					uint charCode = (uint)(startCharCode + charCodeOffset);
					uint glyphIndex = (uint)(startGlyphIndex + charCodeOffset);

					mappings.Add((glyphIndex, charCode));
				}
			}
		}
		else
			throw new Exception("Font subtable version is not supported");

		return mappings.ToArray();
	}

	public Glyph[] parseAllGlyphs(uint[] glyphLocations, (uint, uint)[] unicodeMappings)
	{
		var glyphs = new Glyph[unicodeMappings.Length];
		for (int i = 0; i < glyphs.Length; i++)
		{
			GoTo(glyphLocations[unicodeMappings[i].Item1]);
			var contourCount = ReadInt16();
			var isSimple = contourCount > 0;

			Glyph glyph = new();

			if (isSimple)
				glyph = readSimpleGlyph(contourCount);

			glyph.Unicode = unicodeMappings[i].Item2;
			glyphs[i] = glyph;
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
