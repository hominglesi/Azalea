namespace Azalea.Text;

public interface ITexturedGlyphLookupStore
{
	ITexturedCharacterGlyph? Get(string fontName, char character);
}
