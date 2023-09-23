namespace Azalea.Inputs.Events;

public abstract class KeyEvent : InputEvent
{
	public readonly Keys Key;

	public KeyEvent(Keys key)
	{
		Key = key;
	}
}
