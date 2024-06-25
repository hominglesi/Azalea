using System;
using System.Collections.Generic;

namespace Azalea.Text;
public class Font
{
	public Glyph[] Glyphs { get; }
	public uint UnitsPerEm { get; }

	private Glyph _missingGlyph;
	private Dictionary<uint, Glyph> _glyphTable;

	public Font(Glyph[] glyphs, uint unitsPerEm)
	{
		Glyphs = glyphs;
		UnitsPerEm = unitsPerEm;

		_glyphTable = new();

		foreach (Glyph glyph in glyphs)
		{
			if (glyph.Coordinates == null) continue;

			_glyphTable.Add(glyph.UnicodeValue, glyph);

			if (glyph.GlyphIndex == 0)
				_missingGlyph = glyph;
		}
	}

	public Glyph GetGlyph(uint unicode)
	{
		if (_glyphTable.ContainsKey(unicode) == false)
		{
			if (_missingGlyph.Exists == false)
				throw new Exception($"This font doesn't contain a glyph for unicode value {unicode} " +
					$"and no fallback missing glyph");

			return _missingGlyph;
		}

		return _glyphTable[unicode];
	}
}
