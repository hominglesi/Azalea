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

	public Font(Dictionary<string, uint> fontTableOffsets, Glyph[] glyphs, uint unitsPerEm)
	{
		FontTableOffsets = fontTableOffsets;
		Glyphs = glyphs;
		UnitsPerEm = unitsPerEm;
	}
}
