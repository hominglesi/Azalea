using System;
using System.Collections.Generic;

namespace Azalea.Text;
public class Font
{
	/// <summary>
	/// Dictionary of start offsets for all the different font tables in this font. 
	/// </summary>
	public Dictionary<string, uint> FontTableOffsets { get; }

	/// <summary>
	/// Array of all the glyphs contained in this font.
	/// </summary>
	public Glyph[] Glyphs { get; }

	public uint UnitsPerEm { get; }

	private Dictionary<uint, Glyph> _glyphTable;

	public Font(Dictionary<string, uint> fontTableOffsets, Glyph[] glyphs, uint unitsPerEm)
	{
		FontTableOffsets = fontTableOffsets;
		Glyphs = glyphs;
		UnitsPerEm = unitsPerEm;

		_glyphTable = new();

		foreach (Glyph g in glyphs)
		{
			if (g.Coordinates == null) continue;
			_glyphTable.Add(g.Unicode, g);
		}
	}

	public Glyph GetGlyph(uint unicode)
	{
		if (_glyphTable.ContainsKey(unicode) == false)
			throw new Exception("No glyph found in font");

		return _glyphTable[unicode];
	}
}
