using Azalea.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace Azalea.Text;

public static class FontParser
{
	public static Font Parse(Stream stream)
	{
		using var reader = new FontReader(stream);

		var tableOffsets = ParseTableOffsets(reader);

		var glyphToUnicodeMappings = ParseGlyphToUnicodeMappings(reader, tableOffsets);
		var glyphLocations = ParseGlyphLocations(reader, tableOffsets);
		var glyphs = ParseGlyphs(reader, glyphLocations, glyphToUnicodeMappings);

		var unitsPerEm = ParseUnitsPerEm(reader, tableOffsets);

		return new Font(glyphs, unitsPerEm);
	}

	internal static Dictionary<string, uint> ParseTableOffsets(FontReader reader)
	{
		var tableOffsets = new Dictionary<string, uint>();

		GoToFontDirectory(reader);
		reader.SkipBytes(4); // Skip skalerType

		var fontTableCount = reader.ReadUInt16();

		GoToTableDirectory(reader);
		for (int i = 0; i < fontTableCount; i++)
		{
			var name = reader.ReadTag();
			reader.SkipBytes(4); // Skip checkSum
			var offset = reader.ReadUInt32();
			reader.SkipBytes(4); // Skip length

			tableOffsets[name] = offset;
		}

		return tableOffsets;
	}

	internal static GlyphToUnicodeMapping[] ParseGlyphToUnicodeMappings(
		FontReader reader, Dictionary<string, uint> tableOffsets)
	{
		List<(uint GlyphIndex, uint UnicodeValue)> glyphToUnicodeList = new();

		uint cmapTableOffset = tableOffsets["cmap"];
		reader.GoTo(cmapTableOffset);
		reader.SkipBytes(2); // Skip version

		var subtableCount = reader.ReadUInt16();

		var cmapSubtableOffset = uint.MaxValue;
		for (int i = 0; i < subtableCount; i++)
		{
			var platformID = reader.ReadUInt16();
			var platformSpecificID = reader.ReadUInt16();
			var offset = reader.ReadUInt32();

			if (platformID == 0) // Unicode
			{
				if (platformSpecificID == 4) // Unicode 2.0 or later (non-BMP allowed)
					cmapSubtableOffset = offset;

				if (platformSpecificID == 3 && cmapSubtableOffset == uint.MaxValue) // Unicode 2.0 or later (BMP only)
					cmapSubtableOffset = offset;
			}
		}

		if (cmapSubtableOffset == uint.MaxValue)
			throw new Exception("Font doesn't have a supported Unicode subtable");

		reader.GoTo(cmapTableOffset + cmapSubtableOffset);

		var format = reader.ReadUInt16();

		if (format == 4)
		{
			reader.SkipBytes(4); // Skip length and language

			var segmentCountX2 = reader.ReadUInt16();
			var segmentCount = segmentCountX2 / 2;

			reader.SkipBytes(6); // Skip searchRange, entrySelector, and rangeShift

			var endCodes = new int[segmentCount];
			for (int i = 0; i < segmentCount; i++)
				endCodes[i] = reader.ReadUInt16();

			reader.SkipBytes(2); // Skip reserved

			var startCodes = new int[segmentCount];
			for (int i = 0; i < segmentCount; i++)
				startCodes[i] = reader.ReadUInt16();

			var idDeltas = new int[segmentCount];
			for (int i = 0; i < segmentCount; i++)
				idDeltas[i] = reader.ReadUInt16();

			(int Offset, int ReadLocation)[] idRangeOffsets = new (int, int)[segmentCount];
			for (int i = 0; i < segmentCount; i++)
			{
				var readLocation = (int)reader.GetLocation();
				var offset = reader.ReadUInt16();
				idRangeOffsets[i] = (offset, readLocation);
			}

			for (int i = 0; i < segmentCount; i++)
			{
				var endCode = endCodes[i];
				var currentCode = startCodes[i];

				while (currentCode <= endCode)
				{
					int glyphIndex = 0;

					if (idRangeOffsets[i].Offset == 0)
					{
						glyphIndex = (currentCode + idDeltas[i]) % 65536;
					}
					else
					{
						var rangeOffsetLocation = idRangeOffsets[i].ReadLocation + idRangeOffsets[i].Offset;
						var glyphIndexArrayLocation = 2 * (currentCode - startCodes[i]) + rangeOffsetLocation;

						var readerLocationOld = reader.GetLocation();
						reader.GoTo(glyphIndexArrayLocation);
						var glyphIndexOffset = reader.ReadUInt16();
						reader.GoTo(readerLocationOld);

						if (glyphIndexOffset != 0)
							glyphIndex = (glyphIndexOffset + idDeltas[i]) % 65536;
					}

					glyphToUnicodeList.Add(((uint)glyphIndex, (uint)currentCode));
					currentCode++;
				}
			}
		}
		else if (format == 12)
		{
			reader.SkipBytes(10); // Skip reserved, length, and language
			var groupsCount = reader.ReadUInt32();

			for (int i = 0; i < groupsCount; i++)
			{
				var startCharCode = reader.ReadUInt32();
				var endCharCode = reader.ReadUInt32();
				var startGlyphIndex = reader.ReadUInt32();

				var charCount = endCharCode - startCharCode + 1;
				for (int charCodeOffset = 0; charCodeOffset < charCount; charCodeOffset++)
				{
					uint charCode = (uint)(startCharCode + charCodeOffset);
					uint glyphIndex = (uint)(startGlyphIndex + charCodeOffset);

					glyphToUnicodeList.Add((glyphIndex, charCode));
				}
			}
		}
		else
			throw new Exception("Font subtable version is not supported");

		var listToUnicodeMappings = new GlyphToUnicodeMapping[glyphToUnicodeList.Count];
		for (int i = 0; i < listToUnicodeMappings.Length; i++)
		{
			var glyphIndex = glyphToUnicodeList[i].GlyphIndex;
			var unicodeValue = glyphToUnicodeList[i].UnicodeValue;
			listToUnicodeMappings[i] = new GlyphToUnicodeMapping(glyphIndex, unicodeValue);
		}

		return listToUnicodeMappings;
	}

	internal static uint[] ParseGlyphLocations(FontReader reader, Dictionary<string, uint> tableOffsets)
	{
		reader.GoTo(tableOffsets["maxp"]);
		reader.SkipBytes(4); // Skip version
		var glyphCount = reader.ReadUInt16();

		reader.GoTo(tableOffsets["head"]);
		reader.SkipBytes(50); // Skip until indexToLocFormat
		var isShort = reader.ReadInt16() == 0;

		var glyphLocations = new uint[glyphCount];
		var locaTableOffset = tableOffsets["loca"];
		var glyfTableOffset = tableOffsets["glyf"];
		for (int i = 0; i < glyphCount; i++)
		{
			var glyphLocationOffset = i * (isShort ? 2 : 4);
			reader.GoTo(locaTableOffset + glyphLocationOffset);

			var glyphOffset = isShort ? reader.ReadUInt16() * 2u : reader.ReadUInt32();
			glyphLocations[i] = glyfTableOffset + glyphOffset;
		}

		return glyphLocations;
	}

	internal static Glyph[] ParseGlyphs(FontReader reader,
		uint[] glyphLocations, GlyphToUnicodeMapping[] glyphToUnicodeMappings)
	{
		var glyphs = new Glyph[glyphToUnicodeMappings.Length];
		for (int i = 0; i < glyphs.Length; i++)
		{
			var glyphLocation = glyphLocations[glyphToUnicodeMappings[i].GlyphIndex];
			reader.GoTo(glyphLocation);
			var contourCount = reader.ReadInt16();
			var isSimple = contourCount > 0;

			Glyph glyph;

			if (isSimple)
			{
				var simpleGlyph = ReadSimpleGlyph(reader, contourCount);
				var size = simpleGlyph.MaxCoordinates - simpleGlyph.MinCoordinates;

				glyph = new Glyph(simpleGlyph.Coordinates, simpleGlyph.ContourEndIndices,
					i, glyphToUnicodeMappings[i].UnicodeValue, size);
			}
			else
			{
				glyph = new Glyph();
			}

			glyphs[i] = glyph;
		}

		return glyphs;
	}

	internal static SimpleGlyph ReadSimpleGlyph(FontReader reader, short contourCount)
	{
		var minCoordinates = new Vector2Int(reader.ReadInt16(), reader.ReadInt16());
		var maxCoordinates = new Vector2Int(reader.ReadInt16(), reader.ReadInt16());

		int[] contourEndIndices = new int[contourCount];
		for (int i = 0; i < contourEndIndices.Length; i++)
			contourEndIndices[i] = reader.ReadUInt16();

		var instructionLength = reader.ReadUInt16();
		reader.SkipBytes(instructionLength);

		var indexCount = contourEndIndices[^1] + 1;
		var flags = new byte[indexCount];

		for (int i = 0; i < indexCount; i++)
		{
			byte flag = reader.ReadByte();
			flags[i] = flag;

			// If REPEAT bit is set, read next byte to detirmine number of copies
			if (BitwiseUtils.GetSpecificBit(flag, 3))
			{
				var copies = reader.ReadByte();

				for (int j = 0; j < copies; j++)
					flags[++i] = flag;
			}
		}

		var coordsX = ReadGlyphCoordinates(reader, flags, readingX: true);
		var coordsY = ReadGlyphCoordinates(reader, flags, readingX: false);

		//Flip the glyph to align it with our coordinate space
		for (int i = 0; i < coordsY.Length; i++)
			coordsY[i] *= -1;

		var coordinates = new Vector2Int[coordsX.Length];
		for (int i = 0; i < coordinates.Length; i++)
			coordinates[i] = new(coordsX[i], coordsY[i]);

		return new SimpleGlyph(coordinates, contourEndIndices, minCoordinates, maxCoordinates);
	}

	internal static int[] ReadGlyphCoordinates(FontReader reader, byte[] glyphFlags, bool readingX)
	{
		int offsetSizeFlagBit = readingX ? 1 : 2;
		int offsetSignOrSkipBit = readingX ? 4 : 5;
		int[] coordinates = new int[glyphFlags.Length];

		for (int i = 0; i < coordinates.Length; i++)
		{
			// Coordinate starts at previous value
			coordinates[i] = coordinates[Math.Max(0, i - 1)];
			byte flag = glyphFlags[i];

			// Offset value is represented with 1 byte (unsigned)
			if (BitwiseUtils.GetSpecificBit(flag, offsetSizeFlagBit))
			{
				byte offset = reader.ReadByte();
				int sign = BitwiseUtils.GetSpecificBit(flag, offsetSignOrSkipBit) ? 1 : -1;
				coordinates[i] += offset * sign;
			}

			// Offset value is represented with 2 bytes (signed)
			// (Unless flag tells us to skip it and just keep the coordinate the same)
			else if (BitwiseUtils.GetSpecificBit(flag, offsetSignOrSkipBit) == false)
				coordinates[i] += reader.ReadInt16();
		}

		return coordinates;
	}

	internal static uint ParseUnitsPerEm(FontReader reader, Dictionary<string, uint> tableOffsets)
	{
		reader.GoTo(tableOffsets["head"]);
		reader.SkipBytes(18); // Skip until unitsPerEm
		return reader.ReadUInt16();
	}

	internal static void GoToFontDirectory(FontReader reader) => reader.GoTo(0);
	internal static void GoToTableDirectory(FontReader reader) => reader.GoTo(12);

	internal readonly struct GlyphToUnicodeMapping
	{
		public readonly uint GlyphIndex { get; }
		public readonly uint UnicodeValue { get; }

		public GlyphToUnicodeMapping(uint glyphIndex, uint unicodeValue)
		{
			GlyphIndex = glyphIndex;
			UnicodeValue = unicodeValue;
		}
	}

	internal readonly struct SimpleGlyph
	{
		public readonly Vector2Int[] Coordinates { get; }
		public readonly int[] ContourEndIndices { get; }
		public readonly Vector2Int MinCoordinates { get; }
		public readonly Vector2Int MaxCoordinates { get; }

		public SimpleGlyph(Vector2Int[] coordinates, int[] contourEndIndices,
			Vector2Int minCoordinates, Vector2Int maxCoordinates)
		{
			Coordinates = coordinates;
			ContourEndIndices = contourEndIndices;

			MinCoordinates = minCoordinates;
			MaxCoordinates = maxCoordinates;
		}
	}
}
