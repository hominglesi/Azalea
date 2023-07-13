using Azalea.Text;

namespace Azalea.IO.Stores;

public interface IGlyphStore
{
    string? FontName { get; }

    bool HasGlyph(char character);

    CharacterGlyph? Get(char character);

    int GetKerning(char left, char right);
}
