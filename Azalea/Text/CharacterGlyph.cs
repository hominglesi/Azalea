namespace Azalea.Text;

public class CharacterGlyph : ICharacterGlyph
{
	public float XOffset { get; }
	public float YOffset { get; }
	public float XAdvance { get; }
	public float Baseline { get; }
	public char Character { get; }

	private readonly FontData _containingFont;

	public CharacterGlyph(char character, float xOffset, float yOffset, float xAdvance, float baseline, FontData containingStore)
	{
		_containingFont = containingStore;

		Character = character;
		XOffset = xOffset;
		YOffset = yOffset;
		XAdvance = xAdvance;
		Baseline = baseline;
	}

	public float GetKerning<T>(T lastGlyph)
		where T : ICharacterGlyph
		=> _containingFont.GetKerning(lastGlyph.Character, Character);
}
