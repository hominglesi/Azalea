using Azalea.IO.Stores;

namespace Azalea.Text;

public class CharacterGlyph : ICharacterGlyph
{
	public float XOffset { get; }
	public float YOffset { get; }
	public float XAdvance { get; }
	public float Baseline { get; }
	public char Character { get; }

	private readonly IGlyphStore? _containingStore;

	public CharacterGlyph(char character, float xOffset, float yOffset, float xAdvance, float baseline, IGlyphStore? containingStore)
	{
		_containingStore = containingStore;

		Character = character;
		XOffset = xOffset;
		YOffset = yOffset;
		XAdvance = xAdvance;
		Baseline = baseline;
	}

	public float GetKerning<T>(T lastGlyph)
		where T : ICharacterGlyph
		=> _containingStore?.GetKerning(lastGlyph.Character, Character) ?? 0;
}
