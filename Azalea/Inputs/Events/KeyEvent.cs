namespace Azalea.Inputs.Events;

public abstract class KeyEvent(Keys key) : InputEvent
{
	public readonly Keys Key = key;

	public void Deconstruct(out Keys key)
	{
		key = Key;
	}
}
