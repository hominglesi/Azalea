namespace Azalea.Inputs.Events;

public class CharInputEvent(char chr) : InputEvent
{
	public readonly char Character = chr;

	public void Deconstruct(out char chr)
	{
		chr = Character;
	}
}
