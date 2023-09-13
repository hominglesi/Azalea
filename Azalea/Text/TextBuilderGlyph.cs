using Azalea.Graphics.Textures;
using Azalea.Numerics;

namespace Azalea.Text;

public struct TextBuilderGlyph : ITexturedCharacterGlyph
{
	public readonly Texture Texture => Glyph.Texture;
	public readonly float XOffset => ((_fixedWidth - Glyph.Width) / 2 ?? Glyph.XOffset) * _textSize;
	public readonly float XAdvance => (_fixedWidth ?? Glyph.XAdvance) * _textSize;
	public readonly float Width => Glyph.Width * _textSize;
	public readonly char Character => Glyph.Character;

	public readonly float YOffset => _useFontSizeAsHeight ? Glyph.YOffset * _textSize : 0;
	public readonly float Baseline => _useFontSizeAsHeight ? Glyph.Baseline * _textSize : (Glyph.Baseline - Glyph.YOffset) * _textSize;
	public readonly float Height => Glyph.IsWhiteSpace() ? 0 : Glyph.Height * _textSize;

	public readonly ITexturedCharacterGlyph Glyph;

	public Rectangle DrawRectangle { get; internal set; }

	public float LinePosition { get; internal set; }

	public bool OnNewLine { get; internal set; }

	private readonly float _textSize;
	private readonly float? _fixedWidth;
	private readonly bool _useFontSizeAsHeight;

	public TextBuilderGlyph(ITexturedCharacterGlyph glyph, float textSize, float? fixedWidth = null, bool useFontSizeAsHeight = true)
	{
		this = default;
		_textSize = textSize;
		_fixedWidth = fixedWidth;
		_useFontSizeAsHeight = useFontSizeAsHeight;

		Glyph = glyph;
	}

	public readonly float GetKerning<T>(T lastGlyph)
		where T : ICharacterGlyph
		=> _fixedWidth != null ? 0 : Glyph.GetKerning(lastGlyph);
}
