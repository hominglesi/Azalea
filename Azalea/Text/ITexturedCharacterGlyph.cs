using Azalea.Graphics.Textures;

namespace Azalea.Text;

public interface ITexturedCharacterGlyph : ICharacterGlyph
{
    Texture Texture { get; }
    float Width { get; }
    float Height { get; }
}

public static class TexturedCharacterGlyphExtentions
{
    public static bool IsWhiteSpace<T>(this T glyph)
        where T : ITexturedCharacterGlyph
        => char.IsWhiteSpace(glyph.Character);
}
