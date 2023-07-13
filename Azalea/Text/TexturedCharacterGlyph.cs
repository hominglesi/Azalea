using Azalea.Graphics.Textures;
using System.Runtime.CompilerServices;

namespace Azalea.Text;

public class TexturedCharacterGlyph : ITexturedCharacterGlyph
{
    public Texture Texture { get; }

    public float XOffset => _glyph.XOffset * Scale;
    public float YOffset => _glyph.YOffset * Scale;
    public float XAdvance => _glyph.XAdvance * Scale;
    public float Baseline => _glyph.Baseline * Scale;
    public char Character => _glyph.Character;
    public float Width => Texture.Width * Scale;
    public float Height => Texture.Height * Scale;

    public readonly float Scale;

    private readonly CharacterGlyph _glyph;

    public TexturedCharacterGlyph(CharacterGlyph glyph, Texture texture, float scale = 1)
    {
        _glyph = glyph;
        Texture = texture;
        Scale = scale;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float GetKerning<T>(T lastGlyph)
        where T : ICharacterGlyph
        => _glyph.GetKerning(lastGlyph) * Scale;

}
